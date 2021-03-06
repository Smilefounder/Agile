﻿using Agile.Attributes;
using Agile.API.Dtos;
using Agile.Helpers;
using Agile.API.Helpers;
using api.uimoe.com.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Agile.Web.Helpers;
using Agile.Dtos;

namespace api.uimoe.com.Controllers
{
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

            var response = default(H10000Response);
            try
            {
                var responsebase = LogicHelper.H10000(new H10000Request
                {
                    page = page,
                    pagesize = pagesize
                });

                response = responsebase as H10000Response;
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            if (response == null)
            {
                response = new H10000Response
                {
                    error = 1,
                    message = "操作失败，请稍后再试"
                };
            }

            return View(response);
        }

        [HttpGet]
        public ActionResult CreateNewInterface()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateNewInterface(CreateNewInterfaceVM vm)
        {
            var response = default(H10002Response);
            try
            {
                var responsebase = LogicHelper.H10002(new H10002Request
                {
                    code = vm.Code,
                    description = vm.Description,
                    name = vm.Name
                });

                response = responsebase as H10002Response;
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            if (response == null)
            {
                response = new H10002Response
                {
                    error = 1,
                    message = "操作失败，请稍后再试"
                };
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult UpdateInterface()
        {
            int id;
            int.TryParse(Request.Params["id"], out id);

            try
            {
                var responsebase = LogicHelper.H10004(new H10004Request
                {
                    id = id
                });

                var response = responsebase as H10004Response;
                if (response == null || response.data == null || response.data.Count == 0)
                {
                    return View(new UpdateInterfaceVM());
                }

                var entity = response.data.FirstOrDefault();
                if (entity == null)
                {
                    entity = new H10004ResponseListItem();
                }

                return View(new UpdateInterfaceVM
                {
                    Code = entity.code,
                    Description = entity.description,
                    Id = entity.id,
                    Name = entity.name
                });
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
                return View(new UpdateInterfaceVM());
            }
        }

        [HttpPost]
        public ActionResult UpdateInterface(UpdateInterfaceVM vm)
        {
            var response = default(H10003Response);
            try
            {
                var responsebase = LogicHelper.H10003(new H10003Request
                {
                    id = vm.Id,
                    code = vm.Code,
                    description = vm.Description,
                    name = vm.Name
                });

                response = responsebase as H10003Response;
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            if (response == null)
            {
                response = new H10003Response
                {
                    error = 1,
                    message = "操作失败，请稍后再试"
                };
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteInterface(string code)
        {
            var response = default(H10001Response);
            try
            {
                var responsebase = LogicHelper.H10001(new H10001Request
                {
                    code = code
                });

                response = responsebase as H10001Response;
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            if (response == null)
            {
                response = new H10001Response
                {
                    error = 1,
                    message = "操作失败，请稍后再试"
                };
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [FreeAccess]
        public ActionResult InvokeInterface()
        {
            ViewBag.code = Request.Params["code"];
            ViewBag.requeststr = Request.Params["requeststr"];

            var h10000responsebase = LogicHelper.H10000(new H10000Request
            {
                page = 1,
                pagesize = 30
            });

            var vm = default(List<H10000ResponseListItem>);
            var response = h10000responsebase as H10000Response;
            if (response != null &&
                response.data != null &&
                response.data.RecordList != null)
            {
                vm = response.data.RecordList;
            }

            if (vm == null)
            {
                vm = new List<H10000ResponseListItem>();
            }

            return View(vm);
        }

        [FreeAccess]
        public ActionResult InterfaceInfo(string code)
        {
            var req = "";
            var rsp = "";
            switch (code)
            {
                case "H10014":
                    {
                        req = WebHelper.ToRequestStr<H10014Request>(null);
                        rsp = SerializeHelper.ToJson(new H10014Response
                        {
                            error = 0,
                            isallmatched = true,
                            noresult = new List<string>(),
                            message = "",
                            time = 0,
                            groups = new List<H10014ResponseGroupItem>
                            {
                                new H10014ResponseGroupItem
                                {
                                    chntext="",
                                    rw=0,
                                    items=new List<H10014ResponseListItem>
                                    {
                                       new H10014ResponseListItem
                                       {
                                           canpronounce="",
                                           cantext="",
                                           canvoice=""
                                       }
                                    }
                                }
                            }
                        });
                    }
                    break;
                case "H10037":
                    {
                        req = WebHelper.ToRequestStr<H10037Request>(null);
                        rsp = SerializeHelper.ToJson(new H10037Response
                        {
                            error = 0,
                            message = "",
                            time = 0,
                            data = new List<H10037ResponseListItem>
                            {
                               new H10037ResponseListItem
                               {
                                   id=0,
                                   name="",
                                   total=0
                               }
                            }
                        });
                    }
                    break;
                case "H10038":
                    {
                        req = WebHelper.ToRequestStr<H10038Request>(null);
                        rsp = SerializeHelper.ToJson(new H10038Response
                        {
                            error = 0,
                            message = "",
                            time = 0,
                            data = new PagedListDto<H10038ResponseListItem>
                            {
                                Page = 0,
                                PageSize = 0,
                                RecordCount = 0,
                                RecordList = new List<H10038ResponseListItem>
                                {
                                    new H10038ResponseListItem
                                    {
                                        canpronounce="",
                                        cantext="",
                                        chntext=""
                                    }
                                }
                            }
                        });
                    }
                    break;
            }
            return Json(new { error = 0, req = req, rsp = rsp });
        }

        public ActionResult HBase(string code)
        {
            var watch = new Stopwatch();
            watch.Start();

            var response = default(HBaseResponse);
            try
            {
                switch (code)
                {
                    case "H10000":
                        {
                            response = LogicHelper.H10000(WebHelper.ParseFromRequest<H10000Request>());
                            break;
                        }
                    case "H10001":
                        {
                            response = LogicHelper.H10001(WebHelper.ParseFromRequest<H10001Request>());
                            break;
                        }
                    case "H10002":
                        {
                            response = LogicHelper.H10002(WebHelper.ParseFromRequest<H10002Request>());
                            break;
                        }
                    case "H10003":
                        {
                            response = LogicHelper.H10003(WebHelper.ParseFromRequest<H10003Request>());
                            break;
                        }
                    case "H10004":
                        {
                            response = LogicHelper.H10004(WebHelper.ParseFromRequest<H10004Request>());
                            break;
                        }
                    case "H10005":
                        {
                            response = LogicHelper.H10005(WebHelper.ParseFromRequest<H10005Request>());
                            break;
                        }
                    case "H10006":
                        {
                            response = LogicHelper.H10006(WebHelper.ParseFromRequest<H10006Request>());
                            break;
                        }
                    case "H10007":
                        {
                            response = LogicHelper.H10007(WebHelper.ParseFromRequest<H10007Request>());
                            break;
                        }
                    case "H10008":
                        {
                            response = LogicHelper.H10008(WebHelper.ParseFromRequest<H10008Request>());
                            break;
                        }
                    case "H10009":
                        {
                            var h10009request = WebHelper.ParseFromRequest<H10009Request>();
                            response = LogicHelper.H10009(h10009request);
                            if (response.error == 0)
                            {
                                Session["logeduser"] = h10009request.username;
                            }
                            break;
                        }
                    case "H10010":
                        {
                            response = LogicHelper.H10010(WebHelper.ParseFromRequest<H10010Request>());
                            break;
                        }
                    case "H10011":
                        {
                            response = LogicHelper.H10011(WebHelper.ParseFromRequest<H10011Request>());
                            break;
                        }
                    case "H10012":
                        {
                            response = LogicHelper.H10012(WebHelper.ParseFromRequest<H10012Request>());
                            break;
                        }
                    case "H10013":
                        {
                            response = LogicHelper.H10013(WebHelper.ParseFromRequest<H10013Request>());
                            break;
                        }
                    case "H10014":
                        {
                            response = LogicHelper.H10014(WebHelper.ParseFromRequest<H10014Request>());
                            break;
                        }
                    case "H10017":
                        {
                            response = LogicHelper.H10017(WebHelper.ParseFromRequest<H10017Request>());
                            break;
                        }
                    case "H10018":
                        {
                            response = LogicHelper.H10018(WebHelper.ParseFromRequest<H10018Request>());
                            break;
                        }
                    case "H10019":
                        {
                            response = LogicHelper.H10019(WebHelper.ParseFromRequest<H10019Request>());
                            break;
                        }
                    case "H10022":
                        {
                            response = LogicHelper.H10022(WebHelper.ParseFromRequest<H10022Request>());
                            break;
                        }
                    case "H10023":
                        {
                            response = LogicHelper.H10023(WebHelper.ParseFromRequest<H10023Request>());
                            break;
                        }
                    case "H10024":
                        {
                            response = LogicHelper.H10024(WebHelper.ParseFromRequest<H10024Request>());
                            break;
                        }
                    case "H10025":
                        {
                            response = LogicHelper.H10025(WebHelper.ParseFromRequest<H10025Request>());
                            break;
                        }
                    case "H10026":
                        {
                            response = LogicHelper.H10026(WebHelper.ParseFromRequest<H10026Request>());
                            break;
                        }
                    case "H10027":
                        {
                            response = LogicHelper.H10027(WebHelper.ParseFromRequest<H10027Request>());
                            break;
                        }
                    case "H10028":
                        {
                            response = LogicHelper.H10028(WebHelper.ParseFromRequest<H10028Request>());
                            break;
                        }
                    case "H10029":
                        {
                            response = LogicHelper.H10029(WebHelper.ParseFromRequest<H10029Request>());
                            break;
                        }
                    case "H10030":
                        {
                            response = LogicHelper.H10030(WebHelper.ParseFromRequest<H10030Request>());
                            break;
                        }
                    case "H10032":
                        {
                            response = LogicHelper.H10032(WebHelper.ParseFromRequest<H10032Request>());
                            break;
                        }
                    case "H10034":
                        {
                            response = LogicHelper.H10034(WebHelper.ParseFromRequest<H10034Request>());
                            break;
                        }
                    case "H10035":
                        {
                            response = LogicHelper.H10035(WebHelper.ParseFromRequest<H10035Request>());
                            break;
                        }
                    case "H10036":
                        {
                            response = LogicHelper.H10036(WebHelper.ParseFromRequest<H10036Request>());
                            break;
                        }
                    case "H10037":
                        {
                            response = LogicHelper.H10037(WebHelper.ParseFromRequest<H10037Request>());
                            break;
                        }
                    case "H10038":
                        {
                            response = LogicHelper.H10038(WebHelper.ParseFromRequest<H10038Request>());
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            if (response == null)
            {
                response = new HBaseResponse
                {
                    error = 1,
                    message = "操作失败，请稍后再试"
                };
            }

            watch.Stop();
            response.time = watch.ElapsedMilliseconds;

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult H10000()
        {
            return HBase("H10000");
        }

        public ActionResult H10001()
        {
            return HBase("H10001");
        }

        public ActionResult H10002()
        {
            return HBase("H10002");
        }

        public ActionResult H10003()
        {
            return HBase("H10003");
        }

        public ActionResult H10004()
        {
            return HBase("H10004");
        }

        public ActionResult H10005()
        {
            return HBase("H10005");
        }

        public ActionResult H10006()
        {
            return HBase("H10006");
        }

        public ActionResult H10007()
        {
            return HBase("H10007");
        }

        public ActionResult H10008()
        {
            return HBase("H10008");
        }

        [FreeAccess]
        public ActionResult H10009()
        {
            return HBase("H10009");
        }

        [FreeAccess]
        public ActionResult H10010()
        {
            return HBase("H10010");
        }

        public ActionResult H10011()
        {
            return HBase("H10011");
        }
        public ActionResult H10012()
        {
            return HBase("H10012");
        }

        public ActionResult H10013()
        {
            return HBase("H10013");
        }

        public ActionResult H10014()
        {
            return HBase("H10014");
        }

        public ActionResult H10015()
        {
            return HBase("H10015");
        }

        public ActionResult H10016()
        {
            return HBase("H10016");
        }

        public ActionResult H10017()
        {
            return HBase("H10017");
        }

        public ActionResult H10018()
        {
            return HBase("H10018");
        }

        public ActionResult H10019()
        {
            return HBase("H10019");
        }

        public ActionResult H10020()
        {
            return HBase("H10020");
        }

        public ActionResult H10021()
        {
            return HBase("H10021");
        }

        public ActionResult H10022()
        {
            return HBase("H10021");
        }

        public ActionResult H10023()
        {
            return HBase("H10021");
        }

        public ActionResult H10024()
        {
            return HBase("H10021");
        }

        public ActionResult H10025()
        {
            return HBase("H10021");
        }

        public ActionResult H10026()
        {
            return HBase("H10021");
        }

        public ActionResult H10027()
        {
            return HBase("H10027");
        }

        public ActionResult H10028()
        {
            return HBase("H10028");
        }

        public ActionResult H10029()
        {
            return HBase("H10029");
        }

        public ActionResult H10030()
        {
            return HBase("H10030");
        }

        public ActionResult H10031()
        {
            return HBase("H10031");
        }

        public ActionResult H10032()
        {
            return HBase("H10032");
        }

        public ActionResult H10033()
        {
            return HBase("H10033");
        }

        public ActionResult H10034()
        {
            return HBase("H10034");
        }

        public ActionResult H10035()
        {
            return HBase("H10035");
        }

        public ActionResult H10036()
        {
            return HBase("H10036");
        }

        public ActionResult H10037()
        {
            return HBase("H10037");
        }

        public ActionResult H10038()
        {
            return HBase("H10038");
        }
    }
}
