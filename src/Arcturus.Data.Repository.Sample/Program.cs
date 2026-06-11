using Arcturus.Repository.Specification;

/// <summary>
/// Demonstrates the fixed Include/ThenInclude/AndInclude chaining functionality.
/// This example shows how to properly chain multiple includes without the previous limitation.
/// </summary>
public class SpecificationIncludeChainingExamples
{
    // Sample entity classes for demonstration
    public class Application
    {
        public int Id { get; set; }
        public ICollection<Credential> Credentials { get; set; } = [];
        public ICollection<AllowedScope> AllowedScopes { get; set; } = [];
        public Environment? Environment { get; set; }
    }

    public class Credential
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
    }

    public class AllowedScope
    {
        public int Id { get; set; }
        public Resource? Resource { get; set; }
        public Owner? Owner { get; set; }
        public Scope? Scope { get; set; }
    }

    public class Resource
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class Scope
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class Owner
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Department? Department { get; set; }
        public Region? Region { get; set; }
    }

    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class Region
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class Environment
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public static Specification<Application> Example1_MultipleRootIncludes()
    {
        var appSpecification = new Specification<Application>();

        return (Specification<Application>)appSpecification
            .Include(_ => _.Credentials)
            .Include(_ => _.AllowedScopes)
                .ThenInclude(_ => _.Scope)
                .AndInclude(_ => _.Owner)
                    .ThenInclude(_ => _.Department);
    }
    public static Specification<Application> Example2_ObjectThenCollection()
    {
        var appSpecification = new Specification<Application>();

        // This now works! Previously the second Include would break the chain.
        return appSpecification
            .Include(app => app.Environment)                // Object at root
            .Include(app => app.AllowedScopes)              // Collection at root (sibling)
                .ThenInclude(scope => scope.Resource)
            .Specification;           // Object nested under AllowedScopes
    }
}


//    /// <summary>
//    /// Example 2: The second original bug scenario
//    /// BEFORE: Include after Include didn't preserve context
//    /// AFTER: Works as expected
//    /// </summary>
//    public static Specification<Application> Example2_ObjectThenCollection()
//    {
//        var appSpecification = new Specification<Application>();

//        // This now works! Previously the second Include would break the chain.
//        return appSpecification
//            .Include(app => app.Environment)                // Object at root
//            .Include(app => app.AllowedScopes)              // Collection at root (sibling)
//                .ThenInclude(scope => scope.Resource);           // Object nested under AllowedScopes

//        // Generated paths:
//        // - Application.Environment
//        // - Application.AllowedScopes.Resource
//    }

//    /// <summary>
//    /// Example 3: Using AndInclude for sibling navigation at nested levels
//    /// This is the new functionality that wasn't possible before
//    /// </summary>
//    public static Specification<Application> Example3_SiblingIncludes()
//    {
//        var appSpecification = new Specification<Application>();

//        return appSpecification
//            .Include(app => app.AllowedScopes)              // Collection at root
//                .ThenInclude(scope => scope.Resource)           // Navigate to Resource
//                .AndInclude<Application, Resource, AllowedScope, Owner>(scope => scope.Owner)  // Sibling to Resource (both under AllowedScopes)
//            .ThenInclude(owner => owner.Department)         // Navigate deeper from Owner
//            .Parent();                                      // Return to specification

//        // Generated paths:
//        // - Application.AllowedScopes.Resource
//        // - Application.AllowedScopes.Owner.Department
//    }

//    /// <summary>
//    /// Example 4: Complex scenario combining all navigation methods
//    /// Demonstrates fluent chaining with proper type handling
//    /// </summary>
//    public static Specification<Application> Example4_ComplexHierarchy()
//    {
//        return new Specification<Application>()
//            .Include(app => app.Environment)
//            .Include(app => app.AllowedScopes)
//            .Include(app => app.Credentials)
//            .Parent();

//        // Generated paths:
//        // - Application.Environment
//        // - Application.AllowedScopes
//        // - Application.Credentials
//    }

//    /// <summary>
//    /// Example 5: The AndInclude type inference fix (Bug Fix Example)
//    /// BEFORE: This would fail with "Scope is not a property of ResourceData" compilation error
//    /// AFTER: Now works correctly with the new AndInclude overload that infers TParentItemType
//    /// </summary>
//    public static Specification<Application> Example5_AndIncludeTypeInferenceFix()
//    {
//        var appSpecification = new Specification<Application>();

//        // This scenario demonstrates the bug fix:
//        // After .ThenInclude(_ => _.Resource), the builder type is IncludableSpecificationBuilder<Application, ResourceData>
//        // But .AndInclude(_ => _.Scope) needs to operate on AllowedScope (the parent item type), not ResourceData
//        // The new overload correctly infers TParentItemType = AllowedScope from the lambda expression
//        return appSpecification
//            .Include(app => app.AllowedScopes)              // ICollection<AllowedScope> → AllowedScope
//                .ThenInclude(scope => scope.Resource)          // Navigate to Resource (builder becomes <Application, Resource>)
//                .AndInclude<Application, Resource, AllowedScope, Scope>(scope => scope.Scope)  // Sibling to Resource, both under AllowedScope
//            .Parent();                                      // The compiler now correctly infers this operates on AllowedScope!

//        // Generated paths:
//        // - Application.AllowedScopes.Resource
//        // - Application.AllowedScopes.Scope
//    }

//    /// <summary>
//    /// Example 6: Using .Parent() for explicit navigation when needed
//    /// Shows backwards compatibility with the existing Parent() method
//    /// </summary>
//    public static Specification<Application> Example6_ExplicitParentNavigation()
//    {
//        var appSpecification = new Specification<Application>();

//        return appSpecification
//            .Include(app => app.AllowedScopes)
//                .ThenInclude(scope => scope.Owner)
//                .ThenInclude(owner => owner.Department)
//            .Parent()                                       // Explicitly navigate back to specification
//            .Where(app => app.Id > 0)                       // Can now use other spec methods
//            .Include(app => app.Credentials)                // Add another root-level include
//            .Parent()                                       // Return to specification
//            .OrderBy(app => app.Id);                        // Continue with specification operations

//        // The .Parent() method is still useful when you need to:
//        // 1. Break out of the include chain to use other specification methods
//        // 2. Make the navigation explicit for code clarity
//        // 3. Return to the specification for further configuration
//    }

//    /// <summary>
//    /// Example 7: Migration example showing the improvement
//    /// </summary>
//    public static class BeforeAndAfter
//    {
//        // BEFORE: Required awkward workarounds or wouldn't compile
//        public static Specification<Application> Before_Workaround()
//        {
//            var spec = new Specification<Application>();

//            // Had to use .Parent() explicitly or create separate specifications
//            return spec
//                .Include(app => app.Credentials)
//                .Include(app => app.AllowedScopes)
//                    .ThenInclude(scope => scope.Resource)
//                .Parent();                                  // Also needed at the end
//        }

//        // AFTER: Clean, fluent API
//        public static Specification<Application> After_Fluent()
//        {
//            var spec = new Specification<Application>();

//            // No .Parent() needed for root-level siblings! Just works with Include chaining.
//            return spec
//                .Include(app => app.Credentials)
//                .Include(app => app.AllowedScopes)
//                    .ThenInclude(scope => scope.Resource)
//                    .AndInclude(scope => scope.Owner)
//                .Parent();                                  // Return to specification
//        }
//    }
//}