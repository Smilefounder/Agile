using Agile.Attributes;
using Agile.Dtos.API;
using Agile.Helpers;
using Agile.Helpers.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace cantonesedict.uimoe.com.Attributes
{
    public class HandleAccessAttribute : AuthorizeAttribute, IRequiresSessionState
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //贴有标签则添加一条访问记录
            if (HasAttribute<NewVisitAttribute>(filterContext))
            {
                var request = HttpContext.Current.Request;
                ThreadPool.QueueUserWorkItem(new WaitCallback(CreateNewVisit), request);
            }

            //贴有标签则不需要登录也可以访问
            if (HasAttribute<FreeAccessAttribute>(filterContext))
            {
                return;
            }

            //否则转到登录页
            var username = HttpContext.Current.Session["username"] as string;
            if (String.IsNullOrEmpty(username))
            {
                filterContext.Result = new RedirectResult("~/Home/Login");
            }
        }

        private void CreateNewVisit(object state)
        {
            try
            {
                var request = state as HttpRequest;
                if (request == null)
                {
                    return;
                }

                LogicHelper.H10036(new H10036Request
                {
                    ipaddress = request.UserHostAddress,
                    rawurl = request.Url.ToString(),
                    useragent = request.UserAgent
                });
            }
            catch
            {

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