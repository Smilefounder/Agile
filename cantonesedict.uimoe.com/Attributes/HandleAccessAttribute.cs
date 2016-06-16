using Agile.Attributes;
using Agile.Dtos.API;
using Agile.Helpers;
using Agile.Helpers.API;
using cantonesedict.uimoe.com.ViewModels;
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
            var userinfo = HttpContext.Current.Session["userinfo"] as UserInfoVM;
            if (userinfo == null)
            {
                filterContext.Result = new RedirectResult("~/Home/Login");
                return;
            }

            //贴有标签则登录后就可以访问
            if (HasAttribute<LoginAccessAttribute>(filterContext))
            {
                return;
            }

            //否则校验页面权限
            var permission = userinfo.UserPermissions.Where(a => filterContext.HttpContext.Request.RawUrl.StartsWith(a.RawUrl)).FirstOrDefault();
            if (permission == null)
            {
                filterContext.Result = new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        error = 1,
                        message = "很抱歉，您没有权限执行此操作"
                    }
                };
                return;
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
                    rawurl = request.RawUrl.ToString(),
                    useragent = request.UserAgent,
                    domain = (int)H10044RequestDomainEnum.cantonesedict
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