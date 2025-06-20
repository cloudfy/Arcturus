using System.Linq.Expressions;

namespace Arcturus.Data.Repository.Abstracts;

public sealed class IncludableSpecificationBuilder<TEntity, TProperty>
{
    internal List<string> IncludePaths { get; } = new();
    internal Expression<Func<TEntity, object>> CurrentInclude { get; }

    public IncludableSpecificationBuilder(Expression<Func<TEntity, object>> include)
    {
        CurrentInclude = include;
    }
}

// version 2
//public abstract class SpecificationExecutor<T>(Specification<T> specification)
//{
//    protected Specification<T> Specification { get; } = specification;
//    public abstract IQueryable<T> Apply(IQueryable<T> queryable);
//}

//public class Specification<T>(Expression<Func<T, bool>> predicate)
//{
//    private readonly Expression<Func<T, bool>> _predicate = predicate;
//    private readonly List<Expression<Func<T, object>>> _includes = new();

//    public Expression<Func<T, bool>> Predicate => _predicate;
//    public IEnumerable<Expression<Func<T, object>>> Includes => _includes;

//    internal void AddInclude(Expression<Func<T, object>> includeExpression)
//    {
//        _includes.Add(includeExpression);
//    }
//}


//public class IncludableSpecificationBuilder<TEntity, TProperty>
//{
//    internal List<string> IncludePaths { get; } = new();
//    internal Expression<Func<TEntity, object>> CurrentInclude { get; }

//    public IncludableSpecificationBuilder(Expression<Func<TEntity, object>> include)
//    {
//        CurrentInclude = include;
//    }
//}

//public static class SpecificationExtensions
//{
//    public static IncludableSpecificationBuilder<TEntity, TProperty> Include<TEntity, TProperty>(
//        this Specification<TEntity> spec,
//        Expression<Func<TEntity, TProperty>> navigationPropertyPath)
//        where TEntity : class
//    {
//        var includeAsObject = Expression.Lambda<Func<TEntity, object>>(
//            Expression.Convert(navigationPropertyPath.Body, typeof(object)),
//            navigationPropertyPath.Parameters);

//        spec.AddInclude(includeAsObject);
//        return new IncludableSpecificationBuilder<TEntity, TProperty>(includeAsObject);
//    }

//    // These are no-ops, only syntactic sugar in this mock. EF Core's real ThenInclude is used when the IQueryable is executed.
//    public static IncludableSpecificationBuilder<TEntity, TNextProperty> ThenInclude<TEntity, TPreviousProperty, TNextProperty>(
//        this IncludableSpecificationBuilder<TEntity, IEnumerable<TPreviousProperty>> source,
//        Expression<Func<TPreviousProperty, TNextProperty>> navigationPropertyPath)
//    {
//        // Optionally track the include chain if needed
//        return new IncludableSpecificationBuilder<TEntity, TNextProperty>(source.CurrentInclude);
//    }

//    public static IncludableSpecificationBuilder<TEntity, TNextProperty> ThenInclude<TEntity, TPreviousProperty, TNextProperty>(
//        this IncludableSpecificationBuilder<TEntity, TPreviousProperty> source,
//        Expression<Func<TPreviousProperty, TNextProperty>> navigationPropertyPath)
//    {
//        return new IncludableSpecificationBuilder<TEntity, TNextProperty>(source.CurrentInclude);
//    }

//    private static Expression<Func<T, bool>> AsQueryable<T>(this Func<T, bool> predicate)
//    {
//        var param = Expression.Parameter(typeof(T));
//        var body = Expression.Invoke(Expression.Constant(predicate), param);
//        return Expression.Lambda<Func<T, bool>>(body, param);
//    }
//}

// version 1
//public sealed class Specification<T>(Expression<Func<T, bool>> predicate)
//{
//    private readonly Expression<Func<T, bool>> _predicate = predicate;
//    private readonly List<Expression<Func<T, object>>> _includes = new();

//    internal Expression<Func<T, bool>> Predicate => _predicate;
//    internal IEnumerable<Expression<Func<T, object>>> Includes => _includes;

//    internal void AddInclude(Expression<Func<T, object>> includeExpression)
//    {
//        _includes.Add(includeExpression);
//    }

//    internal IQueryable<T> ApplySpecification(IQueryable<T> queryable)
//    {
//        foreach (var include in _includes)
//        {
//            queryable = queryable.Include(include);
//        }

//        return queryable.Where(_predicate);
//    }
//}

//public interface IIncludableQueryable<out TEntity, out TProperty> : IQueryable<TEntity>;

//public class Test()
//{
//    public void Ko()
//    {
//        var specification = new Specification<ClassKo>(x => x.Id == 38);
//        specification.Include(_ => _.CDO).ThenInclude(_ => _.Ko);

//    }
//}

//public static class SpecificationExtensions
//{
//    public static IIncludableQueryable<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(
//        this IIncludableQueryable<TEntity, IEnumerable<TPreviousProperty>> source,
//        Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
//        where TEntity : class
//    {
//    }
//    public static IIncludableQueryable<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(
//        this IIncludableQueryable<TEntity, TPreviousProperty> source,
//        Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
//        where TEntity : class
//    {
//    }
//    public static IIncludableQueryable<TEntity, TProperty> Include<TEntity, TProperty>(
//        this Specification<TEntity> source
//        , Expression<Func<TEntity, TProperty>> navigationPropertyPath) where TEntity : class
//    {
//        //source.Add(navigationPropertyPath);
//        //Microsoft.EntityFrameworkCore.Utilities.Check.NotNull(navigationPropertyPath, "navigationPropertyPath");
//        //return new IncludableQueryable<TEntity, TProperty>((IQueryable<TEntity>)((source.Provider is EntityQueryProvider) ? ((IQueryable<object>)source.Provider.CreateQuery<TEntity>(Expression.Call(null, IncludeMethodInfo.MakeGenericMethod(typeof(TEntity), typeof(TProperty)), new Expression[2]
//        //{
//        //    source.Expression,
//        //    Expression.Quote(navigationPropertyPath)
//        //}))) : ((IQueryable<object>)source)));
//    }
//}

//public class ClassKo : IEntity<int>
//{
//    public int Id { get; set; }
//    public ClassDo CDO { get; set; }
//}
//public class ClassDo : IEntity<int>
//{
//    public int Id { get; set; }
//    public ClassKo Ko { get; set; }

//}