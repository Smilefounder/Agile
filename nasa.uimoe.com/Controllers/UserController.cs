﻿using Agile.Attributes;
using Agile.API.Dtos;
using Agile.Helpers;
using Agile.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace nasa.uimoe.com.Controllers
{
    public class UserController : Controller
    {
        [FreeAccess]
        [HttpPost]
        public ActionResult Login(H10009Request vm)
        {
            try
            {
                var resonsebase = LogicHelper.H10009(vm);
                if (resonsebase.error == 0)
                {
                    Session["logeduser"] = vm.username;
                    resonsebase.message = "登录成功";
                }

                return Json(resonsebase);
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
                return Json(new { error = 1, message = "操作失败，请稍后再试" });
            }
        }

        [FreeAccess]
        [HttpPost]
        public ActionResult Register(H10010Request vm)
        {
            try
            {
                var resonsebase = LogicHelper.H10010(vm);
                return Json(resonsebase);
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
                return Json(new { error = 1, message = "操作失败，请稍后再试" });
            }
        }
    }
}
