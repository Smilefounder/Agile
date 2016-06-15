using Agile.Attributes;
using Agile.Cache;
using Agile.Dtos.API;
using Agile.Helpers;
using Agile.Helpers.API;
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
            var vm = new IndexVM
            {
                AllMatched = null,
                OneMatches = new List<IndexListItemVM>()
            };

            var input = Request.Params["input"];
            if (String.IsNullOrEmpty(input))
            {
                return View(vm);
            }

            try
            {
                input = Server.UrlDecode(input);

                vm.Input = input;
                var h10015responsebase = LogicHelper.H10015(new H10015Request
                {
                    input = input
                });

                var invokeh10016 = true;
                var h10015response = h10015responsebase as H10015Response;
                if (h10015response != null && h10015response.data != null && h10015response.data.Any())
                {
                    var allMatched = h10015response.data.FirstOrDefault();
                    if (allMatched != null)
                    {
                        //如果查单个字成功了
                        //就不需要再调用后面的H10016单个字查询接口了
                        if (input.Length == 1)
                        {
                            invokeh10016 = false;
                        }

                        vm.AllMatched = new IndexListItemVM
                        {
                            CanPronounce = allMatched.canpronounce,
                            CanText = allMatched.cantext,
                            CanVoice = allMatched.canvoice,
                            ChnText = allMatched.chntext
                        };
                    }
                }

                if (invokeh10016)
                {
                    var h10016responsebase = LogicHelper.H10016(new H10016Request
                    {
                        input = input
                    });

                    var h10016response = h10016responsebase as H10016Response;
                    if (h10016response != null)
                    {
                        vm.OneMatches = h10016response.data.Select(o => new IndexListItemVM
                        {
                            CanPronounce = o.canpronounce,
                            CanText = o.cantext,
                            CanVoice = o.canvoice,
                            ChnText = o.chntext
                        }).ToList();

                        var sb = "";
                        foreach (var ch in input)
                        {
                            var chstr = ch.ToString();
                            var cnt = vm.OneMatches.Count(w => w.ChnText == chstr);
                            if (cnt == 0)
                            {
                                sb += chstr;
                            }
                        }

                        ThreadPool.QueueUserWorkItem(new WaitCallback(SaveFeedbackUseThread), sb);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.Message);
            }

            if (vm.OneMatches == null)
            {
                vm.OneMatches = new List<IndexListItemVM>();
            }

            //查询获得积分
            var userid = default(int);
            var useridstr = Session["userid"] as string;
            int.TryParse(useridstr, out userid);
            if (userid > 0)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(MakeScoreUseThread), new H10040Request
                {
                    canrepeat = 1,
                    score = 1,
                    way = (int)ScoreListItemWayEnum.Query,
                    userid = userid
                });
            }

            return View(vm);
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
            int skip;
            if (int.TryParse(Request.Params["skip"], out skip) == false)
            {
                skip = 0;
            }

            int take;
            if (int.TryParse(Request.Params["take"], out take) == false)
            {
                take = 10;
            }

            var vm = new VocabularyVM
            {
                Skip = skip,
                Take = take,
                Data = new List<VocabularyListItemVM>()
            };

            try
            {
                var responsebase = LogicHelper.H10018(new H10018Request
                {
                    skip = skip,
                    take = take,
                    texttype = (int)H10018RequestTextTypeEnum.Character
                });

                var response = responsebase as H10018Response;
                if (response != null && response.data != null)
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
                LogHelper.Write(ex.ToString());
            }

            return View(vm);
        }

        public ActionResult CanWord()
        {
            return View();
        }

        public ActionResult Term()
        {
            int skip;
            if (int.TryParse(Request.Params["skip"], out skip) == false)
            {
                skip = 0;
            }

            int take;
            if (int.TryParse(Request.Params["take"], out take) == false)
            {
                take = 10;
            }

            var vm = new VocabularyVM
            {
                Skip = skip,
                Take = take,
                Data = new List<VocabularyListItemVM>()
            };

            try
            {
                var responsebase = LogicHelper.H10018(new H10018Request
                {
                    skip = skip,
                    take = take,
                    texttype = (int)H10018RequestTextTypeEnum.Term
                });

                var response = responsebase as H10018Response;
                if (response != null && response.data != null)
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
                LogHelper.Write(ex.ToString());
            }

            return View(vm);
        }

        public ActionResult CanTerm()
        {
            return View();
        }

        public ActionResult Scene()
        {
            try
            {
                var responsebase = LogicHelper.H10037(ReflectHelper.ParseFromRequest<H10037Request>());
                var response = responsebase as H10037Response;
                if (response != null && response.error == 0)
                {
                    return View(response.data);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return View(new List<H10037ResponseListItem>());
        }

        public ActionResult SceneVocabulary()
        {
            try
            {
                var responsebase = LogicHelper.H10038(ReflectHelper.ParseFromRequest<H10038Request>());
                var response = responsebase as H10038Response;
                if (response != null && response.error == 0)
                {
                    return View(response.data.RecordList);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
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
            var username = Session["username"] as string;
            var permissions = Session["permissions"] as List<UserPermissionVM>;
            var vm = new MeVM
            {
                UserName = username,
                PermissionList = permissions ?? new List<UserPermissionVM>()
            };

            return View(vm);
        }

        [HttpPost]
        public ActionResult MakeScore()
        {
            var userid = default(int);
            var useridstr = Session["userid"] as string;
            if (int.TryParse(useridstr, out userid) == false)
            {
                return Json(new { error = 1, message = "请先登录" });
            }

            var request = ReflectHelper.ParseFromRequest<H10040Request>();
            request.userid = userid;

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

        [HttpPost]
        public ActionResult ScoreSum()
        {
            var userid = default(int);
            var useridstr = Session["userid"] as string;
            if (int.TryParse(useridstr, out userid) == false)
            {
                return Json(new { error = 1 });
            }

            var score = 0;

            try
            {
                var responsebase = LogicHelper.H10041(new H10041Request
                {
                    userid = userid,
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
                LogHelper.Write(ex.ToString());
            }

            return Json(new { error = 0, score = score });
        }

        public ActionResult MobileOnly()
        {
            return View();
        }

        [HttpGet]
        public ActionResult FeedbackList()
        {
            var username = Session["username"] as string;
            var vm = new FeedbackListVM();
            try
            {
                var responsebase = LogicHelper.H10022(new H10022Request
                {
                    username = username
                });

                var response = responsebase as H10022Response;
                if (response != null)
                {
                    vm.Data = response.data.Select(o => new FeedbackListItemVM
                    {
                        CanText = o.cantext,
                        ChnText = o.chntext,
                        CreatedAt = o.createdat,
                        Status = o.status
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

            var username = Session["username"] as string;

            try
            {
                var h10017responsebase = LogicHelper.H10017(new H10017Request
                {
                    cantext = vm.CanText,
                    chntext = vm.ChnText,
                    createdby = username
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
                LogHelper.Write(ex.ToString());
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
                LogHelper.Write(ex.ToString());
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
                LogHelper.Write(ex.ToString());
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
                LogHelper.Write(ex.ToString());
            }

            return View(vm);
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
                var request = ReflectHelper.ParseFromRequest<H10009Request>();
                var responsebase = LogicHelper.H10009(request);
                var response = responsebase as H10009Response;
                if (response != null && response.error == 0)
                {
                    Session["username"] = request.username;
                    Session["userid"] = response.userid.ToString();

                    //登录成功获取权限
                    CachePermissionListAfterLogin(response.userid);

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
                LogHelper.Write(ex.ToString());
            }

            return Json(new { error = 1, message = "操作失败，请稍后再试" });
        }

        /// <summary>
        /// 登录成功获取权限
        /// </summary>
        private void CachePermissionListAfterLogin(int userid)
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
                Session["permissions"] = response.data.Select(o => new UserPermissionVM
                {
                    Name = o.name,
                    RawUrl = o.rawurl
                }).ToList();
            }
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
                var response = LogicHelper.H10010(ReflectHelper.ParseFromRequest<H10010Request>());
                return Json(response);
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return Json(new { error = 1, message = "操作失败，请稍后再试" });
        }
    }
}
