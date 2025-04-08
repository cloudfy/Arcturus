using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Text.RegularExpressions;

namespace Arcturus.AspNetCore.Endpoints;

public sealed class RoutePrefixConvention : IControllerModelConvention
{  
    /// <summary>
    /// Applies a route prefix convention to the controller if it inherits from <see cref="AbstractEndpoint"/>.
    /// Updates the route template for selectors with a template of "[controller]" to a formatted route
    /// based on the controller's name.
    /// </summary>
    /// <param name="controller">The <see cref="ControllerModel"/> to which the convention is applied.</param>
    public void Apply(ControllerModel controller)
    {
        if (ShouldApply(controller))
        {
            foreach (var selector in controller.Selectors)
            {
                if (selector.AttributeRouteModel != null &&
                    selector.AttributeRouteModel.Template != null &&
                    selector.AttributeRouteModel.Template.Equals("[controller]", StringComparison.InvariantCultureIgnoreCase))
                {
                    selector.AttributeRouteModel.Template = FormatRoute(controller.ControllerName);
                }
            }
        }
    }

    private static string FormatRoute(string input)
    {
        // Define the regex pattern: optional prefix, capture the main part, and optional "Endpoint" suffix
        var pattern = @"^(Get|List|Create|Post|Delete|Update|Patch)?(.*?)(Endpoint)?$";
        var match = Regex.Match(input, pattern, RegexOptions.IgnoreCase);

        if (match.Success)
        {
            // The main part is captured in the second group of the regex
            return match.Groups[2].Value;
        }

        // If the pattern doesn't match, return the original string
        return input;
    }
    /// <summary>
    /// Checks if the convention should be applied to the controller.
    /// </summary>
    /// <param name="controller"><see cref="ControllerModel"/> to validate.</param>
    /// <returns>True if the controller inherits from <see cref="AbstractEndpoint"/>.</returns>
    private static bool ShouldApply(ControllerModel controller)
    {
        return controller.ControllerType
            .IsSubclassOf(typeof(AbstractEndpoint));
    }
}
