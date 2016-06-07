using Agile.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace www.llyn23.com.Attributes
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

            //获取已登录的用户名
            string logedUser = HttpContext.Current.Session["logeduser"] as string;
            if (String.IsNullOrEmpty(logedUser))
            {
                filterContext.Result = new JsonResult
                {
                    Data = new { error = 1, message = "未登录" },
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