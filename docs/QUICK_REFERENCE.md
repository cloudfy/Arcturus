# Specification Include Chaining - Quick Reference

## Quick Syntax Guide

### Root-Level Sibling Includes (NEW!)
```csharp
spec
	.Include(x => x.PropertyA)
	.Include(x => x.PropertyB)  // ← Just chain it!
	.Include(x => x.PropertyC);
```

### Nested Includes (Same as Before)
```csharp
spec
	.Include(x => x.Parent)
	.ThenInclude(p => p.Child)
	.ThenInclude(c => c.GrandChild);
```

### Sibling Includes at Same Level (NEW!)
```csharp
spec
	.Include(x => x.Parent)
	.ThenInclude(p => p.ChildA)
	.AndInclude(p => p.ChildB);  // ← Sibling to ChildA
```

### Navigate Back to Specification
```csharp
spec
	.Include(x => x.Property)
	.Parent()                     // ← Returns Specification<T>
	.Where(x => x.Id > 0);
```

## Method Quick Reference

| Method | From | To | Purpose |
|--------|------|----|----|
| `.Include(x => x.Prop)` | `Specification<T>` | `IncludableBuilder` | Start root include |
| `.Include(x => x.Prop)` | `IncludableBuilder` | `IncludableBuilder` | Add root sibling |
| `.ThenInclude(x => x.Prop)` | `IncludableBuilder` | `IncludableBuilder` | Go deeper |
| `.AndInclude(x => x.Prop)` | `IncludableBuilder` | `IncludableBuilder` | Same-level sibling |
| `.Parent()` | `IncludableBuilder` | `Specification<T>` | Back to spec |

## Common Patterns

### Pattern 1: Multiple Root Properties
```csharp
.Include(x => x.Profile)
.Include(x => x.Settings)
.Include(x => x.Preferences)
```

### Pattern 2: One Root, Multiple Nested
```csharp
.Include(x => x.User)
.ThenInclude(u => u.Profile)
.AndInclude(u => u.Settings)
.AndInclude(u => u.Orders)
```

### Pattern 3: Deep with Siblings
```csharp
.Include(x => x.Order)
.ThenInclude(o => o.Items)
.ThenInclude(i => i.Product)
.AndInclude(i => i.Discount)
```

### Pattern 4: Complex Hierarchy
```csharp
.Include(x => x.Department)
.ThenInclude(d => d.Manager)
.AndInclude(d => d.Employees)
.ThenInclude(e => e.Skills)
.Include(x => x.Location)  // Back to root
```

## Collections vs Objects

Both work the same way:

```csharp
// Object property
.Include(x => x.Manager)
.ThenInclude(m => m.Department)

// Collection property (auto-unwraps)
.Include(x => x.Employees)
.ThenInclude(e => e.Department)  // e is Employee, not ICollection<Employee>
```

## Cheat Sheet

```
Root
  ├─ .Include(root.A)
  │   ├─ .Include(root.B)           ← Root sibling
  │   ├─ .ThenInclude(a.A1)         ← Go deeper
  │   │   ├─ .ThenInclude(a1.A11)   ← Go even deeper
  │   │   └─ .AndInclude(a1.A12)    ← Same-level sibling
  │   └─ .AndInclude(a.A2)          ← Same-level sibling
  │       └─ .ThenInclude(a2.A21)
  └─ .Parent()                       ← Back to Specification<T>
```

## Remember

✅ **DO**: Chain `.Include()` for root siblings
✅ **DO**: Use `.AndInclude()` for same-level siblings  
✅ **DO**: Use `.ThenInclude()` to go deeper
✅ **DON'T**: Need `.Parent()` between root includes (but you can if you want)

## Before & After

| Before (Workaround) | After (Fluent) |
|---------------------|----------------|
| `.Include(A).Parent().Include(B)` | `.Include(A).Include(B)` |
| Complex navigation needed | Just chain naturally |
| Had to think about context | Intuitive chaining |

---

**That's it! Just chain naturally and it works!** 🎉
