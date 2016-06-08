using Agile.Attributes;
using Agile.Dtos;
using Agile.Dtos.API;
using Agile.Helpers;
using Agile.Helpers.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace www.llyn23.com.Controllers
{
    [FreeAccess]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            int page;
            if (int.TryParse(Request.Params["page"], out page) == false)
            {
                page = 1;
            }

            int pagesize;
            if (int.TryParse(Request.Params["pagesize"], out pagesize) == false)
            {
                pagesize = 10;
            }

            var response = new H10005Response
            {
                data = new PagedListDto<H10005ResponseListItem>
                {
                    RecordList = new List<H10005ResponseListItem>()
                }
            };

            try
            {
                var h10005responsebase = LogicHelper.H10005(new H10005Request
                {
                    page = page,
                    pagesize = pagesize,
                    yearmonth = Request.Params["yearmonth"]
                });

                var h10005response = h10005responsebase as H10005Response;
                if (h10005response != null &&
                    h10005response.error == 0 &&
                    h10005response.data != null &&
                    h10005response.data.RecordList != null)
                {
                    response.data = new PagedListDto<H10005ResponseListItem>
                    {
                        Page = h10005response.data.Page,
                        PageSize = h10005response.data.PageSize,
                        RecordCount = h10005response.data.RecordCount,
                        RecordList = h10005response.data.RecordList
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            if (StringHelper.IsPhoneRequest(Request.UserAgent))
            {
                return View("~/Views/Home/Index_m.cshtml", response);
            }

            return View(response);
        }

        public ActionResult RecentArchiveList()
        {
            try
            {
                var response = LogicHelper.H10006(new H10006Request
                {
                    take = 10
                });

                return View(response);
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
                return View(new H10006Response
                {
                    data = new List<H10006ResponseListItem>()
                });
            }
        }

        public ActionResult ArchiveGroupList()
        {
            int page;
            if (int.TryParse(Request.Params["page"], out page) == false)
            {
                page = 1;
            }

            int pagesize;
            if (int.TryParse(Request.Params["pagesize"], out pagesize) == false)
            {
                pagesize = 10;
            }

            ViewData["page"] = page;
            ViewData["pagesize"] = pagesize;

            try
            {
                var response = LogicHelper.H10007(new H10007Request
                {
                    take = null
                });

                return View(response);
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
                return View(new H10007Response
                {
                    data = new List<H10007ResponseListItem>()
                });
            }
        }

        public ActionResult ArchiveDetail()
        {
            int id;
            int.TryParse(Request.Params["id"], out id);

            try
            {
                var response = LogicHelper.H10008(new H10008Request
                {
                    id = id
                });

                return View(response);
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
                return View(new H10008Response
                {
                    data = new H10008ResponseListItem()
                });
            }
        }

        [HttpGet]
        public ActionResult LeaveComment()
        {
            int id;
            int.TryParse(Request.Params["id"], out id);

            ViewBag.archiveid = id;
            return View();
        }

        [HttpPost]
        public ActionResult LeaveComment(H10011Request vm)
        {
            try
            {
                vm.ipaddress = Request.UserHostAddress;
                vm.useragent = Request.UserAgent;
                var resonsebase = LogicHelper.H10011(vm);
                return Json(resonsebase);
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
                return Json(new { error = 1, message = "操作失败，请稍后再试" });
            }
        }

        [HttpGet]
        public ActionResult RecentCommentList()
        {
            try
            {
                int id;
                int.TryParse(Request.Params["id"], out id);

                var request = new H10012Request
                {
                    archiveid = id
                };

                var resonsebase = LogicHelper.H10012(request);
                var response = resonsebase as H10012Response;
                if (response == null)
                {
                    response = new H10012Response();
                }

                return View(response);
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
                return View(new H10012Response
                {
                    data = new List<H10012ResponseListItem>()
                });
            }
        }
    }
}
