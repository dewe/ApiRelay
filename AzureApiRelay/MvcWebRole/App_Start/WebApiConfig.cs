using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MvcWebRole
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "Relay Default",
                routeTemplate: "{*path}",
                defaults: new { controller = "Relay", path = "/" }
            );

            // To disable tracing in your application, please comment out or remove the following line of code
            // For more information, refer to: http://www.asp.net/web-api
            config.EnableSystemDiagnosticsTracing();
        }
    }
}
