using Agile.Attributes;
using Agile.Dtos;
using Agile.Dtos.API;
using Agile.Helpers;
using Agile.Helpers.API;
using cantonesedict.uimoe.com.ViewModels;
using cantonesedict.uimoe.com.ViewModels.Home;
using cantonesedict.uimoe.com.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace cantonesedict.uimoe.com.Controllers
{
    public class UserController : Controller
    {
        [LoginAccessAttribute]
        public ActionResult ScoreList()
        {
            var userid = default(int);
            var useridstr = Session["userid"] as string;
            if (int.TryParse(useridstr, out userid) == false)
            {
                return View(new List<ScoreListItemVM>());
            }

            try
            {
                var request = ReflectHelper.ParseFromRequest<H10039Request>();
                request.userid = userid;

                var responsebase = LogicHelper.H10039(request);
                var response = responsebase as H10039Response;
                if (response != null &&
                    response.error == 0 &&
                    response.data != null &&
                    response.data.Count > 0)
                {
                    var recordlist = response.data.Select(o => new ScoreListItemVM
                    {
                        createdat = o.createdat,
                        score = o.score,
                        way = o.way
                    }).ToList();

                    return View(recordlist);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return View(new List<ScoreListItemVM>());
        }

        [HttpGet]
        [LoginAccessAttribute]
        public ActionResult FeedbackList()
        {
            var vm = new FeedbackListVM();
            var userinfo = Session["userinfo"] as UserInfoVM;
            if (userinfo != null)
            {
                try
                {
                    var responsebase = LogicHelper.H10022(new H10022Request
                    {
                        username = userinfo.UserName
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
            }

            return View(vm);
        }

        [HttpGet]
        [LoginAccessAttribute]
        public ActionResult PlanList()
        {
            return View();
        }

        [HttpGet]
        public ActionResult UserList()
        {
            return View();
        }

        public ActionResult VocabularyList()
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

        public ActionResult VisitList()
        {
            var vm = new VisitListVM { };
            try
            {
                var responsebase = LogicHelper.H10045(ReflectHelper.ParseFromRequest<H10045Request>());
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
    }
}
