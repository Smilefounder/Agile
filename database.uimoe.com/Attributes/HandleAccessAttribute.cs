using Agile.Attributes;
using Agile.API.Dtos;
using Agile.Helpers;
using Agile.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace database.uimoe.com.Attributes
{
    public class HandleAccessAttribute : AuthorizeAttribute, IRequiresSessionState
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //贴有标签则不用登录也可以访问
            if (HasAttribute<FreeAccessAttribute>(filterContext))
            {
                return;
            }

            var token = HttpContext.Current.Request.Params["token"];
            if (!String.IsNullOrEmpty(token))
            {
                HttpContext.Current.Session["token"] = token;
            }
            else
            {
                token = HttpContext.Current.Session["token"] as string;
            }

            var response = new HBaseResponse
            {
                error = 1,
                message = "未登录"
            };

            if (String.IsNullOrEmpty(token))
            {
                filterContext.Result = new JsonResult
                {
                    Data = response,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

                return;
            }


            try
            {
                response = LogicHelper.H10032(new H10032Request
                 {
                     token = token
                 });

                if (response != null && response.error != 0)
                {
                    filterContext.Result = new JsonResult
                    {
                        Data = response,
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
                filterContext.Result = new JsonResult
                {
                    Data = response,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        /// <summary>
        /// 判断是否贴有属性标签
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        private bool HasAttribute<T>(AuthorizationContext filterContext) where T : Attribute
        {
            Type attrType = typeof(T);
            if (filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(attrType, false))
            {
                return true;
            }

            if (filterContext.ActionDescriptor.IsDefined(attrType, false))
            {
                return true;
            }

            return false;
        }
    }
}