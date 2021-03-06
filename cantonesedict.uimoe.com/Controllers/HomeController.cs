﻿using Agile.API.Dtos;
using Agile.API.Helpers;
using Agile.Attributes;
using Agile.Dtos;
using Agile.Helpers;
using Agile.Static;
using Agile.Web.Helpers;
using cantonesedict.uimoe.com.ViewModels;
using cantonesedict.uimoe.com.ViewModels.Home;
using cantonesedict.uimoe.com.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Mvc;

namespace cantonesedict.uimoe.com.Controllers
{
    [FreeAccess]
    [NewVisit]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.input = Request.Params["input"];
            return View();
        }

        public ActionResult GetIndex()
        {
            var response = new H10014Response
            {
                error = 0,
                noresult = new List<string>(),
                groups = new List<H10014ResponseGroupItem>()
            };

            var input = Request.Params["input"];
            ViewBag.input = input;

            if (string.IsNullOrEmpty(input))
            {
                return View(response);
            }

            try
            {
                input = Server.UrlDecode(input);
                var h10014responsebase = LogicHelper.H10014(new H10014Request
                {
                    input = input
                });

                var h10014response = h10014responsebase as H10014Response;
                if (h10014response != null)
                {
                    if (h10014response.groups != null && h10014response.groups.Any())
                    {
                        foreach (var g in h10014response.groups)
                        {
                            response.groups.Add(new H10014ResponseGroupItem
                            {
                                rw = g.rw,
                                chntext = g.chntext,
                                items = g.items.Select(o => new H10014ResponseListItem
                                {
                                    canpronounce = o.canpronounce,
                                    cantext = o.cantext,
                                    canvoice = o.canvoice
                                }).ToList()
                            });
                        }
                    }

                    if (h10014response.noresult != null && h10014response.noresult.Any())
                    {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(SaveNoResultUseThread), h10014response.noresult);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.Message);
            }

            //查询获得积分
            var userinfo = Session["userinfo"] as UserInfoVM;
            if (userinfo != null)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(MakeScoreUseThread), new H10040Request
                {
                    canrepeat = 1,
                    score = 1,
                    way = (int)ScoreListItemWayEnum.Query,
                    userid = userinfo.UserId
                });
            }

            //保存查询记录
            ThreadPool.QueueUserWorkItem(new WaitCallback(RecordQueryUseThread), input);

            return View(response);
        }

        private void SaveNoResultUseThread(object state)
        {
            try
            {
                var words = state as List<string>;
                if (words == null || words.Count == 0)
                {
                    return;
                }

                LogicHelper.H10066(words.ToArray());
            }
            catch
            { }
        }

        private void SaveFeedbackUseThread(object state)
        {
            var input = state as string;
            if (String.IsNullOrEmpty(input))
            {
                return;
            }
            try
            {

                LogicHelper.H10017(new H10017Request
                {
                    chntext = input,
                    createdby = "reimu"
                });
            }
            catch
            {

            }
        }

        public ActionResult Vocabulary()
        {
            return View();
        }

        public ActionResult Word()
        {
            var vm = new VocabularyVM();

            try
            {
                var responsebase = LogicHelper.H10046(new H10046Request
                {
                    take = 10,
                    texttype = (int)H10018RequestTextTypeEnum.Character
                });

                var response = responsebase as H10046Response;
                if (response != null &&
                    response.error == 0 &&
                    response.data != null &&
                    response.data.Any())
                {
                    vm.Data = response.data.Select(o => new VocabularyListItemVM
                    {
                        CanPronounce = o.canpronounce,
                        CanText = o.cantext,
                        CanVoice = o.canvoice,
                        ChnText = o.chntext
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            if (vm.Data == null)
            {
                vm.Data = new List<VocabularyListItemVM>();
            }

            return View(vm);
        }

        public ActionResult CanWord()
        {
            return View();
        }

        public ActionResult GetCanWordList()
        {
            var response = new H10051Response
            {
                error = 0,
                data = new PagedListDto<H10051ResponseListItem>
                {
                    RecordList = new List<H10051ResponseListItem>()
                }
            };

            try
            {
                var h10051request = WebHelper.ParseFromRequest<H10051Request>();
                var h10051response = LogicHelper.H10051(h10051request);
                if (h10051response != null &&
                    h10051response.error == 0 &&
                    h10051response.data != null &&
                    h10051response.data.RecordList != null &&
                    h10051response.data.RecordList.Any())
                {
                    response.data = new PagedListDto<H10051ResponseListItem>
                    {
                        Page = h10051response.data.Page,
                        PageSize = h10051response.data.PageSize,
                        RecordCount = h10051response.data.RecordCount,
                        RecordList = h10051response.data.RecordList
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return View(response);
        }

        public ActionResult Term()
        {
            var vm = new VocabularyVM();

            try
            {
                var responsebase = LogicHelper.H10046(new H10046Request
                {
                    take = 10,
                    texttype = (int)H10018RequestTextTypeEnum.Term
                });

                var response = responsebase as H10046Response;
                if (response != null &&
                    response.error == 0 &&
                    response.data != null &&
                    response.data.Any())
                {
                    vm.Data = response.data.Select(o => new VocabularyListItemVM
                    {
                        CanPronounce = o.canpronounce,
                        CanText = o.cantext,
                        CanVoice = o.canvoice,
                        ChnText = o.chntext
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            if (vm.Data == null)
            {
                vm.Data = new List<VocabularyListItemVM>();
            }

            return View(vm);
        }

        public ActionResult CanTerm()
        {
            return View();
        }

        public ActionResult GetCanTermList()
        {
            var response = new H10052Response
            {
                error = 0,
                data = new PagedListDto<H10052ResponseListItem>
                {
                    RecordList = new List<H10052ResponseListItem>()
                }
            };

            try
            {
                var responsebase = LogicHelper.H10046(new H10046Request
                {
                    take = 10,
                    texttype = (int)H10018RequestTextTypeEnum.Term
                });

                var h10046response = responsebase as H10046Response;
                if (h10046response != null &&
                    h10046response.error == 0 &&
                    h10046response.data != null &&
                    h10046response.data.Any())
                {
                    response.data = new PagedListDto<H10052ResponseListItem>
                    {
                        RecordList = h10046response.data.Select(o => new H10052ResponseListItem
                        {
                            canpronounce = o.canpronounce,
                            cantext = o.cantext,
                            description = o.chntext
                        }).ToList()
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return View(response);
        }

        public ActionResult Scene()
        {
            var vm = new List<H10037ResponseListItem>();

            try
            {
                var responsebase = LogicHelper.H10037(WebHelper.ParseFromRequest<H10037Request>());
                var response = responsebase as H10037Response;
                if (response != null && response.error == 0)
                {
                    vm = response.data;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return View(vm);
        }

        public ActionResult SceneVocabulary()
        {
            try
            {
                var responsebase = LogicHelper.H10038(WebHelper.ParseFromRequest<H10038Request>());
                var response = responsebase as H10038Response;
                if (response != null && response.error == 0)
                {
                    return View(response.data.RecordList);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return View(new List<H10038ResponseListItem>());
        }

        public ActionResult Initials()
        {
            return View();
        }

        /// <summary>
        /// 韵母
        /// </summary>
        /// <returns></returns>
        public ActionResult Finals()
        {
            return View();
        }

        /// <summary>
        /// 韵腹
        /// </summary>
        /// <returns></returns>
        public ActionResult FinalsM()
        {
            return View();
        }

        /// <summary>
        /// 韵尾
        /// </summary>
        /// <returns></returns>
        public ActionResult FinalsE()
        {
            return View();
        }

        /// <summary>
        /// 鼻音
        /// </summary>
        /// <returns></returns>
        public ActionResult FinalsN()
        {
            return View();
        }

        public ActionResult Tones()
        {
            return View();
        }

        public ActionResult Discovery()
        {
            return View();
        }

        public ActionResult Me()
        {
            var userinfo = new UserInfoVM
            {
                UserPermissions = new List<UserPermissionVM>()
            };

            var userinfo2 = Session["userinfo"] as UserInfoVM;
            if (userinfo2 != null)
            {
                userinfo.UserId = userinfo2.UserId;
                userinfo.UserName = userinfo2.UserName;

                if (userinfo2.UserPermissions != null &&
                    userinfo2.UserPermissions.Any())
                {
                    foreach (var permission in userinfo2.UserPermissions)
                    {
                        if (permission.HasMenu.GetValueOrDefault(0) > 0)
                        {
                            userinfo.UserPermissions.Add(permission);
                        }
                    }
                }
            }

            return View(userinfo);
        }

        [HttpPost]
        public ActionResult MakeScore()
        {
            var userinfo = Session["userinfo"] as UserInfoVM;
            if (userinfo == null)
            {
                return Json(new { error = 1, message = "请先登录" });
            }

            var request = WebHelper.ParseFromRequest<H10040Request>();
            request.userid = userinfo.UserId;

            ThreadPool.QueueUserWorkItem(new WaitCallback(MakeScoreUseThread), request);
            return Json(new { error = 0 });
        }

        public void MakeScoreUseThread(object state)
        {
            var request = state as H10040Request;
            if (request == null)
            {
                return;
            }

            try
            {
                LogicHelper.H10040(request);
            }
            catch
            {

            }
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

        [HttpPost]
        public ActionResult ScoreSum()
        {
            var userinfo = Session["userinfo"] as UserInfoVM;
            if (userinfo == null)
            {
                return Json(new { error = 1 });
            }

            var score = 0;

            try
            {
                var responsebase = LogicHelper.H10041(new H10041Request
                {
                    userid = userinfo.UserId,
                    type = (int)H10041RequestTypeEnum.Total
                });
                var response = responsebase as H10041Response;
                if (response != null && response.error == 0)
                {
                    score = response.score;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return Json(new { error = 0, score = score });
        }

        public ActionResult MobileOnly()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Feedback()
        {
            return View(new FeedbackVM
            {
                ChnText = Request.Params["chntext"]
            });
        }

        [HttpPost]
        public ActionResult Feedback(FeedbackVM vm)
        {
            var response = new HBaseResponse
            {
                error = 1,
                message = "操作失败，请稍后再试。"
            };

            var userinfo = Session["userinfo"] as UserInfoVM;
            if (userinfo != null)
            {

            }

            try
            {
                var h10017responsebase = LogicHelper.H10017(new H10017Request
                {
                    cantext = vm.CanText,
                    chntext = vm.ChnText,
                    createdby = userinfo.UserName
                });

                var h10017response = h10017responsebase as H10017Response;
                if (h10017response == null)
                {
                    return Json(response);
                }

                if (h10017response.error != 0)
                {
                    return Json(response);
                }

                return Json(new HBaseResponse
                {
                    error = 0,
                    message = "感谢您的反馈！"
                });
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
                return Json(response);
            }
        }

        public ActionResult CantoneseTest()
        {
            var vm = new CantoneseTestVM();
            try
            {
                var responsebase = LogicHelper.H10019(new H10019Request
                {
                    take = 20
                });

                var response = responsebase as H10019Response;
                if (response != null && response.items != null && response.options != null)
                {
                    vm.Items = new List<CantoneseTestItemVM>();
                    var groups = response.options.GroupBy(g => g.testitemid);
                    foreach (var gp in groups)
                    {
                        var item = response.items.Where(w => w.id == gp.Key.GetValueOrDefault(0)).FirstOrDefault();
                        if (item != null)
                        {
                            vm.Items.Add(new CantoneseTestItemVM
                            {
                                Id = item.id,
                                Title = item.title,
                                Options = gp.ToList().Select(o => new CantoneseTestItemOptionVM
                                {
                                    DisplayText = o.displaytext,
                                    InnerValue = o.innervalue.GetValueOrDefault(0)
                                }).ToList()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return View(vm);
        }

        public ActionResult TranslateLyric()
        {
            return View();
        }

        public ActionResult GetLyricList()
        {
            var title = Request.Params["title"];
            var artists = Request.Params["artists"];
            var url = String.Format("http://geci.me/api/lyric/{0}", title);
            if (!String.IsNullOrEmpty(artists))
            {
                url = String.Format("http://geci.me/api/lyric/{0}/{1}", title, artists);
            }

            try
            {
                var downloadstr = new WebClient().DownloadString(url);
                return Content(downloadstr);
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
                return Content("{\"count\":0,\"code\":0,\"result\":[]}");
            }
        }

        public ActionResult LyricResult()
        {
            var url = Request.Params["lrc"];
            var vm = new LyricResultVM
            {
                Title = Request.Params["title"],
                Artists = Request.Params["artists"],
                Lines = new List<LyricResultListItemVM>()
            };

            try
            {
                var idx = url.LastIndexOf("/");
                var filename = url.Substring(idx);

                var folder = Server.MapPath("~/Downoads/Lyrics");
                if (!System.IO.Directory.Exists(folder))
                {
                    System.IO.Directory.CreateDirectory(folder);
                }

                var fullname = Server.MapPath("~/Downoads/Lyrics/" + filename);
                if (!System.IO.File.Exists(fullname))
                {
                    new WebClient().DownloadFile(url, fullname);
                }

                var lines = System.IO.File.ReadAllLines(fullname);
                foreach (var line in lines)
                {
                    var replacedline = Regex.Replace(line, "[[].*[]]", "");
                    if (String.IsNullOrEmpty(replacedline))
                    {
                        continue;
                    }

                    vm.Lines.Add(new LyricResultListItemVM
                    {
                        CanPronounce = CantoneseDictionary.FindAll(replacedline),
                        ChnText = replacedline
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return View(vm);
        }

        [HttpGet]
        public ActionResult NotAllowed()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            try
            {
                var request = WebHelper.ParseFromRequest<H10009Request>();
                var responsebase = LogicHelper.H10009(request);
                var response = responsebase as H10009Response;
                if (response != null && response.error == 0)
                {
                    //登录成功获取权限
                    CacheUserInfoAfterLogin(response.userid, request.username);

                    //登录成功获得积分
                    ThreadPool.QueueUserWorkItem(new WaitCallback(MakeScoreUseThread), new H10040Request
                    {
                        canrepeat = 0,
                        score = 10,
                        way = (int)ScoreListItemWayEnum.Login,
                        userid = response.userid
                    });
                }
                return Json(response);
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return Json(new { error = 1, message = "操作失败，请稍后再试" });
        }

        /// <summary>
        /// 登录成功后缓存用户信息
        /// </summary>
        private void CacheUserInfoAfterLogin(int userid, string username)
        {
            var userinfo = new UserInfoVM
            {
                UserId = userid,
                UserName = username
            };

            try
            {
                var responsebase = LogicHelper.H10044(new H10044Request
                {
                    userid = userid,
                    domain = (int)H10044RequestDomainEnum.cantonesedict
                });

                var response = responsebase as H10044Response;
                if (response != null &&
                    response.error == 0 &&
                    response.data != null &&
                    response.data.Any())
                {
                    userinfo.UserPermissions = response.data.Select(o => new UserPermissionVM
                    {
                        Name = o.name,
                        RawUrl = o.rawurl,
                        HasMenu = o.hasmenu
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }


            if (userinfo.UserPermissions == null)
            {
                userinfo.UserPermissions = new List<UserPermissionVM>();
            }

            Session["userinfo"] = userinfo;
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(FormCollection collection)
        {
            try
            {
                var response = LogicHelper.H10010(WebHelper.ParseFromRequest<H10010Request>());
                return Json(response);
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return Json(new { error = 1, message = "操作失败，请稍后再试" });
        }

        public ActionResult GetHot()
        {
            var vm = new List<GroupItemDto>();

            try
            {
                vm = LogicHelper.H10093(10);
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return View(vm);
        }

        public ActionResult GetNew()
        {
            var vm = new List<H10018ResponseListItem>();

            try
            {
                vm = LogicHelper.H10092(10);
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return View(vm);
        }
    }
}
