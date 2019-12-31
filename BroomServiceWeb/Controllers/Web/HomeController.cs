using BroomServiceWeb.Helpers;
using BroomServiceWeb.Services;
using BroomServiceWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BroomServiceWeb.Controllers.Web
{
    public class HomeController : MyController
    {
        HomeService homeService;

        public HomeController()
        {
            homeService = new HomeService();
        }

        public ActionResult ChangeLanguage(string lang)
        {
            new LanguageMang().SetLanguage(lang);
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                TempData["PreviousUrl"] = System.Web.HttpContext.Current.Request.UrlReferrer.ToString();
            }
            return Redirect((string)TempData["PreviousUrl"]);
        }

        // GET: Home
        [WebAuthAttribute]
        public ActionResult Index()
        {
            var getUserId = Convert.ToInt64(Session["UserId"]);
            if (getUserId != 0)
            {
                var getData = homeService.GetHomeData(getUserId);
                var culVal = System.Globalization.CultureInfo.CurrentCulture.Name;
                if (getData != null)
                {
                    if (getData.Categories.Count > 0)
                    {
                        for (int i = 0; i < getData.Categories.Count; i++)
                        {
                            getData.Categories[i].Name = culVal == "fr-FR" ? getData.Categories[i].Name_French
    : culVal == "ru-RU" ? getData.Categories[i].Name_Russian
    : culVal == "he-IL" ? getData.Categories[i].Name_Hebrew
    : getData.Categories[i].Name;
                        }
                    }
                }
                return View(getData);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
    }
}