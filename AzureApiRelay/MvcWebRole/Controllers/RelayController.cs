using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace MvcWebRole.Controllers
{
    public class RelayController : ApiController
    {
        private static readonly Uri ServiceBaseUri = new Uri("http://www.sunet.se");
        private readonly IHttpClient _client;

        public RelayController()
        {
            var httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(20) };
            _client = new HttpClientWrapper(httpClient);
        }

        public RelayController(IHttpClient httpClient)
        {
            _client = httpClient;
        }

        [HttpGet]
        public async Task<HttpResponseMessage> RelayGet(string path)
        {
            var request = new HttpRequestMessage();
            request.Method = Request.Method;
            request.RequestUri = new Uri(ServiceBaseUri, path);
            TryCopyHeader(request, "Accept");

            using (var serviceResponse = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
            {
                return CreateResponse(serviceResponse);
            }
        }

        private void TryCopyHeader(HttpRequestMessage request, string name)
        {
            if (Request.Headers.Contains(name))
            {
                IEnumerable<string> values = Request.Headers.GetValues(name);
                request.Headers.Add(name, values);
            }
        }

        private HttpResponseMessage CreateResponse(HttpResponseMessage serviceResponse)
        {
            var relayResponse = Request.CreateResponse(serviceResponse.StatusCode);
            relayResponse.Content = serviceResponse.Content;
            serviceResponse.Content = null;
            return relayResponse;
        }
    }
}