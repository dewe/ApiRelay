using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MvcWebRole.Controllers
{
    public interface IHttpClient
    {
        Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption);
    }
}