using BroomServiceWeb.Controllers.Web;
using BroomServiceWeb.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BroomServiceWeb.Controllers
{
    public class TermsController : MyController
    {
        UserService userService;

        public TermsController()
        {
            userService = new UserService();
        } 
        public ActionResult Index()
        {
            var result = userService.GetTermsConditions();
            return View(result);
        }
        public ActionResult PrivacyPolicy()
        {
            var result = userService.GetPrivacyPolicy();
            return View(result);
        }
       

    }
}