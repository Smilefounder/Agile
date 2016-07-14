using Agile.Dtos;
using Agile.API.Dtos;
using Agile.Helpers;
using Agile.API.Helpers;
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

        public ActionResult GetNewList()
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
                var h10058request = ReflectHelper.ParseFromRequest<H10058Request>();
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
                            content = o.content,
                            jokeid = o.jokeid,
                            pictureurl = o.pictureurl
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