# Specification Include Chaining Guide

## Overview

The Arcturus Specification pattern now supports advanced include chaining with three key navigation methods:
- **`.Include()`** - Root-level includes (can be chained for siblings at root)
- **`.ThenInclude()`** - Navigate deeper into the include hierarchy
- **`.AndInclude()`** - Create sibling includes at the current nesting level
- **`.Parent()`** - Navigate back up the hierarchy

## Navigation Hierarchy

```
Root (Specification<T>)
  ├─ .Include()       → Root-level include (creates IncludableSpecificationBuilder)
  │   ├─ .ThenInclude() → Navigate deeper
  │   │   ├─ .ThenInclude() → Navigate even deeper
  │   │   └─ .AndInclude()  → Sibling at same level
  │   └─ .AndInclude()    → Sibling at same level
  └─ .Include()       → Another root-level include (sibling to first Include)
```

## API Usage Examples

### Example 1: Multiple Root-Level Includes

Chain multiple root-level includes without explicit `.Parent()` calls:

```csharp
var spec = new Specification<Application>();

spec
	.Include(app => app.Credentials)        // Root level
	.Include(app => app.AllowedScopes)      // Sibling at root level
	.ThenInclude(scope => scope.Resource);  // Nested under AllowedScopes
```

**Generated Include Paths:**
- `Application.Credentials`
- `Application.AllowedScopes.Resource`

### Example 2: Nested Includes with Siblings

Use `.AndInclude()` to create sibling includes at the same nesting level:

```csharp
var spec = new Specification<Application>();

spec
	.Include(app => app.AllowedScopes)         // Root level
	.ThenInclude(scope => scope.Resource)      // Nested: AllowedScopes → Resource
	.AndInclude(scope => scope.Owner)          // Sibling: AllowedScopes → Owner
	.ThenInclude(owner => owner.Department);   // Nested: AllowedScopes → Owner → Department
```

**Generated Include Paths:**
- `Application.AllowedScopes.Resource`
- `Application.AllowedScopes.Owner.Department`

### Example 3: Complex Hierarchical Includes

Combine all navigation methods for complex scenarios:

```csharp
var spec = new Specification<Application>();

spec
	.Include(app => app.Environment)              // Root: Environment
	.Include(app => app.AllowedScopes)            // Root: AllowedScopes (sibling to Environment)
	.ThenInclude(scope => scope.Resource)         // Nested: AllowedScopes → Resource
	.AndInclude(scope => scope.Owner)             // Sibling: AllowedScopes → Owner
	.ThenInclude(owner => owner.Department)       // Nested: AllowedScopes → Owner → Department
	.AndInclude(owner => owner.Region)            // Sibling: AllowedScopes → Owner → Region
	.Include(app => app.Credentials);             // Root: Credentials (new root-level include)
```

**Generated Include Paths:**
- `Application.Environment`
- `Application.AllowedScopes.Resource`
- `Application.AllowedScopes.Owner.Department`
- `Application.AllowedScopes.Owner.Region`
- `Application.Credentials`

### Example 4: Collection Navigation

Works seamlessly with collection properties:

```csharp
var spec = new Specification<User>();

spec
	.Include(user => user.Orders)                 // Collection at root
	.ThenInclude(order => order.Items)            // Collection nested in collection
	.ThenInclude(item => item.Product)            // Object nested in collection
	.AndInclude(item => item.Discount)            // Sibling: Items → Discount
	.AndInclude(order => order.ShippingAddress);  // Sibling: Orders → ShippingAddress
```

**Generated Include Paths:**
- `User.Orders.Items.Product`
- `User.Orders.Items.Discount`
- `User.Orders.ShippingAddress`

### Example 5: Using `.Parent()` for Explicit Navigation

When you need to navigate back up explicitly:

```csharp
var spec = new Specification<Application>();

spec
	.Include(app => app.AllowedScopes)
	.ThenInclude(scope => scope.Owner)
	.ThenInclude(owner => owner.Department)
	.Parent()                                  // Back to Owner level
	.AndInclude(owner => owner.Region)
	.Parent()                                  // Back to AllowedScopes level  
	.Parent()                                  // Back to Application (root) level
	.Include(app => app.Environment);         // New root-level include
```

## Method Semantics

### `.Include<TEntity, TProperty>()`
- **On Specification<T>**: Creates a root-level include
- **On IncludableSpecificationBuilder**: Creates a new root-level sibling include
- **Returns**: `IncludableSpecificationBuilder<TEntity, TProperty>`

### `.ThenInclude<TPrevious, TNext>()`
- **Purpose**: Navigate deeper into the current include path
- **Extends**: The current builder's chain (modifies existing chain)
- **Returns**: `IncludableSpecificationBuilder<TEntity, TNext>` with extended chain

### `.AndInclude<TPrevious, TNext>()`
- **Purpose**: Create a sibling include at the same parent level
- **Creates**: A new separate include chain starting from the parent
- **Returns**: `IncludableSpecificationBuilder<TEntity, TNext>` for the new sibling

### `.Parent()`
- **Purpose**: Return to the underlying specification for further operations
- **Returns**: `Specification<TEntity>`
- **Use case**: When you need to call other specification methods (Where, OrderBy, etc.)

## Implementation Notes

### How Include Chains Work Internally

1. Each `IncludeExpression` holds a **reference** to a list of lambda expressions
2. When `.ThenInclude()` is called, it adds to the **existing** chain (modifying the list)
3. When `.Include()` or `.AndInclude()` is called, it creates a **new** chain and registers it
4. The evaluator processes all registered chains when the query executes

### Parent Chain Tracking

Each `IncludableSpecificationBuilder` maintains:
- **`IncludeChain`**: The full chain of expressions for this include path
- **`ParentChain`**: The chain of expressions up to the parent level (used by `.AndInclude()`)

Example:
```
Include(A)              → IncludeChain: [A],      ParentChain: []
  .ThenInclude(B)       → IncludeChain: [A, B],   ParentChain: [A]
	.AndInclude(C)      → IncludeChain: [A, C],   ParentChain: [A]
	  .ThenInclude(D)   → IncludeChain: [A, C, D], ParentChain: [A, C]
```

## Migration from Previous API

### Before (required explicit `.Parent()`)
```csharp
spec
	.Include(app => app.Credentials)
	.Parent()                                  // Required!
	.Include(app => app.AllowedScopes)
	.ThenInclude(scope => scope.Resource);
```

### After (fluent sibling chaining)
```csharp
spec
	.Include(app => app.Credentials)
	.Include(app => app.AllowedScopes)        // Direct chaining
	.ThenInclude(scope => scope.Resource);
```

Both styles are still supported - `.Parent()` remains available for explicit control.

## Type Safety and Collection Unwrapping

The API automatically unwraps `ICollection<T>` types to enable proper `ThenInclude` chaining:

```csharp
// ICollection<TItem> is automatically unwrapped to TItem
spec
	.Include(user => user.Orders)             // ICollection<Order> → Order
	.ThenInclude(order => order.Items)        // ICollection<OrderItem> → OrderItem
	.ThenInclude(item => item.Product);       // Product
```

This is handled by specialized overloads for `ICollection<T>` navigation properties.

## Best Practices

1. **Use `.Include()` for root-level siblings** - More readable than `.Parent().Include()`
2. **Use `.AndInclude()` for same-level siblings** - Clearer intent than navigating up and down
3. **Use `.ThenInclude()` for deeper nesting** - Standard Entity Framework semantics
4. **Use `.Parent()` sparingly** - Only when you need explicit control or to call other spec methods
5. **Chain operations fluently** - Take advantage of the builder pattern for readability

## Comparison with Entity Framework Core

This API mirrors and extends EF Core's `Include/ThenInclude` pattern:

| EF Core | Arcturus Specification | Purpose |
|---------|----------------------|---------|
| `.Include()` | `.Include()` | Root-level include |
| `.ThenInclude()` | `.ThenInclude()` | Navigate deeper |
| N/A | `.AndInclude()` | Sibling at same level |
| N/A | `.Parent()` | Navigate back to spec |

The Arcturus Specification pattern adds `.AndInclude()` and `.Parent()` for more flexible navigation.
