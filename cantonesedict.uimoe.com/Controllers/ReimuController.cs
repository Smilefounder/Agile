using Agile.API.Dtos;
using Agile.API.Helpers;
using Agile.Attributes;
using Agile.Dtos;
using Agile.Helpers;
using Agile.Web.Helpers;
using cantonesedict.uimoe.com.ViewModels.Reimu;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    if (item.IName == "feedbackcount")
                    {
                        vm.feedbackcount = Convert.ToInt32(item.ICount);
                        continue;
                    }

                    if (item.IName == "noresultcount")
                    {
                        vm.noresultcount = Convert.ToInt32(item.ICount);
                        continue;
                    }

                    if (item.IName == "termcount")
                    {
                        vm.termcount = Convert.ToInt32(item.ICount);
                        continue;
                    }

                    if (item.IName == "usercount")
                    {
                        vm.usercount = Convert.ToInt32(item.ICount);
                        continue;
                    }

                    if (item.IName == "wordcount")
                    {
                        vm.wordcount = Convert.ToInt32(item.ICount);
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return View(vm);
        }

        #region 词汇
        public ActionResult Vocabulary()
        {
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
                LogHelper.Write(ex.ToString());
            }

            return View(vm);
        }

        [HttpGet]
        public ActionResult AddVocabulary()
        {
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
                LogHelper.Write(ex.ToString());
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
                LogHelper.Write(ex.ToString());
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
                LogHelper.Write(ex.ToString());
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
                LogHelper.Write(ex.ToString());
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
                LogHelper.Write(ex.ToString());
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
                LogHelper.Write(ex.ToString());
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
                LogHelper.Write(ex.ToString());
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
                LogHelper.Write(ex.ToString());
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
                LogHelper.Write(ex.ToString());
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
                LogHelper.Write(ex.ToString());
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
                LogHelper.Write(ex.ToString());
            }

            return View(vm);
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
                LogHelper.Write(ex.ToString());
            }

            return View(vm);
        }
    }
}
