using Arcturus.Repository.Abstracts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection;

namespace Arcturus.Repository.EntityFrameworkCore.SqlServer.Internals;

internal static class EfAddOrUpdateExtensions
{
    // Public result type — tells you if we updated or inserted
    
    /// <summary>
    /// Add or Update a single value using an expression predicate.
    /// - SQL Server: emits a single MERGE with parameters (fast path).
    /// - Other providers: ExecuteUpdateAsync; if 0 rows updated, inserts the value.
    /// 
    /// updateColumns (optional): property selector such as
    ///   x => x.Name
    ///   x => new { x.Name, x.Price }
    ///   x => new object[] { x.Name, x.Price }
    /// If null, defaults to all non-key, non-store-generated properties.
    /// </summary>
    internal static async Task AddOrUpdate<T>(
        this DbSet<T> set,
        T value,
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, object>>? updateColumns = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        // Fire and forget pattern requested by signature — but you probably want the result 😊
        _ = await AddOrUpdateInternal(set, value, predicate, updateColumns, cancellationToken);
    }

    /// <summary>
    /// Same as AddOrUpdate but returns an action result (recommended).
    /// </summary>
    internal static async Task<AddOrUpdateResult<T>> AddOrUpdateWithResult<T>(
        this DbSet<T> set,
        T value,
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, object>>? updateColumns = null,
        CancellationToken cancellationToken = default)
        where T : class
        => await AddOrUpdateInternal(set, value, predicate, updateColumns, cancellationToken);

    // ----------------- helpers -----------------

    private static async Task<AddOrUpdateResult<T>> AddOrUpdateInternal<T>(
        DbSet<T> set,
        T value,
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, object>>? updateColumns,
        CancellationToken cancellationToken)
        where T : class
    {
        var ctx = set.GetDbContext();

        // FAST PATH: SQL Server MERGE
        if (ctx.Database.IsSqlServer())
        {
            return await MergeOneSqlServer(set, value, predicate, updateColumns, cancellationToken);
        }

#if NET10_0_OR_GREATER
        // https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-10.0/breaking-changes#ExecuteUpdateAsync-lambda
        throw new NotImplementedException("EF Core 10+ supports ExecuteUpdate with MERGE; implement provider-specific fast paths here.");
#else
        // GENERIC PATH: ExecuteUpdate → Insert
        var et = set.EntityType();
        var updateExpr = BuildSetPropertyCalls(value, et, updateColumns);
        var updated = await set.Where(predicate).ExecuteUpdateAsync(updateExpr, cancellationToken);

        if (updated > 0)
        {
            return new AddOrUpdateResult<T>(Updated: true, RowsAffected: updated, InsertedEntity: null);
        }
#endif

        await set.AddAsync(value, cancellationToken);
        await ctx.SaveChangesAsync(cancellationToken);
        return new AddOrUpdateResult<T>(Updated: false, RowsAffected: 1, InsertedEntity: value);
    }

    /// <summary>
    /// SQL Server MERGE for a single row based on a LINQ predicate.
    /// We translate the predicate to an ON-clause **only** when it’s a pure equality
    /// over member(s) = constant(s) derived from <paramref name="value"/>.
    /// If the predicate doesn’t match that simple shape, we fall back to two-step path.
    /// </summary>
    private static async Task<AddOrUpdateResult<T>> MergeOneSqlServer<T>(
        DbSet<T> set,
        T value,
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, object>>? updateColumns,
        CancellationToken cancellationToken)
        where T : class
    {
        var ctx = set.GetDbContext();
        var et = set.EntityType();
        var (schema, table) = (et.GetSchema() ?? "dbo", et.GetTableName() ?? et.ClrType.Name);

        // Try to extract simple equality members from the predicate: e => e.A == value.A && e.B == value.B
        if (!TryExtractEqualityMembers(et, predicate, out var matchProps))
        {
            // fallback
            return await AddOrUpdateInternal(set, value, predicate, updateColumns, cancellationToken);
        }

        // Determine updatable columns (default: non-key, non-store-generated)
        var allProps = et.GetProperties().Where(p => p.IsColumnIncluded()).ToList();
        var updateProps = updateColumns is null
            ? allProps.Where(p => !p.IsKey() && !p.IsStoreGenerated()).ToList()
            : ExtractPropertiesFromSelector(et, updateColumns);

        var insertProps = allProps.Where(p => !p.IsStoreGenerated()).ToList();

        // src props = union to ensure every referenced column exists in VALUES
        var srcProps = insertProps.Concat(updateProps).Concat(matchProps).Distinct().ToList();

        // Build parameter list in srcProps order
        var parameters = new List<SqlParameter>();
        string Bracket(string s) => $"[{s}]";
        string Column(IProperty p) => Bracket(p.GetColumnName());
        var tAlias = "T";
        var sAlias = "S";

        var valuePlaceholders = new List<string>();
        for (int i = 0; i < srcProps.Count; i++)
        {
            var p = srcProps[i];
            var val = p.PropertyInfo!.GetValue(value);
            var prm = new SqlParameter($"@p{i}", val ?? DBNull.Value);
            var mapping = p.FindRelationalTypeMapping();
            if (mapping?.DbType is { } dbt) prm.DbType = dbt;
            parameters.Add(prm);
            valuePlaceholders.Add(prm.ParameterName);
        }

        // ON clause
        var onSql = string.Join(" AND ", matchProps.Select(p => $"{tAlias}.{Column(p)} = {sAlias}.{Column(p)}"));

        // SET clause
        var firstMatchProp = matchProps.First();
        var setSql = updateProps.Any()
            ? "SET " + string.Join(", ", updateProps.Select(p => $"{tAlias}.{Column(p)} = {sAlias}.{Column(p)}"))
            : $"SET {tAlias}.{Column(firstMatchProp)} = {tAlias}.{Column(firstMatchProp)}"; // no-op to keep MERGE valid

        // INSERT lists
        var insertCols = string.Join(", ", insertProps.Select(Column));
        var insertVals = string.Join(", ", insertProps.Select(p => $"{sAlias}.{Column(p)}"));

        // Source columns and row
        var srcCols = string.Join(", ", srcProps.Select(Column));
        var srcRow = string.Join(", ", valuePlaceholders);

        // OUTPUT the action so we know whether it was INSERT or UPDATE
        var sql =
$@"
MERGE {Bracket(schema)}.{Bracket(table)} AS {tAlias}
USING (VALUES ({srcRow})) AS {sAlias} ({srcCols})
ON {onSql}
WHEN MATCHED THEN
    UPDATE {setSql}
WHEN NOT MATCHED BY TARGET THEN
    INSERT ({insertCols})
    VALUES ({insertVals})
OUTPUT $action;
";

        // Execute and read a single $action row
        string? action;
        var conn = (SqlConnection)ctx.Database.GetDbConnection();  // No await using here—let DbContext manage disposal
        if (conn.State != System.Data.ConnectionState.Open)
        {
            await conn.OpenAsync(cancellationToken);
        }
        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddRange([.. parameters]);

        using var transaction = await ctx.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            action = (string?)await cmd.ExecuteScalarAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            return action?.Equals("UPDATE", StringComparison.OrdinalIgnoreCase) == true
                ? new AddOrUpdateResult<T>(Updated: true, RowsAffected: 1, InsertedEntity: null)
                : new AddOrUpdateResult<T>(Updated: false, RowsAffected: 1, InsertedEntity: value);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

    }

    // ----------------- small utilities -----------------

    // Expose the DbContext (you asked for GetContext 😉)
    public static DbContext GetDbContext<T>(this DbSet<T> set) where T : class
        => set.GetService<ICurrentDbContext>().Context;

    private static IEntityType EntityType<T>(this DbSet<T> set) where T : class
        => set.GetService<ICurrentDbContext>().Context.Model.FindEntityType(typeof(T))!;

    private static bool IsColumnIncluded(this IProperty p)
        => p.GetColumnName(StoreObjectIdentifier.Table(p.DeclaringEntityType.GetTableName()!, p.DeclaringEntityType.GetSchema())) != null;

    private static bool IsStoreGenerated(this IProperty p)
        => p.ValueGenerated == ValueGenerated.OnAdd
        || p.ValueGenerated == ValueGenerated.OnAddOrUpdate
        || p.IsConcurrencyToken
        || p.GetComputedColumnSql() != null;

    private static string GetColumnName(this IProperty p)
        => p.GetColumnName(StoreObjectIdentifier.Table(p.DeclaringEntityType.GetTableName()!, p.DeclaringEntityType.GetSchema()))!;

    private static RelationalTypeMapping? FindRelationalTypeMapping(this IProperty p)
        => p.FindTypeMapping() as RelationalTypeMapping;

    private static List<IProperty> ExtractPropertiesFromSelector<T>(IEntityType et, Expression<Func<T, object>> selector)
    {
        var members = new List<MemberExpression>();

        void Add(MemberExpression me)
        {
            if (me.Member is PropertyInfo) members.Add(me);
            else throw new NotSupportedException("Only property members are supported in updateColumns.");
        }

        switch (selector.Body)
        {
            case MemberExpression me:
                Add(me);
                break;
            case UnaryExpression { Operand: MemberExpression me }:
                Add(me);
                break;
            case NewExpression ne:
                foreach (var a in ne.Arguments)
                    if (a is MemberExpression me2) Add(me2);
                    else throw new NotSupportedException("Anonymous object must contain property members only.");
                break;
            case NewArrayExpression na:
                foreach (var a in na.Expressions)
                    if (a is MemberExpression me3) Add(me3);
                    else throw new NotSupportedException("Array must contain property members only.");
                break;
            default:
                throw new NotSupportedException("Unsupported updateColumns selector shape.");
        }

        return members.Select(m =>
        {
            var pi = (PropertyInfo)m.Member;
            var p = et.FindProperty(pi) ?? throw new InvalidOperationException($"Property '{pi.Name}' not mapped.");
            return p;
        }).ToList();
    }

    // Try to parse: e => e.A == value.A && e.B == value.B  (all AND of equalities vs constants from "value")
    private static bool TryExtractEqualityMembers<T>(
        IEntityType et
        , Expression<Func<T, bool>> predicate
        , out List<IProperty> members)
        where T : class
    {
        var tmp = new List<IProperty>(); // local collection to capture in lambdas

        bool ok = Visit(predicate.Body);

        // assign to out param only after all lambdas/local funcs are done
        members = ok && tmp.Count > 0 ? tmp : new List<IProperty>();
        return ok && tmp.Count > 0;

        bool Visit(Expression expr)
        {
            if (expr is BinaryExpression be && be.NodeType == ExpressionType.AndAlso)
                return Visit(be.Left) & Visit(be.Right);

            if (expr is BinaryExpression eq && eq.NodeType == ExpressionType.Equal)
            {
                // left: e.Prop    right: captured constant/member (typically value.Prop)
                if (eq.Left is MemberExpression lm && lm.Expression is ParameterExpression &&
                    eq.Right is MemberExpression rm)
                {
                    if (rm.Expression is ConstantExpression or MemberExpression)
                    {
                        var lpi = lm.Member as PropertyInfo
                                  ?? throw new NotSupportedException("Left side must be a property.");
                        var prop = et.FindProperty(lpi)
                                   ?? throw new InvalidOperationException($"Property '{lpi.Name}' not mapped.");
                        tmp.Add(prop); // capture local, not the 'out' param
                        return true;
                    }
                }
            }
            return false;
        }
    }


#if NET10_0_OR_GREATER
#else
    // Build: set => set.SetProperty(x => x.Prop, x => value.Prop) for chosen columns
    private static Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> BuildSetPropertyCalls<T>(
        T value,
        IEntityType et,
        Expression<Func<T, object>>? updateColumns)
        where T : class
    {
        var props = updateColumns is null
            ? et.GetProperties().Where(p => !p.IsKey() && !p.IsStoreGenerated()).ToList()
            : ExtractPropertiesFromSelector(et, updateColumns);

        var setParam = Expression.Parameter(typeof(SetPropertyCalls<T>), "set");
        Expression body = setParam;

        var setPropOpen = typeof(SetPropertyCalls<T>).GetMethods()
            .Single(m => m.Name == nameof(SetPropertyCalls<T>.SetProperty) && m.GetParameters().Length == 2);

        foreach (var p in props)
        {
            var pi = p.PropertyInfo!;
            var setProp = setPropOpen.MakeGenericMethod(pi.PropertyType);

            var eParam = Expression.Parameter(typeof(T), "e");
            var left = Expression.Lambda(Expression.Property(eParam, pi), eParam);

            var constVal = Expression.Constant(pi.GetValue(value), pi.PropertyType);
            var right = Expression.Lambda(constVal, eParam);

            body = Expression.Call(setProp, body, Expression.Quote(left), Expression.Quote(right));
        }

        return Expression.Lambda<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>(body, setParam);
    }
#endif
}
