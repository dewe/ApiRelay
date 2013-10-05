using System.IO;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcWebRole.Controllers;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MvcWebRole.Tests.Controllers
{
    [TestClass]
    public class RelayControllerTest
    {
        [TestMethod]
        public void ShouldRelayGetStatus()
        {
            var client = A.Fake<IHttpClient>();
            SetupFakeClient(client);

            var controller = new RelayController(client);
            SetupControllerForTest(controller);
            
            var response = controller.GetRelay("").Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        private void SetupFakeClient(IHttpClient client)
        {
            var taskSource = new TaskCompletionSource<HttpResponseMessage>();
            taskSource.SetResult(new HttpResponseMessage(HttpStatusCode.OK));
            var completedTask = taskSource.Task;

            A.CallTo(() => 
                client.GetAsync(A<Uri>.Ignored, HttpCompletionOption.ResponseHeadersRead))
                .Returns(completedTask);
        }

        private void SetupControllerForTest(RelayController controller)
        {
            var config = new HttpConfiguration();
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/test/apirelay");
            var route = config.Routes.MapHttpRoute("DefaultApi", "test/{controller}");
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary {{"controller", "apirelay"}});

            controller.ControllerContext = new HttpControllerContext(config, routeData, request);
            controller.Request = request;
            controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
        }
    }
}
