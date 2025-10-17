using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.WebHost;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace Neo
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CheckSessionOutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower();
            if (!controllerName.Contains("account"))
            {
                HttpSessionStateBase session = filterContext.HttpContext.Session;
                var user = session["UserID"];
                if (((user == null) && (!session.IsNewSession)) || (session.IsNewSession))
                {
                   
                    var url = new UrlHelper(filterContext.RequestContext);
                    var loginUrl = url.Content("~/Account/Login");

                    //RouteValueDictionary redirectTargetDictionary = new RouteValueDictionary();
                    //redirectTargetDictionary.Add("action", "Login");
                    //redirectTargetDictionary.Add("controller", "Account");
                    //redirectTargetDictionary.Add("timeout", "true");
                    //filterContext.Result = new RedirectToRouteResult(redirectTargetDictionary);

                    filterContext.HttpContext.Response.Redirect(loginUrl, true);
                }
            }
        }
    }    
}