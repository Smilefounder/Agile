using Agile.API.Dtos;
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
using Agile.Dtos;

namespace mp.uimoe.com.Controllers
{
    public class HomeController : Controller
    {
        private const string mpName = "CantoneseDictionary";

        private const string mpId = "gh_6d1dc1e90f30";

        private const string appId = "wx1e67f96f78645efc";

        private const string appSecret = "cc1c60cbfb37609e32fb136b37b34a0c";

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
                    var vr = new MP_VoiceResponse
                    {
                        FromUserName = requestbase.ToUserName,
                        ToUserName = requestbase.FromUserName,
                        MsgType = "voice",
                        CreateTime = timestamp > int.MaxValue ? int.MaxValue : Convert.ToInt32(timestamp)
                    };

                    HandleTextRequest(xml, response, vr);

                    if (!string.IsNullOrEmpty(vr.MediaId))
                    {
                        return Content(WeixinHelper.BuildContentResponse(vr));
                    }

                    return Content(WeixinHelper.BuildContentResponse(response));
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return Content(WeixinHelper.BuildContentResponse(response));
        }

        private void HandleTextRequest(string xml, MP_TextResponse response, MP_VoiceResponse vresponse)
        {
            response.Content = "暂不支持的输入";

            var sb = new StringBuilder();
            var textrequest = SerializeHelper.ParseFromXml<MP_TextRequest>(xml);
            if (textrequest == null)
            {
                return;
            }

            var isvoice = false;
            if (textrequest.Content.StartsWith("#") && textrequest.Content.Length == 2)
            {
                isvoice = true;
                textrequest.Content = textrequest.Content.Substring(1);
            }

            if (textrequest.Content.StartsWith("@"))
            {
                textrequest.Content = textrequest.Content.Substring(1);
                if (textrequest.Content.ToLower() == "uploadvoice")
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(UploadVoiceUseThread), null);
                    response.Content = "已启动更新全部发音素材任务";
                    return;
                }

                if (textrequest.Content.Length == 1)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(UploadSingleVoiceUseThread), textrequest.Content);
                    response.Content = "已启动更新单个发音素材任务";
                    return;
                }
            }

            var textresponsebase = LogicHelper.H10014(new H10014Request
            {
                input = textrequest.Content
            });

            //保存查询记录
            ThreadPool.QueueUserWorkItem(new WaitCallback(RecordQueryUseThread), textrequest.Content);

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

            if (textresponse.noresult != null && textresponse.noresult.Any())
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(SaveNoResultUseThread), textresponse.noresult);
            }

            if (isvoice)
            {
                if (textresponse.groups != null &&
                    textresponse.groups.Count > 0 &&
                    textresponse.groups[0].items != null &&
                    textresponse.groups[0].items.Count > 0)
                {
                    response.Content = "未找到发音文件";

                    var canpronounce = textresponse.groups[0].items[0].canpronounce;
                    if (!string.IsNullOrEmpty(canpronounce))
                    {
                        var folder = Server.MapPath("~/Content/voice");
                        var filepath = System.IO.Path.Combine(new string[] { folder, canpronounce.Trim() + ".wav" });
                        var mediaid = WeixinHelper.UploadTempFile(filepath, UploadFileTypeEnum.voice, appId, appSecret);
                        if (!string.IsNullOrEmpty(mediaid))
                        {
                            vresponse.MediaId = mediaid;
                        }
                    }

                    return;
                }
            }

            foreach (var g in textresponse.groups)
            {
                var plist = g.items.Select(o => o.canpronounce);
                var clist = g.items.Select(o => o.cantext);
                sb.AppendFormat("[普]{0} \r\n[粤]{1} \r\n[音]{2}\r\n", g.chntext, clist.FirstOrDefault(), string.Join(",", plist));
            }

            sb.AppendFormat("--------------------\r\n");
            sb.AppendFormat("<a href='http://cantonesedict.uimoe.com/Home/Index?input={0}'>点这里查看发音</a>\r\n", Server.UrlEncode(textrequest.Content));
            response.Content = sb.ToString();
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

        public void RecordQueryUseThread(object state)
        {
            var chnText = state as string;
            if (string.IsNullOrEmpty(chnText))
            {
                return;
            }

            try
            {
                LogicHelper.H10091(chnText);
            }
            catch
            {

            }
        }

        public void UploadVoiceUseThread(object state)
        {
            var words = new List<KeyValueDto>();
            try
            {
                words = LogicHelper.H10096();
            }
            catch
            { }

            UploadVoice(words.ToArray());
        }

        public void UploadSingleVoiceUseThread(object state)
        {
            var chntext = state as string;
            if (string.IsNullOrEmpty(chntext))
            {
                return;
            }

            var words = new List<KeyValueDto>();
            try
            {
                words = LogicHelper.H10098(chntext);
            }
            catch
            { }

            UploadVoice(words.ToArray());
        }

        public void UploadVoice(params KeyValueDto[] words)
        {
            var folder = Server.MapPath("~/Content/voice");
            foreach (var w in words)
            {
                var filepath = System.IO.Path.Combine(new string[] { folder, w.IValue.Trim() + ".wav" });
                if (!System.IO.File.Exists(filepath))
                {
                    continue;
                }

                try
                {
                    var mediaid = WeixinHelper.UploadFile(filepath, UploadFileTypeEnum.voice, appId, appSecret);
                    if (!string.IsNullOrEmpty(mediaid))
                    {
                        LogicHelper.H10097(Convert.ToInt32(w.IKey), mediaid);
                    }
                }
                catch
                {
                    continue;
                }
            }
        }
    }
}
