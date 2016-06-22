using Agile.Dtos.API;
using Agile.Helpers;
using Agile.Helpers.API;
using mp.uimoe.com.Helpers;
using mp.uimoe.com.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace mp.uimoe.com.Controllers
{
    public class HahaController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var request = ReflectHelper.ParseFromRequest<MP_AuthorizeRequest>();
            //var response = WeixinHelper.CheckWeixinMessage(request);
            return Content(request.echostr);
        }

        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            var timestamp = DateTimeHelper.GetTimeStamp();
            var response = new MP_TextResponse
            {
                Content = "欢迎关注，发送任意文本给公众号，将会回应一个随机笑话~ ",
                CreateTime = timestamp > int.MaxValue ? int.MaxValue : Convert.ToInt32(timestamp)
            };

            try
            {
                var xml = "";
                var stream = HttpContext.Request.InputStream;
                using (var reader = new StreamReader(stream))
                {
                    xml = reader.ReadToEnd();
                }

                var requestbase = SerializeHelper.ParseFromXml<MP_RequestBase>(xml);
                if (requestbase == null)
                {
                    return Content(WeixinHelper.BuildContentResponse(response));
                }

                response.FromUserName = requestbase.ToUserName;
                response.ToUserName = requestbase.FromUserName;
                response.MsgId = response.CreateTime;

                var msgType = requestbase.MsgType.ToLower();
                if (msgType == "event")
                {
                    var eventrequestbase = requestbase as MP_EventRequestBase;
                    if (eventrequestbase == null)
                    {
                        return Content(WeixinHelper.BuildContentResponse(response));
                    }

                    var eventstr = eventrequestbase.Event.ToLower();
                    if (eventstr == "subscribe")
                    {
                        response.Content = "欢迎关注，发送任意文本给公众号，将会回应一个随机笑话~ ";
                        return Content(WeixinHelper.BuildContentResponse(response));
                    }

                    if (eventstr == "unsubscribe")
                    {
                        response.Content = "主人不要我了T_T";
                        return Content(WeixinHelper.BuildContentResponse(response));
                    }
                }

                if (msgType == "text")
                {
                    HandleTextRequest(xml, response);
                    return Content(WeixinHelper.BuildContentResponse(response));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return Content(WeixinHelper.BuildContentResponse(response));
        }

        private void HandleTextRequest(string xml, MP_TextResponse response)
        {
            response.Content = "未找到相关结果";

            var textrequest = SerializeHelper.ParseFromXml<MP_TextRequest>(xml);
            if (textrequest == null)
            {
                return;
            }

            try
            {
                var list = LogicHelper.H10059(new H10059Request
                {
                    take = 1
                });

                if (list != null &&
                    list.error == 0 &&
                    list.data != null &&
                    list.data.Any())
                {
                    var item = list.data[0];
                    response.Content = item.content;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }
        }
    }
}
