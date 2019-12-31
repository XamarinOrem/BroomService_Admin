using BroomServiceWeb.Helpers;
using BroomServiceWeb.Models;
using BroomServiceWeb.Resources;
using BroomServiceWeb.Services;
using BroomServiceWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BroomServiceWeb.Controllers.Web
{
    public class AccountController : MyController
    {
        UserService userService;

        public AccountController()
        {
            userService = new UserService();
        }

        #region Login

        public ActionResult Login()
        {
            LoginViewModel model = new LoginViewModel();
            if (Request.Cookies["Login"] != null)
            {
                model.Email = Request.Cookies["Login"].Values["EmailID"];
                model.Password = Request.Cookies["Login"].Values["Password"];
                model.RememberMe = true;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                UserModel userModel = new UserModel();
                userModel.Email = model.Email;
                userModel.Password = model.Password;
                var user = userService.GetLoginUserWeb(userModel);
                if (user != null)
                 {
                    Session["AdminId"] = null;
                    Session["AdminName"] = null;
                    Session["AdminEmail"] = null;
                    Session["AdminType"] = null;

                    Session["UserId"] = user.UserId;
                    Session["UserEmail"] = user.Email;
                    Session["UserName"] = user.FirstName + " " + user.LastName;
                    Session["ProfilePic"] = user.Image;
                    Session["UserType"] = user.UserType;
                    if (model.RememberMe)
                    {
                        HttpCookie cookie = new HttpCookie("Login");
                        cookie.Values.Add("EmailID", model.Email);
                        cookie.Values.Add("Password", model.Password);
                        cookie.Expires = DateTime.Now.AddDays(15);
                        Response.Cookies.Add(cookie);
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["ErrorMsg"] = userService.message;
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        #endregion

        #region Register

        public ActionResult Register()
        {
            //ViewBag.CountryCodes = userService.GetCountryCodeSelect();
            UserModel userModel = new UserModel();
            userModel.CountryCode = "+972";
            ViewBag.Countries = userService.GetCountriesSelect();
            return View(userModel);
        }

        [HttpPost]
        public ActionResult Register(UserModel userModel, HttpPostedFileBase PicturePath)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (PicturePath != null)
                    {
                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".png", ".jpeg", ".JPG", ".PNG", ".JPEG" };
                        var ext = System.IO.Path.GetExtension(PicturePath.FileName);

                        var fileName = Path.GetFileNameWithoutExtension(PicturePath.FileName) + ext.ToLower();

                        if (!AllowedFileExtensions.Contains(ext))
                        {
                            ViewBag.ErrorMessage = Resource.file_extension_invalid;
                        }
                        ext = System.IO.Path.GetExtension(PicturePath.FileName);
                        {
                            // Saving the image in to the local path
                            string path = System.IO.Path.Combine(Server.MapPath("~/Images/User/"),
                                                       System.IO.Path.GetFileName(fileName));
                            PicturePath.SaveAs(path);
                            userModel.PicturePath = fileName;
                        }
                    }
                    var userData = userService.SignUp(userModel);
                    if (userData != null)
                    {
                        ModelState.Clear();
                        TempData["SuccessMsg"] = userService.message;
                    }
                    else
                    {
                        TempData["ErrorMsg"] = userService.message;
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                }
            }

            //ViewBag.CountryCodes = userService.GetCountryCodeSelect();
            ViewBag.Countries = userService.GetCountriesSelect();
            return View();
        }

        #endregion

        #region Forgot Password

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
                    ModelState.Clear();
                    TempData["SuccessMsg"] = userService.message;
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["ErrorMsg"] = userService.message;
                }
            }
            return View();
        }


        #endregion

        #region Change Password

        [WebAuthAttribute]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                long userId = Convert.ToInt64(Session["UserId"]);
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

        #endregion

        #region Logout

        public ActionResult Logout()
        {
            Session["UserId"] = null;
            Session["UserEmail"] = null;
            Session["UserName"] = null;
            Session["ProfilePic"] = null;
            return RedirectToAction("Login");
        }

        #endregion

        #region Terms Conditions

        public ActionResult TermsConditions()
        {
            var result = userService.GetTermsConditions();
            return PartialView("TermsConditions", result);
        }

        #endregion
    }
}