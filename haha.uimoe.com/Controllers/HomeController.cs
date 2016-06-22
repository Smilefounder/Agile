using Agile.Dtos;
using Agile.Dtos.API;
using Agile.Helpers;
using Agile.Helpers.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace haha.uimoe.com.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NewList()
        {
            return View();
        }

        public ActionResult GetHotList()
        {
            var rtype = (int)H10058RequestTypeEnum.ByGoodCount;
            return GetNewList(rtype);
        }

        public ActionResult GetNewList(int? rtype)
        {
            var response = new H10058Response
            {
                error = 0,
                data = new PagedListDto<H10058ResponseListItem>
                {
                    RecordList = new List<H10058ResponseListItem>()
                }
            };

            try
            {
                var rtype1 = rtype.GetValueOrDefault(0);
                var rtype2 = (int)H10058RequestTypeEnum.ByGoodCount;

                var h10058request = ReflectHelper.ParseFromRequest<H10058Request>();
                if (rtype1 == rtype2)
                {
                    h10058request.rtype = rtype2;
                }
                else
                {
                    h10058request.rtype = (int)H10058RequestTypeEnum.ByCreatedAt;
                }

                var h10058response = LogicHelper.H10058(h10058request);
                if (h10058response != null &&
                    h10058response.error == 0 &&
                    h10058response.data != null &&
                    h10058response.data.RecordList != null &&
                    h10058response.data.RecordList.Any())
                {
                    response.data = new PagedListDto<H10058ResponseListItem>
                    {
                        Page = h10058response.data.Page,
                        PageSize = h10058response.data.PageSize,
                        RecordCount = h10058response.data.RecordCount,
                        RecordList = h10058response.data.RecordList.Select(o => new H10058ResponseListItem
                        {
                            badcount = o.badcount,
                            commentcount = o.commentcount,
                            content = o.content,
                            goodcount = o.goodcount,
                            jokeid = o.jokeid,
                            pictureurl = o.pictureurl,
                            postedat = o.postedat,
                            postedby = o.postedby
                        }).ToList()
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return View("~/Views/Home/GetNewList.cshtml", response);
        }
    }
}