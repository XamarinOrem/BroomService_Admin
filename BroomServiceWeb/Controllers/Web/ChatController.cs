using BroomServiceWeb.Helpers;
using BroomServiceWeb.Models;
using BroomServiceWeb.Services;
using BroomServiceWeb.ViewModels;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BroomServiceWeb.Controllers.Web
{
    public class ChatController : MyController
    {
        UserService userService;
        PropertyService propertyService;

        public ChatController()
        {
            userService = new UserService();
            propertyService = new PropertyService();
        }

        // GET: Chat
        [WebAuthAttribute]
        public ActionResult Index(long userId)
        {
            var _userId = Convert.ToInt64(Session["UserId"]);
            if (_userId != 0)
            {
                var userData = userService.GetUserDetail(userId);
                if (userData != null)
                {
                    TempData["UserName"] = userData.FirstName + " " + userData.LastName;
                    TempData["UserId"] = userData.UserId;
                    TempData["UserImage"] = userData.PicturePath;
                }

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [WebAuthAttribute]
        [HttpGet]
        public async Task<JsonResult> GetChat(long userId)
        {
            if (userId != 0)
            {
                long UserId = Convert.ToInt64(Session["UserId"]);
                var chatData = await FirebaseHelper.GetChatForUserID(UserId, userId);
                return Json(chatData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Session Expired", JsonRequestBehavior.AllowGet);
            }
        }

        [WebAuthAttribute]
        [HttpPost]
        public JsonResult SendMessage(long userId)
        {
            if (userId != 0)
            {
                long userID = Convert.ToInt64(Session["UserId"]);
                ChatRequestModel m = new ChatRequestModel()
                {
                    FromUserId = userID,
                    ToUserId = userId
                };
                propertyService.AddChatRequest(m);
                wsBase wsBase = new wsBase();
                wsBase.status = true;
                return Json(wsBase, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Session Expired", JsonRequestBehavior.AllowGet);
            }
        }

        [WebAuthAttribute]
        public ActionResult ChatListing()
        {
            ChatListVM chatListVM = new ChatListVM();
            var userId = Convert.ToInt64(Session["UserId"]);
            if (userId != 0)
            {
                var result = propertyService.GetChat(userId);
                if (result != null)
                {
                    if (result.Count > 0)
                    {
                        chatListVM.chatUser = result;
                    }
                }
                return View(chatListVM);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
    }
}