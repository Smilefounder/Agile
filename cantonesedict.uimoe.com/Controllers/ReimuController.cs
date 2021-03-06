﻿using Agile.API.Dtos;
using Agile.API.Helpers;
using Agile.Attributes;
using Agile.Dtos;
using Agile.Helpers;
using Agile.Static;
using Agile.Web.Helpers;
using cantonesedict.uimoe.com.Helpers;
using cantonesedict.uimoe.com.ViewModels.Reimu;
using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace cantonesedict.uimoe.com.Controllers
{
    [FreeAccess]
    public class ReimuController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Statistics()
        {
            var vm = new StatisticsVM
            {
                now = DateTime.Now.ToString("yyyy-MM-dd")
            };

            try
            {
                var list = LogicHelper.H10067();
                foreach (var item in list)
                {
                    if (item.IName == "todayquerycount")
                    {
                        vm.todayquerycount = Convert.ToInt32(item.ICount);
                        continue;
                    }

                    if (item.IName == "querycount")
                    {
                        vm.querycount = Convert.ToInt32(item.ICount);
                        continue;
                    }

                    if (item.IName == "noresultcount")
                    {
                        vm.noresultcount = Convert.ToInt32(item.ICount);
                        continue;
                    }

                    if (item.IName == "wordcount")
                    {
                        vm.wordcount = Convert.ToInt32(item.ICount);
                        continue;
                    }

                    if (item.IName == "termcount")
                    {
                        vm.termcount = Convert.ToInt32(item.ICount);
                        continue;
                    }

                    if (item.IName == "sentencecount")
                    {
                        vm.sentencecount = Convert.ToInt32(item.ICount);
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return View(vm);
        }

        #region 词汇
        public ActionResult Vocabulary()
        {
            ViewBag.len = Request.Params["len"];
            return View();
        }

        public ActionResult Vocabulary_pl()
        {
            var vm = new H10068Response
            {
                error = 0,
                data = new PagedListDto<H10068ResponseListItem>()
            };

            try
            {
                var request = WebHelper.ParseFromRequest<H10068Request>();
                vm = LogicHelper.H10068(request);
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return View(vm);
        }

        [HttpGet]
        public ActionResult AddVocabulary()
        {
            ViewBag.chntext = Request.Params["chntext"];
            return View();
        }

        [HttpPost]
        public ActionResult AddVocabulary(H10070Request request)
        {
            var response = new HBaseResponse
            {
                error = 1,
                message = "操作失败，请稍后再试"
            };

            try
            {
                var rows = LogicHelper.H10070(request);
                if (rows > 0)
                {
                    response = new HBaseResponse
                    {
                        error = 0
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return Json(response);
        }

        [HttpPost]
        public ActionResult QueryVocabulary()
        {
            var item = default(GroupItemDto);

            try
            {
                var chntext = Request.Params["chntext"];
                var response = LogicHelper.H10088(chntext);
                if (response != null)
                {
                    item = new GroupItemDto
                    {
                        ICount = response.ICount,
                        IName = response.IName
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            if (item == null)
            {
                return Json(new { error = 1, message = "未找到相关结果" });
            }

            return Json(new { error = 0, data = item.IName });
        }

        [HttpPost]
        public ActionResult GetVocabularyRef(string chntext)
        {
            var response = new HBaseResponse
            {
                error = 1
            };

            try
            {
                var chntext2 = HttpUtility.UrlEncode(chntext, Encoding.GetEncoding("gbk")).ToUpper();
                var str = CantoneseHelper.GetFromWeb(chntext2);
                if (!string.IsNullOrEmpty(str))
                {
                    response = new HBaseResponse
                    {
                        error = 0,
                        message = str
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return Json(response);
        }

        [HttpGet]
        public ActionResult UpdateVocabulary()
        {
            var model = new H10071ResponseListItem();
            var id = 0;
            int.TryParse(Request.Params["id"], out id);

            try
            {
                var response = LogicHelper.H10071(id);
                if (response != null &&
                    response.data != null)
                {
                    model = new H10071ResponseListItem
                    {
                        canpronounce = response.data.canpronounce,
                        cantext = response.data.cantext,
                        canvoice = response.data.canvoice,
                        chntext = response.data.chntext,
                        createdat = response.data.createdat,
                        id = response.data.id
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateVocabulary(H10071ResponseListItem model)
        {
            var response = new HBaseResponse
            {
                error = 1,
                message = "操作失败，请稍后再试"
            };

            try
            {
                var rows = LogicHelper.H10072(model);
                if (rows > 0)
                {
                    response = new HBaseResponse
                    {
                        error = 0
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return Json(response);
        }

        [HttpPost]
        public ActionResult DeleteVocabulary()
        {
            var response = new HBaseResponse
            {
                error = 1,
                message = "操作失败，请稍后再试"
            };

            var id = 0;
            int.TryParse(Request.Params["id"], out id);

            try
            {
                var rows = LogicHelper.H10069(id);
                if (rows > 0)
                {
                    response = new HBaseResponse
                    {
                        error = 0
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return Json(response);
        }
        #endregion

        #region 情景
        public ActionResult Category()
        {
            return View();
        }

        public ActionResult Category_pl()
        {
            var vm = new H10073Response
            {
                error = 0,
                data = new PagedListDto<H10073ResponseListItem>()
            };

            try
            {
                var request = WebHelper.ParseFromRequest<H10073Request>();
                vm = LogicHelper.H10073(request);
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return View(vm);
        }

        [HttpGet]
        public ActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddCategory(H10075Request request)
        {
            var response = new HBaseResponse
            {
                error = 1,
                message = "操作失败，请稍后再试"
            };

            try
            {
                var rows = LogicHelper.H10075(request);
                if (rows > 0)
                {
                    response = new HBaseResponse
                    {
                        error = 0
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return Json(response);
        }

        [HttpGet]
        public ActionResult UpdateCategory()
        {
            var model = new H10076ResponseListItem();
            var id = 0;
            int.TryParse(Request.Params["id"], out id);

            try
            {
                var response = LogicHelper.H10076(id);
                if (response != null &&
                    response.data != null)
                {
                    model = new H10076ResponseListItem
                    {
                        name = response.data.name,
                        createdat = response.data.createdat,
                        id = response.data.id
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateCategory(H10076ResponseListItem model)
        {
            var response = new HBaseResponse
            {
                error = 1,
                message = "操作失败，请稍后再试"
            };

            try
            {
                var rows = LogicHelper.H10077(model);
                if (rows > 0)
                {
                    response = new HBaseResponse
                    {
                        error = 0
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return Json(response);
        }

        [HttpPost]
        public ActionResult DeleteCategory()
        {
            var response = new HBaseResponse
            {
                error = 1,
                message = "操作失败，请稍后再试"
            };

            var id = 0;
            int.TryParse(Request.Params["id"], out id);

            try
            {
                var rows = LogicHelper.H10074(id);
                if (rows > 0)
                {
                    response = new HBaseResponse
                    {
                        error = 0
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return Json(response);
        }

        public ActionResult CategoryVocabulary()
        {
            int categoryid;
            int.TryParse(Request.Params["categoryid"], out categoryid);

            ViewBag.categoryid = categoryid;
            return View();
        }

        public ActionResult CategoryVocabulary_pl()
        {
            var vm = new H10084Response
            {
                error = 0,
                data = new PagedListDto<H10084ResponseListItem>()
            };

            try
            {
                var request = WebHelper.ParseFromRequest<H10084Request>();
                vm = LogicHelper.H10084(request);
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return View(vm);
        }

        [HttpGet]
        public ActionResult AddCategoryVocabulary()
        {
            int categoryid;
            int.TryParse(Request.Params["categoryid"], out categoryid);

            ViewBag.categoryid = categoryid;
            return View();
        }

        [HttpPost]
        public ActionResult AddCategoryVocabulary(int? categoryid, int? vocabularyid)
        {
            var response = new HBaseResponse
            {
                error = 1,
                message = "操作失败，请稍后再试"
            };

            try
            {
                var rows = LogicHelper.H10083(categoryid.GetValueOrDefault(), vocabularyid.GetValueOrDefault());
                if (rows > 0)
                {
                    response = new HBaseResponse
                    {
                        error = 0
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return Json(response);
        }

        [HttpGet]
        public ActionResult UpdateCategoryVocabulary()
        {
            var model = new H10084ResponseListItem();
            var id = 0;
            int.TryParse(Request.Params["id"], out id);

            try
            {
                var response = LogicHelper.H10085(id);
                if (response != null)
                {
                    model = new H10084ResponseListItem
                    {
                        chntext = response.chntext,
                        vocabularyid = response.vocabularyid,
                        id = response.id
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateCategoryVocabulary(int? vocabularyid, int? id)
        {
            var response = new HBaseResponse
            {
                error = 1,
                message = "操作失败，请稍后再试"
            };

            try
            {
                var rows = LogicHelper.H10086(vocabularyid.GetValueOrDefault(), id.GetValueOrDefault());
                if (rows > 0)
                {
                    response = new HBaseResponse
                    {
                        error = 0
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return Json(response);
        }

        [HttpPost]
        public ActionResult DeleteCategoryVocabulary()
        {
            var response = new HBaseResponse
            {
                error = 1,
                message = "操作失败，请稍后再试"
            };

            var id = 0;
            int.TryParse(Request.Params["id"], out id);

            try
            {
                var rows = LogicHelper.H10087(id);
                if (rows > 0)
                {
                    response = new HBaseResponse
                    {
                        error = 0
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return Json(response);
        }
        #endregion

        #region 用户
        public ActionResult UserList()
        {
            return View();
        }

        public ActionResult UserList_pl()
        {
            var vm = new H10078Response
            {
                error = 0,
                data = new PagedListDto<H10078ResponseListItem>()
            };

            try
            {
                var request = WebHelper.ParseFromRequest<H10078Request>();
                request.domain = (int)H10044RequestDomainEnum.cantonesedict;
                vm = LogicHelper.H10078(request);
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return View(vm);
        }
        #endregion

        public ActionResult Feedback()
        {
            return View();
        }

        public ActionResult Feedback_pl()
        {
            var vm = new H10079Response
            {
                error = 0,
                data = new PagedListDto<H10079ResponseListItem>()
            };

            try
            {
                var request = WebHelper.ParseFromRequest<H10079Request>();
                vm = LogicHelper.H10079(request);
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult DeleteFeedback()
        {
            var response = new HBaseResponse
            {
                error = 1,
                message = "操作失败，请稍后再试"
            };

            var id = 0;
            int.TryParse(Request.Params["id"], out id);

            try
            {
                var rows = LogicHelper.H10081(id);
                if (rows > 0)
                {
                    response = new HBaseResponse
                    {
                        error = 0
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return Json(response);
        }

        public ActionResult NoResult()
        {
            return View();
        }

        public ActionResult NoResult_pl()
        {
            var vm = new H10080Response
            {
                error = 0,
                data = new PagedListDto<H10080ResponseListItem>()
            };

            try
            {
                var request = WebHelper.ParseFromRequest<H10080Request>();
                vm = LogicHelper.H10080(request);
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return View(vm);
        }
        [HttpPost]
        public ActionResult DeleteNoResult()
        {
            var response = new HBaseResponse
            {
                error = 1,
                message = "操作失败，请稍后再试"
            };

            var id = 0;
            int.TryParse(Request.Params["id"], out id);

            try
            {
                var rows = LogicHelper.H10082(id);
                if (rows > 0)
                {
                    response = new HBaseResponse
                    {
                        error = 0
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return Json(response);
        }


        [HttpPost]
        public ActionResult ClearVocabulary()
        {
            var response = HBaseResponse.Faild;

            try
            {
                var rows = LogicHelper.H10089();
                if (rows > 0)
                {
                    response = new HBaseResponse
                    {
                        error = 0,
                        message = string.Format("已清理{0}条重复数据", rows)
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return Json(response);
        }

        [HttpPost]
        public ActionResult ClearNoResult()
        {
            var response = HBaseResponse.Faild;

            try
            {
                var rows = LogicHelper.H10090();
                if (rows > 0)
                {
                    response = new HBaseResponse
                    {
                        error = 0,
                        message = string.Format("已清理{0}条重复数据", rows)
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return Json(response);
        }

        public ActionResult Query()
        {
            int day;
            int.TryParse(Request.Params["day"], out day);

            ViewBag.day = day;
            return View();
        }

        public ActionResult Query_pl()
        {
            var vm = new H10080Response
            {
                error = 0,
                data = new PagedListDto<H10080ResponseListItem>()
            };

            try
            {
                var request = WebHelper.ParseFromRequest<H10080Request>();
                vm = LogicHelper.H10095(request);
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult DeleteQuery()
        {
            var response = new HBaseResponse
            {
                error = 1,
                message = "操作失败，请稍后再试"
            };

            var id = 0;
            int.TryParse(Request.Params["id"], out id);

            try
            {
                var rows = LogicHelper.H10094(id);
                if (rows > 0)
                {
                    response = new HBaseResponse
                    {
                        error = 0
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return Json(response);
        }
    }
}
