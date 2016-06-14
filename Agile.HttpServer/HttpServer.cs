using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.HttpServer
{
    public class HttpServer
    {
        public static Request ParseRequest(string str)
        {
            var lines = str.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            var firstline = lines[0];
            var parts = firstline.Split(' ');
            var request = new Request
            {
                HttpVersion = parts[2],
                Method = parts[0],
                RawUrl = parts[1],
                Params = new Dictionary<string, string>()
            };

            var blankline = 0;
            for (var i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                if (String.IsNullOrEmpty(line))
                {
                    blankline = i;
                    break;
                }

                var pairs = line.Split(':');
                var key = pairs[0];
                if (request.Params.ContainsKey(key))
                {
                    request.Params[key] = pairs[1];
                }
                else
                {
                    request.Params.Add(key, pairs[1]);
                }
            }

            request.PostData = lines[lines.Length - 1];
            return request;
        }

        public static string BuildResponse(Response response)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0} {1} {2}\r\n", response.HttpVersion, response.Status, response.Description);
            if (response.Params != null)
            {
                foreach (var key in response.Params.Keys)
                {
                    sb.AppendFormat("{0}:{1}\r\n", key, response.Params[key]);
                }
            }
            sb.AppendFormat("\r\n");
            sb.AppendFormat("{0}", response.Body);
            return sb.ToString();
        }

        public static ActionResult View(string viewPath)
        {
            return new ViewResult
            {
                ContentType = (int)ContentTypeEnum.Html,
                ViewPath = viewPath
            };
        }

        public static ActionResult Json(object data)
        {
            return new JsonResult
            {
                ContentType = (int)ContentTypeEnum.Json,
                Data = data
            };
        }

        public static ActionResult Content(string str)
        {
            return new ContentResult
            {
                ContentType = (int)ContentTypeEnum.Text,
                Content = str
            };
        }
    }
}
