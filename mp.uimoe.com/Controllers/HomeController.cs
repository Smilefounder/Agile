﻿using Agile.API.Dtos;
using Agile.Helpers;
using Agile.API.Helpers;
using mp.uimoe.com.Helpers;
using mp.uimoe.com.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using Agile.Web.Helpers;
using System.Collections.Generic;

namespace mp.uimoe.com.Controllers
{
    public class HomeController : Controller
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
                Content = "Hi~",
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

                xml = xml.Replace("<![CDATA[", "");
                xml = xml.Replace("]]>", "");

                var requestbase = SerializeHelper.ParseFromXml<MP_RequestBase>(xml);
                if (requestbase == null)
                {
                    return Content(WeixinHelper.BuildContentResponse(response));
                }

                response.FromUserName = requestbase.ToUserName;
                response.ToUserName = requestbase.FromUserName;

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
                        response.Content = "欢迎关注粤语词典，给我发送文字，我会返回其粤语注音给你。更多功能还在开发中，谢谢支持...";
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
            response.Content = "暂不支持的输入";

            var sb = new StringBuilder();
            var textrequest = SerializeHelper.ParseFromXml<MP_TextRequest>(xml);
            if (textrequest == null)
            {
                return;
            }

            var textresponsebase = LogicHelper.H10014(new H10014Request
            {
                input = textrequest.Content
            });

            var textresponse = textresponsebase as H10014Response;
            if (textresponse == null ||
                textresponse.groups == null ||
                textresponse.groups.Count == 0)
            {
                sb.AppendFormat("未找到相关记录\r\n");
                sb.AppendFormat("--------------------\r\n");
                sb.AppendFormat("<a href='http://cantonesedict.uimoe.com/home/feedback?chntext={0}'>点这里反馈</a>\r\n", Server.UrlEncode(textrequest.Content));
                response.Content = sb.ToString();

                ThreadPool.QueueUserWorkItem(new WaitCallback(SaveNoResultUseThread), textrequest.Content);
                return;
            }

            foreach (var g in textresponse.groups)
            {
                var plist = g.items.Select(o => o.canpronounce);
                sb.AppendFormat("{0}:{1}\r\n", g.chntext, string.Join(",", plist));
            }

            sb.AppendFormat("--------------------\r\n");
            sb.AppendFormat("<a href='http://cantonesedict.uimoe.com/Home/Index?input={0}'>点这里查看发音</a>\r\n", Server.UrlEncode(textrequest.Content));
            response.Content = sb.ToString();

            if (textresponse.noresult != null && textresponse.noresult.Any())
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(SaveNoResultUseThread), textresponse.noresult);
            }
        }

        private void SaveNoResultUseThread(object state)
        {
            try
            {
                var chntext = state as string;
                LogicHelper.H10066(chntext);
            }
            catch
            { }
        }
    }
}
