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
    public class MoneyController : Controller
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
                Content = "暂不支持的消息",
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
                        response.Content = "欢迎关注，给我发送消费金额（数字），我会帮你记账噢，更多功能还在开发中...";
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

            var textrequest = SerializeHelper.ParseFromXml<MP_TextRequest>(xml);
            if (textrequest == null)
            {
                return;
            }

            var sb = new StringBuilder();
            var ttype = default(H10024RequestTypeEnum?);
            if (textrequest.Content == "今天")
            {
                ttype = H10024RequestTypeEnum.Today;
            }

            if (textrequest.Content == "本月")
            {
                ttype = H10024RequestTypeEnum.Today;
            }

            if (textrequest.Content == "今年")
            {
                ttype = H10024RequestTypeEnum.Today;
            }

            if (textrequest.Content == "一共")
            {
                ttype = H10024RequestTypeEnum.Today;
            }

            if (ttype.HasValue)
            {
                var h10024responsebase = LogicHelper.H10024(new H10024Request
                {
                    ttype = (int)ttype.Value,
                    username = textrequest.FromUserName
                });

                var cost = default(decimal);
                var income = default(decimal);
                var h10024response = h10024responsebase as H10024Response;
                if (h10024response != null && h10024response.error == 0)
                {
                    cost = h10024response.cost;
                    income = h10024response.income;
                }

                sb.AppendFormat("今日\r\n");
                sb.AppendFormat("-----------\r\n");
                sb.AppendFormat("支出：{0}\r\n", Math.Abs(cost));
                sb.AppendFormat("收入：{0}\r\n", income);
                sb.AppendFormat("-----------\r\n");
                sb.AppendFormat("小结：{0}\r\n", cost + income);
                response.Content = sb.ToString();
                return;
            }


            decimal money;
            decimal moneytemp;
            decimal.TryParse(textrequest.Content, out moneytemp);
            if (moneytemp == 0)
            {
                response.Content = "请输入0以外的数字，数字前的符号：+表示收入，-表示支出，不输符号表示支出";
                return;
            }

            if (!textrequest.Content.StartsWith("+") &&
                !textrequest.Content.StartsWith("-"))
            {
                money = -Math.Abs(moneytemp);
            }
            else
            {
                money = moneytemp;
            }

            var h0023response = LogicHelper.H10023(new H10023Request
            {
                money = money,
                username = textrequest.FromUserName
            });

            if (h0023response == null)
            {
                response.Content = "操作失败，请稍后再试";
                return;
            }

            if (h0023response.error != 0)
            {
                response.Content = h0023response.message;
                return;
            }

            var appendstr = "";
            if (money > 0)
            {
                appendstr = "收入：" + money + "元";
            }
            else
            {
                appendstr = "  消费：" + Math.Abs(money) + "元";
            }

            response.Content = appendstr + "\r\n----------\r\n回复【今天】、【本月】、【今年】、【一共】查看账单统计";
        }
    }
}
