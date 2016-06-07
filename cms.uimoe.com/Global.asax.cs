using Agile.Helpers;
using cms.uimoe.com.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace cms.uimoe.com
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var text = "";
            var path = Server.MapPath("~/404.html");
            var rawUrl = HttpContext.Current.Request.RawUrl;

            try
            {
                var page = CoreHelper.GetPage(rawUrl);
                if (page != null)
                {
                    path = Server.MapPath("~/Htmls/" + page.FileName);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
                text = System.IO.File.ReadAllText(path);
            }

            Response.Write(text);
            Response.End();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}