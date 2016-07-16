using Agile.Attributes;
using Agile.Dtos;
using Agile.API.Dtos;
using Agile.Helpers;
using Agile.API.Helpers;
using cantonesedict.uimoe.com.ViewModels;
using cantonesedict.uimoe.com.ViewModels.Home;
using cantonesedict.uimoe.com.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Agile.Web.Helpers;

namespace cantonesedict.uimoe.com.Controllers
{
    public class UserController : Controller
    {
        [LoginAccess]
        public ActionResult ScoreList()
        {
            var vm = new List<ScoreListItemVM>();
            var userinfo = Session["userinfo"] as UserInfoVM;
            if (userinfo == null)
            {
                return View(vm);
            }

            try
            {
                var request = WebHelper.ParseFromRequest<H10039Request>();
                request.userid = userinfo.UserId;

                var responsebase = LogicHelper.H10039(request);
                var response = responsebase as H10039Response;
                if (response != null &&
                    response.error == 0 &&
                    response.data != null &&
                    response.data.Count > 0)
                {
                    vm = response.data.Select(o => new ScoreListItemVM
                    {
                        createdat = o.createdat,
                        score = o.score,
                        way = o.way
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return View(vm);
        }

        [HttpGet]
        [LoginAccessAttribute]
        public ActionResult FeedbackList()
        {
            var vm = new FeedbackListVM
            {
                Data = new PagedListDto<FeedbackListItemVM>
                {
                    RecordList = new List<FeedbackListItemVM>()
                }
            };

            var userinfo = Session["userinfo"] as UserInfoVM;
            if (userinfo == null)
            {
                return View(vm);
            }

            try
            {
                var responsebase = LogicHelper.H10022(new H10022Request
                {
                    username = userinfo.UserName
                });

                var response = responsebase as H10022Response;
                if (response != null &&
                    response.error == 0 &&
                    response.data != null &&
                    response.data.RecordList != null &&
                    response.data.RecordList.Any())
                {
                    vm.Data = new PagedListDto<FeedbackListItemVM>
                    {
                        Page = response.data.Page,
                        PageSize = response.data.PageSize,
                        RecordCount = response.data.RecordCount,
                        RecordList = response.data.RecordList.Select(o => new FeedbackListItemVM
                        {
                            CanText = o.cantext,
                            ChnText = o.chntext,
                            CreatedAt = o.createdat,
                            Status = o.status
                        }).ToList()
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return View(vm);
        }

        [HttpGet]
        [LoginAccessAttribute]
        public ActionResult PlanList()
        {
            var response = new H10061Response
            {
                error = 0,
                data = new PagedListDto<H10061ResponseListItem>
                {
                    RecordList = new List<H10061ResponseListItem>()
                }
            };

            var userinfo = Session["userinfo"] as UserInfoVM;
            if (userinfo == null)
            {
                return View(response);
            }

            try
            {
                var h10061request = WebHelper.ParseFromRequest<H10061Request>();
                h10061request.userid = userinfo.UserId;

                var h10061response = LogicHelper.H10061(h10061request);
                if (h10061response != null &&
                    h10061response.error == 0 &&
                    h10061response.data != null &&
                    h10061response.data.RecordList != null &&
                    h10061response.data.RecordList.Any())
                {
                    response.data = new PagedListDto<H10061ResponseListItem>
                    {
                        Page = h10061response.data.Page,
                        PageSize = h10061response.data.PageSize,
                        RecordCount = h10061response.data.RecordCount,
                        RecordList = h10061response.data.RecordList
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return View(response);
        }

        [HttpGet]
        [LoginAccess]
        public ActionResult DoPlan()
        {
            ViewBag.sceneid = Request.Params["sceneid"];

            var vm = new DoPlanVM();
            var userinfo = Session["userinfo"] as UserInfoVM;
            if (userinfo == null)
            {
                return View(vm);
            }

            try
            {
                var h10062request = WebHelper.ParseFromRequest<H10062Request>();
                h10062request.userid = userinfo.UserId;

                var h10062response = LogicHelper.H10062(h10062request);
                if (h10062response != null && h10062response.data != null)
                {
                    vm = new DoPlanVM
                    {
                        CanPronounce = h10062response.data.canpronounce,
                        CanText = h10062response.data.cantext,
                        ChnText = h10062response.data.chntext,
                        Finished = h10062response.data.finished > 0,
                        VocabularyId= h10062response.data.id
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return View(vm);
        }

        [HttpPost]
        [LoginAccess]
        public ActionResult DoPlan(int? sceneid, int? vocabularyid)
        {
            var response = new H10063Response
            {
                error = 1,
                message = "操作失败，请稍后重试"
            };

            var userinfo = Session["userinfo"] as UserInfoVM;
            if (userinfo == null)
            {
                response.message = "请先登录";
                return Json(response);
            }

            try
            {
                var h10063response = LogicHelper.H10063(new H10063Request
                {
                    sceneid = sceneid,
                    userid = userinfo.UserId,
                    vocabularyid = vocabularyid
                });

                if (h10063response != null)
                {
                    response = new H10063Response
                    {
                        error = h10063response.error,
                        message = h10063response.message
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return Json(response);
        }

        [HttpGet]
        public ActionResult UserList()
        {
            var vm = new UserListVM
            {
                data = new PagedListDto<UserListItemVM>
                {
                    RecordList = new List<UserListItemVM>()
                }
            };

            try
            {
                var responsebase = LogicHelper.H10047(WebHelper.ParseFromRequest<H10047Request>());
                var response = responsebase as H10047Response;
                if (response != null &&
                    response.error == 0 &&
                    response.data != null &&
                    response.data.RecordList != null &&
                    response.data.RecordList.Any())
                {
                    vm.data = new PagedListDto<UserListItemVM>
                    {
                        Page = response.data.Page,
                        PageSize = response.data.PageSize,
                        RecordCount = response.data.RecordCount,
                        RecordList = response.data.RecordList.Select(o => new UserListItemVM
                        {
                            Status = o.status,
                            UserName = o.username
                        }).ToList()
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return View(vm);
        }

        public ActionResult VocabularyList()
        {
            var vm = new VocabularyListVM
            {
                data = new SkipTakeListDto<VocabularyListItemVM>
                {
                    RecordList = new List<VocabularyListItemVM>()
                }
            };

            try
            {
                var responsebase = LogicHelper.H10018(WebHelper.ParseFromRequest<H10018Request>());
                var response = responsebase as H10018Response;
                if (response != null &&
                    response.error == 0 &&
                    response.data != null &&
                    response.data.RecordList != null &&
                    response.data.RecordList.Any())
                {
                    vm.data = new SkipTakeListDto<VocabularyListItemVM>
                    {
                        Skip = response.data.Skip,
                        Take = response.data.Take,
                        RecordList = response.data.RecordList.Select(o => new VocabularyListItemVM
                        {
                            CanPronounce = o.canpronounce,
                            CanText = o.cantext,
                            CanVoice = o.canvoice,
                            ChnText = o.chntext
                        }).ToList()
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return View(vm);
        }

        public ActionResult VisitList()
        {
            var vm = new VisitListVM
            {
                data = new PagedListDto<VisitListItemVM>
                {
                    RecordList = new List<VisitListItemVM>()
                }
            };

            try
            {
                var responsebase = LogicHelper.H10045(WebHelper.ParseFromRequest<H10045Request>());
                var response = responsebase as H10045Response;
                if (response != null &&
                    response.error == 0 &&
                    response.data != null &&
                    response.data.RecordList != null &&
                    response.data.RecordList.Any())
                {
                    vm.data = new PagedListDto<VisitListItemVM>
                    {
                        Page = response.data.Page,
                        PageSize = response.data.PageSize,
                        RecordCount = response.data.RecordCount,
                        RecordList = response.data.RecordList.Select(o => new VisitListItemVM
                        {
                            createdatstr = o.createdat.HasValue ? o.createdat.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                            ipaddress = o.ipaddress,
                            rawurl = o.rawurl,
                            useragent = o.useragent
                        }).ToList()
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult PassUser(string username)
        {
            try
            {
                LogicHelper.H10048(new H10048Request
                {
                    username = username
                });
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
                return Json(new { error = 1, message = ex.Message });
            }

            return Json(new { error = 0 });
        }

        [HttpPost]
        public ActionResult PassFeedback(string chntext)
        {
            try
            {
                LogicHelper.H10050(new H10050Request
                {
                    chntext = chntext
                });
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
                return Json(new { error = 1, message = ex.Message });
            }

            return Json(new { error = 0 });
        }

        [HttpPost]
        public ActionResult ForbidUser(string username)
        {
            try
            {
                LogicHelper.H10049(new H10049Request
                {
                    username = username
                });
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
                return Json(new { error = 1, message = ex.Message });
            }

            return Json(new { error = 0 });
        }

        public ActionResult FeedbackListAll()
        {
            var vm = new FeedbackListVM
            {
                Data = new PagedListDto<FeedbackListItemVM>
                {
                    RecordList = new List<FeedbackListItemVM>()
                }
            };

            try
            {
                var responsebase = LogicHelper.H10022(new H10022Request());
                var response = responsebase as H10022Response;
                if (response != null &&
                    response.error == 0 &&
                    response.data != null &&
                    response.data.RecordList != null &&
                    response.data.RecordList.Any())
                {
                    vm.Data = new PagedListDto<FeedbackListItemVM>
                    {
                        Page = response.data.Page,
                        PageSize = response.data.PageSize,
                        RecordCount = response.data.RecordCount,
                        RecordList = response.data.RecordList.Select(o => new FeedbackListItemVM
                        {
                            CanText = o.cantext,
                            ChnText = o.chntext,
                            CreatedAt = o.createdat,
                            Status = o.status
                        }).ToList()
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return View(vm);
        }

        public ActionResult NoResultList()
        {
            var response = new H10053Response
            {
                error = 0,
                data = new PagedListDto<H10053ResponseListItem>
                {
                    RecordList = new List<H10053ResponseListItem>()
                }
            };

            try
            {
                var h10053request = WebHelper.ParseFromRequest<H10053Request>();
                var h10053response = LogicHelper.H10053(h10053request);
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return View(response);
        }

        [HttpPost]
        public ActionResult DeleteNoResult(string chntext)
        {
            try
            {
                LogicHelper.H10054(new H10054Request
                {
                    chntext = chntext
                });
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
                return Json(new { error = 1, message = ex.Message });
            }

            return Json(new { error = 0 });
        }

        public ActionResult PermissionList()
        {
            var response = new H10044Response
            {
                error = 0,
                data = new List<H10044ResponseListItem>()
            };

            var userinfo = Session["userinfo"] as UserInfoVM;
            if (userinfo == null)
            {
                return View(response);
            }

            try
            {
                var h10044request = new H10044Request
                {
                    domain = (int)H10044RequestDomainEnum.cantonesedict,
                    userid = userinfo.UserId
                };

                var responsebase = LogicHelper.H10044(h10044request);
                var h10044response = responsebase as H10044Response;
                if (h10044response != null &&
                    h10044response.error == 0 &&
                    h10044response.data != null &&
                    h10044response.data.Any())
                {
                    response.data = h10044response.data.Select(o => new H10044ResponseListItem
                    {
                        domain = o.domain,
                        hasmenu = o.hasmenu,
                        name = o.name,
                        rawurl = o.name
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return View(response);
        }

        public ActionResult DeletePermission(int? permissionid)
        {
            try
            {
                LogicHelper.H10055(new H10055Request
                {
                    permissionid = permissionid
                });
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return Json(new { error = 0 });
        }

        public ActionResult DeleteUserPermission(int? permissionid)
        {
            var userinfo = Session["userinfo"] as UserInfoVM;
            if (userinfo == null)
            {
                return Json(new { error = 1, message = "请先登录" });
            }

            try
            {
                LogicHelper.H10055(new H10055Request
                {
                    permissionid = permissionid,
                    userid = userinfo.UserId
                });
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return Json(new { error = 0 });
        }

        [HttpGet]
        public ActionResult AddPermission()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddPermission(string name, string rawurl)
        {
            try
            {
                LogicHelper.H10057(new H10057Request
                {
                    domain = (int)H10044RequestDomainEnum.cantonesedict,
                    name = name,
                    rawurl = rawurl
                });
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
                return Json(new { error = 1, message = ex.Message });
            }

            return Json(new { error = 0 });
        }
    }
}
