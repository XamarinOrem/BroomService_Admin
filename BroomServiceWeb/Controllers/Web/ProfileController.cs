using BroomServiceWeb.Helpers;
using BroomServiceWeb.Models;
using BroomServiceWeb.Resources;
using BroomServiceWeb.Services;
using BroomServiceWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BroomServiceWeb.Controllers.Web
{
    public class ProfileController : MyController
    {
        UserService userService;

        public ProfileController()
        {
            userService = new UserService();
        }

        #region Profile
        [WebAuthAttribute]
        public ActionResult MyProfile()
        {
            var userId = Convert.ToInt64(Session["UserId"]);
            if (userId != 0)
            {
                var result = userService.GetProfile(userId);
                return View(result);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        [WebAuthAttribute]
        public ActionResult EditProfile()
        {
            var userId = Convert.ToInt64(Session["UserId"]);
            if (userId != 0)
            {
                ViewBag.CountryCodes = userService.GetCountryCodeSelect();
                ViewBag.Countries = userService.GetCountriesSelect();
                var result = userService.GetProfile(userId);
                //result.UserTypeText = result.CountryCode;
                return View(result);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult EditProfile(UserModel userModel, HttpPostedFileBase myfile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (myfile != null)
                    {
                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".png", ".jpeg", ".JPG", ".PNG", ".JPEG" };
                        var ext = System.IO.Path.GetExtension(myfile.FileName);

                        var fileName = Path.GetFileNameWithoutExtension(myfile.FileName) + ext.ToLower();

                        if (!AllowedFileExtensions.Contains(ext))
                        {
                            ViewBag.ErrorMessage = Resource.file_extension_invalid;
                        }
                        ext = System.IO.Path.GetExtension(myfile.FileName);
                        {
                            // Saving the image in to the local path
                            string path = System.IO.Path.Combine(Server.MapPath("~/Images/User/"),
                                                       System.IO.Path.GetFileName(fileName));
                            myfile.SaveAs(path);
                            userModel.PicturePath = fileName;
                        }
                    }
                    var userData = userService.EditProfile(userModel);
                    if (userData)
                    {
                        ModelState.Clear();
                        Session["UserName"] = userModel.FirstName + " " + userModel.LastName;
                        Session["ProfilePic"] = userModel.PicturePath;
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

            ViewBag.CountryCodes = userService.GetCountryCodeSelect();
            ViewBag.Countries = userService.GetCountriesSelect();
            return RedirectToAction("MyProfile");
        }

        #endregion

        #region About Us
        [WebAuthAttribute]
        public ActionResult AboutUs()
        {
            var result = userService.GetAboutUsData();
            return View(result);
        }
        #endregion

        #region Terms Conditions
        [WebAuthAttribute]
        public ActionResult TermsConditions()
        {
            var result = userService.GetTermsConditions();
            return View(result);
        }
        #endregion

        #region Contact Us

        [WebAuthAttribute]
        public ActionResult ContactUs()
        {
            var userId = Convert.ToInt64(Session["UserId"]);
            if (userId != 0)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult ContactUs(ContactU data)
        {
            ContactUsViewModel contactUsViewModel = new ContactUsViewModel();
            contactUsViewModel.Email = data.Email;
            contactUsViewModel.Name = data.Name;
            contactUsViewModel.Message = data.Message;
            contactUsViewModel.UserId = Convert.ToInt64(Session["UserId"]);

            var response = userService.ContactUs(contactUsViewModel);
            if (response)
            {
                ModelState.Clear();
                TempData["SuccessMsg"] = userService.message;
            }
            else
            {
                TempData["ErrorMsg"] = userService.message;
            }
            return View();
        }
        #endregion
    }
}