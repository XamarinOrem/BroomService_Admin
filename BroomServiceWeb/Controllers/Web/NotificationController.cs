using BroomServiceWeb.Helpers;
using BroomServiceWeb.Models;
using BroomServiceWeb.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BroomServiceWeb.Controllers.Web
{
    public class NotificationController : MyController
    {
        PropertyService propertyService;

        public NotificationController()
        {
            propertyService = new PropertyService();
        }

        [WebAuthAttribute]
        // GET: Notification
        public ActionResult Index(int? pageNumber)
        {
            var userId = Convert.ToInt64(Session["UserId"]);
            if (userId != 0)
            {
                var result = propertyService.GetNotifications(userId).OrderByDescending(a => a.CreatedDate).ToPagedList(pageNumber ?? 1, 10);
                return View(result);
            }
            else
            {
                return RedirectToAction("Login","Account");
            }
        }

        [HttpPost]
        public JsonResult AcceptRejectQuote(AcceptRejectQuoteModel model)
        {
            model.UserId= Convert.ToInt64(Session["UserId"]);
            var status = propertyService.AcceptRejectQuote(model);

            return Json("Success",JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AcceptRejectRequest(AcceptRejectJobReqModel model)
        {
            model.UserId = Convert.ToInt64(Session["UserId"]);
            var status = propertyService.AcceptRejectRequest(model);

            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}