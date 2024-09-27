using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Arcturus.AspNetCore.Endpoints;

public static class MvcOptionsExtensions
{
    public static MvcOptions UseEndpointsConventions(this MvcOptions options)
    {
        options.Conventions.Add(
            new EndpointsApplicationModelConvention(
                new RoutePrefixConvention()));
        return options;
    }

    private sealed class EndpointsApplicationModelConvention : IApplicationModelConvention
    {
        private readonly IControllerModelConvention _controllerModelConvention;

        public EndpointsApplicationModelConvention(IControllerModelConvention controllerConvention)
        {
            ArgumentNullException.ThrowIfNull(controllerConvention);

            _controllerModelConvention = controllerConvention;
        }

        public void Apply(ApplicationModel application)
        {
            ArgumentNullException.ThrowIfNull(application);

            var controllers = application.Controllers.ToArray();
            foreach (var controller in controllers)
            {
                _controllerModelConvention.Apply(controller);
            }
        }
    }
}


