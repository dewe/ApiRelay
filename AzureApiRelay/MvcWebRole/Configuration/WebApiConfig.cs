using System.Web.Http;
using System.Web.Http.Hosting;

namespace MvcWebRole.Configuration
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

            // How to control buffering: http://www.strathweb.com/2012/09/dealing-with-large-files-in-asp-net-web-api/
            config.Services.Replace(typeof(IHostBufferPolicySelector), new NoBufferPolicy());
        }
    }
}
