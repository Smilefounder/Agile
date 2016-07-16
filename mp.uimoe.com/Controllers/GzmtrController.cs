using Agile.API.Dtos;
using Agile.Helpers;
using Agile.API.Helpers;
using mp.uimoe.com.Helpers;
using mp.uimoe.com.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Agile.Web.Helpers;

namespace mp.uimoe.com.Controllers
{
    public class GzmtrController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var request = WebHelper.ParseFromRequest<MP_AuthorizeRequest>();
            //var response = WeixinHelper.CheckWeixinMessage(request);
            return Content(request.echostr);
        }

        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            var timestamp = DateTimeHelper.GetTimeStamp();
            var response = new MP_TextResponse
            {
                Content = "欢迎关注，查询线路请发送数字标号（规划中的线路也可以），查询站点请发送站点名...",
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
                        response.Content = "欢迎关注，查询线路请发送数字标号（规划中的线路也可以），查询站点请发送站点名...";
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

            var sb = new StringBuilder();

            var islinetext = false;
            if (textrequest.Content.Contains("广佛") || textrequest.Content.ToUpper().Contains("APM"))
            {
                islinetext = true;
            }
            else
            {
                int num = StringHelper.GetNumberFromStr(textrequest.Content);
                if (num > 0)
                {
                    islinetext = true;
                }
            }

            if (islinetext)
            {
                var h10027responsebase = LogicHelper.H10027(new H10027Request
                {
                    name = textrequest.Content
                });

                var h10027response = h10027responsebase as H10027Response;
                if (h10027response != null && h10027response.line != null)
                {
                    sb.AppendFormat(h10027response.line.name + "\r\n");
                    sb.AppendFormat("-----------\r\n");
                    if (h10027response.line.startedat.HasValue)
                    {
                        sb.AppendFormat("开建于：{0}\r\n", h10027response.line.startedat.Value.ToString("yyyy-MM-dd"));
                    }

                    if (h10027response.line.completedat.HasValue)
                    {
                        sb.AppendFormat("完工于：{0}\r\n", h10027response.line.completedat.Value.ToString("yyyy-MM-dd"));
                    }

                    if (h10027response.line.openedat.HasValue)
                    {
                        sb.AppendFormat("运营从：{0}\r\n", h10027response.line.openedat.Value.ToString("yyyy-MM-dd"));
                    }

                    if (h10027response.stations != null && h10027response.stations.Count > 0)
                    {
                        sb.AppendFormat("-----------\r\n");
                        foreach (var station in h10027response.stations)
                        {
                            sb.AppendFormat("{0}", station.name);
                            if (station.status.GetValueOrDefault(0) == 1)
                            {
                                sb.AppendFormat("(未开通)");
                            }

                            sb.AppendFormat(", ");
                        }
                    }

                    response.Content = sb.ToString();
                }

                return;
            }

            var h10028responsebase = LogicHelper.H10028(new H10028Request
            {
                name = textrequest.Content
            });

            var h10028response = h10028responsebase as H10028Response;
            if (h10028response != null && h10028response.data != null && h10028response.data.Count > 0)
            {
                sb.AppendFormat("与【{0}】有关的站点\r\n", textrequest.Content);
                sb.AppendFormat("-----------\r\n");
                foreach (var station in h10028response.data)
                {
                    sb.AppendFormat("{0}({1})", station.stationname, station.linename);
                    if (station.status.GetValueOrDefault(0) == 1)
                    {
                        sb.AppendFormat("(未开通)");
                    }

                    sb.AppendFormat("\r\n");
                }

                response.Content = sb.ToString();
            }
        }
    }
}
