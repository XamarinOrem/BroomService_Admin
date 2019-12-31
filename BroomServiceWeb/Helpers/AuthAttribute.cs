using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace BroomServiceWeb.Helpers
{
    public class AuthAttribute : ActionFilterAttribute, IAuthenticationFilter
    {
        private bool _auth;
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            _auth = (filterContext.ActionDescriptor.GetCustomAttributes(
                typeof(OverrideAuthenticationAttribute), true).Length == 0);
        }


        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            var user = System.Web.HttpContext.Current.Session["AdminId"];

            if (user == null)
            {
                filterContext.Result = new RedirectResult("~/Admin/Login");
            }
        }
    }
}