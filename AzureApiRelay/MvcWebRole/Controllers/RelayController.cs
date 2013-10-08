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
        private static readonly Uri ServiceBaseUri = new Uri("https://galaxy");
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
            var relayRequest = new HttpRequestMessage();
            relayRequest.Method = Request.Method;
            relayRequest.RequestUri = new Uri(ServiceBaseUri, path);

            TryCopyHeader("Accept", relayRequest.Headers, Request.Headers);
            TryCopyHeader("Authorization", relayRequest.Headers, Request.Headers);

            using (var serviceResponse = await _client.SendAsync(relayRequest, HttpCompletionOption.ResponseHeadersRead))
            {
                return CreateResponse(serviceResponse);
            }
        }

        private void TryCopyHeader(string name, HttpHeaders toHeaders, HttpHeaders fromHeaders)
        {
            if (!fromHeaders.Contains(name)) 
                return;
            
            var values = fromHeaders.GetValues(name);
            toHeaders.Add(name, values);
        }

        private HttpResponseMessage CreateResponse(HttpResponseMessage serviceResponse)
        {
            var relayResponse = Request.CreateResponse(serviceResponse.StatusCode);

            TryCopyHeader("WWW-Authenticate", relayResponse.Headers, serviceResponse.Headers);

            relayResponse.Content = serviceResponse.Content;
            serviceResponse.Content = null;
            return relayResponse;
        }
    }
}