using Agile.Helpers;
using Agile.Web.Helpers;
using sharp.uimoe.com.Dtos;
using sharp.uimoe.com.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sharp.uimoe.com
{
    /// <summary>
    /// ajax 的摘要说明
    /// </summary>
    public class ajax : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var action = context.Request.Params["action"];
            if (!String.IsNullOrEmpty(action))
            {
                action = action.ToLower();
            }

            var obj = default(object);
            switch (action)
            {
                case "select":
                    {
                        obj = CoreHelper.Select(WebHelper.ParseFromRequest<SelectDto>());
                    }
                    break;
                case "ul":
                    {
                        obj = CoreHelper.Ul(WebHelper.ParseFromRequest<UlDto>());
                    }
                    break;
                case "table":
                    {
                        obj = CoreHelper.Table(WebHelper.ParseFromRequest<TableDto>());
                    }
                    break;
            }

            var json = SerializeHelper.ToJson(obj);
            context.Response.ContentType = "text/plain";
            context.Response.Write(json);
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