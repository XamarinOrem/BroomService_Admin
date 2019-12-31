using BroomServiceWeb.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace BroomServiceWeb
{
    public static class WebApiConfig
    { 
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.MessageHandlers.Add(new LanguageMessageHandler());
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
