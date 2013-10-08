using System.Net.Http;
using System.Web.Http.WebHost;

namespace MvcWebRole.Configuration
{
   public class NoBufferPolicy : WebHostBufferPolicySelector
    {
        public override bool UseBufferedInputStream(object hostContext)
        {
            return false;
        }

        public override bool UseBufferedOutputStream(HttpResponseMessage response)
        {
            return false;
        }
    }
}