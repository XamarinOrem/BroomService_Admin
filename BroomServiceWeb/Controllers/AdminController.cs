using BroomServiceWeb.Controllers.Web;
using BroomServiceWeb.Helpers;
using BroomServiceWeb.Models;
using BroomServiceWeb.Resources;
using BroomServiceWeb.Services;
using BroomServiceWeb.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO; 
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BroomServiceWeb.Controllers
{
    public class AdminController : MyController
    {
        UserService userService; 
        PropertyService propertyService;
        CategoryService categoryService;
        public AdminController()
        {
            userService = new UserService();
            propertyService = new PropertyService();
            categoryService = new CategoryService();
        } 
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = userService.LoginAdminUser(model.Email, model.Password);
                if (user.UserId != 0)
                {
                    Session["AdminId"] = user.UserId;
                    Session["AdminName"] = user.Name.ToUpper();
                    Session["AdminEmail"] = user.Email;
                    Session["AdminType"] = user.UserType.ToString();

                    Session["UserId"] = null;
                    Session["UserEmail"] = null;
                    Session["UserName"] = null;
                    Session["ProfilePic"] = null;
                    Session["UserType"] = null;
                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    TempData["LoginMsg"] = Resource.invalid_user;
                    return View();
                }
            }
            else { return View(); }
        }

        public ActionResult Logout()
        {
            Session["AdminId"] = null;
            return RedirectToAction("Login");
        }
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = userService.ForgotPassword(model.Email);
                if (result)
                {
                    TempData["SuccessMsg"] = userService.message;
                }
                else
                {
                    TempData["ErrorMsg"] = userService.message;
                }
            }
            return View();
        }
        [AuthAttribute]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                long userId = Convert.ToInt64(Session["AdminId"]);
                var result = userService.ChangePassword(userId, model);
                if (result)
                {
                    TempData["SuccessMsg"] = userService.message;
                }
                else
                {
                    TempData["ErrorMsg"] = userService.message;
                }
            }
            return View();
        }
        [HttpGet]
        public JsonResult GetPendingJobRequestCount()
        {
            var count = propertyService.GetPendingJobRequestCount();
            return Json(count, JsonRequestBehavior.AllowGet);
        }
        [AuthAttribute]
        public ActionResult Dashboard()
        {
            var getData = userService.GetDashboardData(); 
            return View(getData);
        }

        [AuthAttribute]
        public ActionResult UpdateProfile()
        {
            var userId = Convert.ToInt64(Session["AdminId"]);
            var userData = userService.GetAdminUserDetail(userId);
            ProfileViewModel model = new ProfileViewModel()
            {
                FirstName = userData.FirstName,
                LastName = userData.LastName,
                Email = userData.Email,
                PhoneNumber = userData.PhoneNumber
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateProfile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.UserId = Convert.ToInt64(Session["AdminId"]);
                var result = userService.UpdateAdminProfile(model);
                if (result)
                {
                    TempData["SuccessMsg"] = userService.message;
                }
                else
                {
                    TempData["ErrorMsg"] = userService.message;
                }
            }
            return View();
        }



        [AuthAttribute]
        public ActionResult Customers(int? pageNumber)
        {
            TempData["IndexPage"] = pageNumber ?? 1;
            var users = userService.GetUsers(Enums.UserTypeEnum.Customer).ToPagedList(pageNumber ?? 1, 10); 
            return View(users);
        }
        [AuthAttribute]
        public ActionResult CustomerDetail(long id)
        {
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                TempData["PreviousUrl"] = System.Web.HttpContext.Current.Request.UrlReferrer.ToString();
            }
            else
            {
                TempData["PreviousUrl"] = null;
            }
            var detail = userService.GetCustomerDetail(id);
            return View(detail);
        }
        public ActionResult UpdatePropertyStatus(long id)
        { 
            if (userService.UpdatePropertyStatus(id))
            {
                TempData["SuccessMsg"] = categoryService.message;
            }
            else
            {
                TempData["ErrorMsg"] = categoryService.message;
            }
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                var PreviousUrl = System.Web.HttpContext.Current.Request.UrlReferrer.ToString();
                return Redirect(PreviousUrl);
            } 
            else
            {
                return RedirectToAction("Customers", "Admin");
            }
        }
        [AuthAttribute]
        public ActionResult PropertyDetail(long id)
        {
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                TempData["PreviousUrl"] = System.Web.HttpContext.Current.Request.UrlReferrer.ToString();
            }
            else
            {
                TempData["PreviousUrl"] = null;
            }
            var detail = propertyService.GetPropertiesById(id); 
            return View(detail);
        } 
        [HttpGet]
        public JsonResult GetCustomerPaymentMethod(long customerId)
        {
            var paymentMethod = userService.GetCustomerPaymentMethod(customerId);
            return Json(paymentMethod, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UpdatePaymentMethod(long customerId, int paymentMethod)
        { 
            wsBase wsBase = new wsBase();
            wsBase.status = userService.UpdatePaymentMethod(customerId, paymentMethod);
            wsBase.message = userService.message;
            return Json(wsBase, JsonRequestBehavior.AllowGet);
        }

        [AuthAttribute]
        public ActionResult ServiceProviders(int? pageNumber)
        {
            TempData["IndexPage"] = pageNumber ?? 1;
            var users = userService.GetUsers(Enums.UserTypeEnum.ServiceProvider).ToPagedList(pageNumber ?? 1, 10); ;
            return View(users);
        }
        [AuthAttribute]
        public ActionResult AddServiceProvider(int usertype, long? id, int? pageNumber)
        {
            TempData["IndexPage"] = pageNumber;
            TempData["ErrorMsg"] = null;
            TempData["SuccessMsg"] = null;
            var categories = categoryService.GetCatSubCategories(usertype);
            ViewBag.Categories = categories;
            TempData["Title"] = usertype == (int)Enums.UserTypeEnum.ServiceProvider && (id == null || id == 0) ? Resource.add_service_provider
                : usertype == (int)Enums.UserTypeEnum.ServiceProvider && (id != null || id != 0) ? Resource.edit_service_provider :
                usertype == (int)Enums.UserTypeEnum.Worker && (id == null || id == 0) ? Resource.add_worker
                : Resource.edit_worker;

            if (id != null)
            {
                var user = userService.GetUserDetail(id ?? 0);
                if (user != null)
                {
                    return View(user);
                }
                else
                {
                    var model = new UserViewModel();
                    model.UserType = usertype;
                    model.Categories = categories;
                    return View(model);
                }
            }
            else
            {
                var model = new UserViewModel();
                model.Categories = categories;
                model.UserType = usertype;
                return View(model);
            }
        }
        [HttpPost]
        public ActionResult AddServiceProvider(UserViewModel model, FormCollection fc)
        {
            var pageNumber = TempData["IndexPage"];
            model.Categories = categoryService.GetCatSubCategories(model.UserType??0);
            var subCatIds = fc["subCatIds"].ToString();
            var subsubCatIds = fc["subsubCatIds"].ToString();
            var userType = Convert.ToInt32(fc["UserType"].ToString());
            model.UserType = userType;

            TempData["Title"] = userType == (int)Enums.UserTypeEnum.ServiceProvider && (model.UserId == 0) ? Resource.add_service_provider
        : userType == (int)Enums.UserTypeEnum.ServiceProvider && (model.UserId != 0) ? Resource.edit_service_provider :
        userType == (int)Enums.UserTypeEnum.Worker && (model.UserId == 0) ? Resource.add_worker
        : Resource.edit_worker;
            if (ModelState.IsValid)
            {
                var result = userService.AddUpdateServiceProvider(model, subCatIds, subsubCatIds);
                if (result)
                {
                    TempData["SuccessMsg"] = userService.message;
                    if (userType == (int)Enums.UserTypeEnum.ServiceProvider)
                    {
                        return RedirectToAction("ServiceProviders", "Admin", new { pageNumber = pageNumber });
                    }
                    else
                    {
                        return RedirectToAction("Workers", "Admin", new { pageNumber = pageNumber });
                    }
                }
                else
                {
                    TempData["ErrorMsg"] = userService.message;
                }
            }

            return View(model);
        }

        [AuthAttribute]
        public ActionResult Workers(int? pageNumber)
        {
            TempData["IndexPage"] = pageNumber ?? 1;
            var users = userService.GetUsers(Enums.UserTypeEnum.Worker).ToPagedList(pageNumber ?? 1, 10); ;
            return View(users);
        }
        [AuthAttribute]
        public ActionResult WorkerDetail(long id)
        {
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                TempData["PreviousUrl"] = System.Web.HttpContext.Current.Request.UrlReferrer.ToString();
            }
            else
            {
                TempData["PreviousUrl"] = null;
            }
            var detail = userService.GetWorkerDetail(id);
            ViewBag.UserType = detail.User.UserType;
            detail.Orders = propertyService.GetJobRequests(id);
            return View(detail);
        }

        [AllowAnonymous]
        public ActionResult EmailVerification(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return HttpNotFound();
            }
            string userName = string.Empty;
            var status = userService.CheckVerifyEmail(userId, out userName);
            if (status)
            {
                ViewBag.Username = userName;
                return View();
            }
            else
            {
                return HttpNotFound();
            }
        }
        public ActionResult UpdateUserStatus(long id)
        {
            int? usertype;
            if (userService.UpdateUserStatus(id, out usertype))
            {
                TempData["SuccessMsg"] = categoryService.message;
            }
            else
            {
                TempData["ErrorMsg"] = categoryService.message;
            }
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                var PreviousUrl = System.Web.HttpContext.Current.Request.UrlReferrer.ToString();
                return Redirect(PreviousUrl);
            }
            if (usertype == (int)Enums.UserTypeEnum.Customer)
            {
                return RedirectToAction("Customers", "Admin");
            }
            else if (usertype == (int)Enums.UserTypeEnum.ServiceProvider)
            {
                return RedirectToAction("Serviceproviders", "Admin");
            }
            else
            {
                return RedirectToAction("Workers", "Admin");
            }
        }
         


        #region Worker Orders
        [AuthAttribute]
        public ActionResult Notifications(int? pageNumber)
        {
            long AdminId = Convert.ToInt64(Session["AdminId"]);
            var notifications =propertyService.GetAdminNotifications(AdminId).
                ToPagedList(pageNumber ?? 1, 10);  
            return View(notifications);
        }

        public ActionResult AcceptRejectJobRequest(long id,long requestId,bool IsAccept, int? pageNumber)
        {
            long AdminId = Convert.ToInt64(Session["AdminId"]);
            AcceptRejectJobReqAdminModel model = new AcceptRejectJobReqAdminModel()
            {
                IsAccept = IsAccept,
                NotificationId = id,
                JobRequestId =requestId,
                UserId= AdminId
            };
            if (propertyService.AcceptRejectRequestAdmin(model))
            {
                TempData["SuccessMsg"] = propertyService.message;
            }
            else
            {
                TempData["ErrorMsg"] = propertyService.message;
            }
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                var PreviousUrl = System.Web.HttpContext.Current.Request.UrlReferrer.ToString();
                return Redirect(PreviousUrl);
            }
            return RedirectToAction("Notifications", "Admin", new { pageNumber = pageNumber });
        }
     
        [AuthAttribute]
        public ActionResult Orders(int? pageNumber)
        {
            var orders = propertyService.GetAllOrders(pageNumber);
            return View(orders);
        }
        [AuthAttribute]
        public ActionResult AssignWorker(long id, long requestId, int? pageNumber)
        {
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                TempData["PreviousUrl"] = System.Web.HttpContext.Current.Request.UrlReferrer.ToString(); 
            }
            else
            {
                TempData["PreviousUrl"] = null;
            }
            TempData["IndexPage"] = pageNumber ?? 1;
            TempData["NotificationId"] = id;
            ViewBag.Workers = userService.GetWorkers(requestId);
            long AdminId = Convert.ToInt64(Session["AdminId"]);
            var jobDetail = propertyService.GetJobRequestDetail(requestId, AdminId);
            return View(jobDetail); 
        }
        [AuthAttribute]
        public ActionResult OrderDetail(long id, int? pageNumber)
        {
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                TempData["PreviousUrl"] = System.Web.HttpContext.Current.Request.UrlReferrer.ToString();
            }
            else
            {
                TempData["PreviousUrl"] = null;
            }
            TempData["IndexPage"] = pageNumber ?? 1;
            ViewBag.Workers = userService.GetWorkers(id);
            long AdminId = Convert.ToInt64(Session["AdminId"]);
            var jobDetail = propertyService.GetJobRequestDetail(id, AdminId);
            return View(jobDetail);
        }
        [HttpPost]
        public JsonResult SendQuote(long requestId,decimal price)
        {
            long userId = Convert.ToInt64(Session["AdminId"]);
            QuotePriceModel quotePriceModel = new QuotePriceModel()
            {
                JobRequestId= requestId,
                QuotePrice=price ,
                UserId= userId
            };
            wsBase wsBase = new wsBase();
            wsBase.status = propertyService.SendQuote(quotePriceModel,true);
            wsBase.message = propertyService.message;
            return Json(wsBase, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AssignJobToWorker(long requestId, long workerId)
        {
            long AdminId = Convert.ToInt64(Session["AdminId"]); 
            wsBase wsBase = new wsBase();
            wsBase.status = propertyService.AssignJobToWorker(requestId, workerId, AdminId);
            wsBase.message = propertyService.message;
            return Json(wsBase, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Chat
        [AuthAttribute]
        public ActionResult AdminChatListing()
        {
            long AdminId = Convert.ToInt64(Session["AdminId"]);
            ChatListVM chatListVM = new ChatListVM(); 
            var result = propertyService.GetChat(AdminId);
            if (result != null)
            {
                if (result.Count > 0)
                {
                    chatListVM.chatUser = result;
                }
            }
            return View(chatListVM);
        }
        [HttpPost]
        public JsonResult SendMessage(long userId)
        {
            long userID = Convert.ToInt64(Session["UserId"]);
            //ChatDetailListModel model = new ChatDetailListModel()
            //{
            //    SenderUserId = userID,
            //    RecieverUserId = userId,
            //    UserMessage = text,
            //    UserMessageTime = DateTime.Now.ToString("hh:mm tt"),
            //    IsSender = true,
            //    TimeStamp=timestamp
            //};
            ChatRequestModel m = new ChatRequestModel()
            {
                FromUserId = userID,
                ToUserId = userId
            };
            propertyService.AddChatRequest(m);
            //var status = await FirebaseHelper.AddChatMessage(model);
            wsBase wsBase = new wsBase();
            wsBase.status = true;
            return Json(wsBase, JsonRequestBehavior.AllowGet);
        }
       
        [AuthAttribute]
        public   ActionResult  Chat(long userId)
        {
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                TempData["PreviousUrl"] = System.Web.HttpContext.Current.Request.UrlReferrer.ToString();
            }
            else
            {
                TempData["PreviousUrl"] = null;
            }
            var userData = userService.GetUserDetail(userId);
            if (userData != null)
            {
                TempData["UserName"] = userData.FirstName + " " + userData.LastName;
                TempData["UserId"] = userData.UserId;
            }
           
            return View();
        }
        [HttpGet]
        public async Task<JsonResult> GetChat(long userId)
        {
            long AdminId = Convert.ToInt64(Session["AdminId"]);
            var chatData = await FirebaseHelper.GetChatForUserID(AdminId, userId); 
            return Json(chatData, JsonRequestBehavior.AllowGet);
        }

      
        #endregion
        #region Setting
        [AuthAttribute]
        public ActionResult Setting()
        {
            var result = userService.GetSettingData();
            if (result != null)
            {
                return View(result);
            }
            else
            {
                return View();
            }
        }
         
        [AuthAttribute]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Setting(SettingModel model)
        {
            if (ModelState.IsValid)
            {
                var result = userService.AddUpdateSettingData(model);
            }
            return View();
        }
        #endregion
        #region About us
        [AuthAttribute]
        public ActionResult AboutUs()
        {
            var result = userService.GetAboutUsData();
            if (result != null)
            {
                return View(result);
            }
            else
            {
                return View();
            }
        }

        [AuthAttribute]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AboutUs(AboutUsViewModel aboutUsModel)
        {
            if (ModelState.IsValid)
            {
                var result = userService.AddUpdateAboutUsData(aboutUsModel);
            }
            return View();
        }
        #endregion

        #region Terms & Conditions
        [AuthAttribute]
        public ActionResult TermsConditions()
        {
            var result = userService.GetTermsConditions();
            if (result != null)
            {
                return View(result);
            }
            else
            {
                return View();
            }
        }

        [AuthAttribute]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult TermsConditions(TermsCondition model)
        {
            if (ModelState.IsValid)
            {
                var result = userService.AddUpdateTermsConditionsData(model);
            }
            return View();
        }

        #endregion


        #region  Privacy Policy
        [AuthAttribute]
        public ActionResult AddUpdatePrivacyPolicy()
        {
            var result = userService.GetPrivacyPolicy();
            if (result != null)
            {
                return View(result);
            }
            else
            {
                return View();
            }
        }

        [AuthAttribute]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddUpdatePrivacyPolicy(PrivacyPolicy model)
        {
            if (ModelState.IsValid)
            {
                var result = userService.AddUpdatePrivacyPolicyData(model);
            }
            return View();
        }

        #endregion
        
        #region Testimonials

        public ActionResult GetTestimonials(int? pageNumber)
        {
            TempData["IndexPage"] = pageNumber ?? 1;
            var testimonials = userService.GetTestimonials().ToPagedList(pageNumber ?? 1, 10);
            return View(testimonials);
        }

        public ActionResult AddTestimonials(long? id, int? pageNumber)
        {
            TempData["IndexPage"] = pageNumber;
            TempData["ErrorMsg"] = null;
            TempData["SuccessMsg"] = null;
            TempData["Title"] = id == null || id == 0 ? "Add Testimonial" : "Edit Testimonial";
            if (id != null)
            {
                var testimonial = userService.GetTestimonialById(id ?? 0);
                return View(testimonial);
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult AddTestimonials(Testimonial model, HttpPostedFileBase file,
            HttpPostedFileBase fileIcon)
        {
            var pageNumber = TempData["IndexPage"];
            TempData["Title"] = model.Id == 0 ? "Add Testimonial" : "Edit Testimonial";
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".png", ".jpeg", ".JPG", ".PNG", ".JPEG" };
                    var ext = System.IO.Path.GetExtension(file.FileName);

                    var fileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName) + ext.ToLower();

                    if (!AllowedFileExtensions.Contains(ext))
                    {
                        ViewBag.ErrorMessage = "File Extension Is InValid - Only Upload jpg/png/jpeg/JPG/PNG/JPEG .";
                    }
                    ext = Path.GetExtension(file.FileName);
                    {
                        string path = "~/Images/Testimonial";
                        if (!Directory.Exists(Server.MapPath(path)))
                            Directory.CreateDirectory(Server.MapPath(path));

                        path = System.IO.Path.Combine(Server.MapPath(path),
                                                   System.IO.Path.GetFileName(fileName));
                        file.SaveAs(path);
                        model.Image = fileName;
                    }
                }
                var result = userService.AddUpdateTestimonial(model);
                if (result)
                {
                    TempData["SuccessMsg"] = userService.message;
                    return RedirectToAction("GetTestimonials", "Admin", new { pageNumber = pageNumber });
                }
                else
                {
                    TempData["ErrorMsg"] = userService.message;
                }
            }
            return View(model);
        }

        public ActionResult UpdateTestimonialStatus(long id, int? pageNumber)
        {
            if (userService.UpdateTestimonialStatus(id))
            {
                TempData["SuccessMsg"] = userService.message;
            }
            else
            {
                TempData["ErrorMsg"] = userService.message;
            }
            return RedirectToAction("GetTestimonials", "Admin", new { pageNumber = pageNumber });
        }

        public ActionResult DeleteTestimonial(long id, int? pageNumber)
        {
            if (userService.DeleteTestimonial(id))
            {
                TempData["SuccessMsg"] = userService.message;
            }
            else
            {
                TempData["ErrorMsg"] = userService.message;
            }
            return RedirectToAction("GetTestimonials", "Admin", new { pageNumber = pageNumber });
        }
        #endregion


    }
}