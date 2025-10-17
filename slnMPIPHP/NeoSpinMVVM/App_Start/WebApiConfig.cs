using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Routing;

namespace Neo
{
    public static class WebApiConfig
    {
        public static string ControllerOnly = "ApiControllerOnly";
        public static string ControllerAndId = "ApiControllerAndIntegerId";
        public static string ControllerAction = "ApiControllerAction";

        public static void RegisterRoutes(RouteCollection routes)
        {
            //routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

           // routes.MapHttpRoute(
           //    name: ControllerAction,
           //    routeTemplate: "api/{controller}/{action}"
           //);

            routes.MapHttpRoute(
                name: ControllerAction,
                routeTemplate: "api/{controller}/{action}",
                defaults: null,
                constraints: null,
                handler: new WebapiSessionHandler(GlobalConfiguration.Configuration)
            );
        }
    }
}
