using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcWebRole.Controllers;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;

namespace MvcWebRole.Tests.Controllers
{
    [TestClass]
    public class RelayControllerTest
    {
        [TestMethod]
        public void Should_relay_get_request()
        {
            var client = A.Fake<IHttpClient>();

            var controller = new RelayController(client);
            var getRequest = new HttpRequestMessage(HttpMethod.Get, "http://localhost/");
            SetupControllerForTest(controller, getRequest);
            
            var response = controller.RelayGet("/").Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public void Should_forward_accept_header()
        {
            var client = A.Fake<IHttpClient>();

            var controller = new RelayController(client);
            SetupControllerForTest(controller, new HttpRequestMessage());

            var contentType = new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/x-test");
            controller.Request.Headers.Accept.Add(contentType);

            var response = controller.RelayGet("/").Result;

            A.CallTo(() => 
                client.SendAsync(
                    A<HttpRequestMessage>.That.Matches(r => r.Headers.Accept.Contains(contentType)),
                    A<HttpCompletionOption>.Ignored))
                .MustHaveHappened();
        }

        private void SetupFakeResponses(IHttpClient client, HttpResponseMessage response)
        {
            var taskSource = new TaskCompletionSource<HttpResponseMessage>();
            taskSource.SetResult(response);
            var completedTask = taskSource.Task;

            A.CallTo(() =>
                client.SendAsync(A<HttpRequestMessage>.Ignored, A<HttpCompletionOption>.Ignored))
                .Returns(completedTask);
        }

        private void SetupControllerForTest(RelayController controller, HttpRequestMessage request)
        {
            var config = new HttpConfiguration();
            var route = config.Routes.MapHttpRoute("DefaultApi", "test/{controller}");
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary {{"controller", "apirelay"}});

            controller.ControllerContext = new HttpControllerContext(config, routeData, request);
            controller.Request = request;
            controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
        }
    }
}
