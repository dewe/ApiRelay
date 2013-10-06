using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MvcWebRole.Controllers
{
    public class HttpClientWrapper : IHttpClient
    {
        private readonly HttpClient _client;

        public HttpClientWrapper(HttpClient client)
        {
            _client = client;
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption)
        {
            return _client.SendAsync(request, completionOption);
        }
    }
}