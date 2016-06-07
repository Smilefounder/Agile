using os.uimoe.com.Dtos;
using System.Net;

namespace os.uimoe.com.Helpers
{
    public class CoreHelper
    {
        public static X10000Response X10000(X10000Request request)
        {
            var text = new WebClient().DownloadString(request.xurl);
            return new X10000Response
            {
                error = 0,
                text = text
            };
        }
    }
}