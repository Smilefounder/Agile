using Agile.Helpers;
using os.uimoe.com.Dtos;
using os.uimoe.com.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace os.uimoe.com
{
    /// <summary>
    /// ajax 的摘要说明
    /// </summary>
    public class ajax : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var contentType = "";
            var content = "";
            var action = context.Request.Params["action"];

            try
            {
                switch (action)
                {
                    default:
                        {
                            contentType = "text/plain";
                            content = "";
                            break;
                        }
                    case "x10000":
                        {
                            var x10000request = ReflectHelper.ParseFromRequest<X10000Request>();
                            var x10000response = CoreHelper.X10000(x10000request);
                            content = x10000response.text;
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            context.Response.ContentType = contentType;
            context.Response.Write(content);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}