using BroomServiceWeb.Models;
using BroomServiceWeb.Resources;
using BroomServiceWeb.Services;
using BroomServiceWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace BroomServiceWeb.Controllers.Api
{
    public class AccountController : ApiController
    {
        UserService userService;
        public AccountController()
        {
            userService = new UserService();
        }

        /// <summary>
        /// Get Countries api
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetCountries()
        {
            var result = userService.GetCountries();
            string msg = string.Empty;
            if (result == null)
            {
                msg = Resource.no_data_found;
            }
            else
            {
                msg = Resource.success;
            }
            return this.Ok(new
            {
                status = result == null ? false : true,
                message = msg,
                data = result
            });
        }

        /// <summary>
        /// Login Api
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Login(UserModel model)
        {
            if (model == null)
            {
                return this.Ok(new
                {
                    status = false,
                    message = Resource.fill_required_records,
                });
            }
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return this.Ok(new
                {
                    status = false,
                    message = Resource.fill_required_records,
                });
            }
            var userData = userService.GetLoginUserApp(model);

            return this.Ok(new
            {
                status = userData == null ? false : true,
                message = userService.message,
                userData = userData
            });
        }

        /// <summary>
        /// Logout Api
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Logout(UserModel model)
        {
            var status = userService.Logout(model.UserId);
            return this.Ok(new
            {
                status = status,
                message = userService.message
            });
        }
        /// <summary>
        /// SignUp Api
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult SignUp()
        {
            UserModel userModel = new UserModel();

            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);
                IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".png", ".jpeg", ".JPG", ".PNG", ".JPEG" };
                var date = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss").Replace("-", "_");

                var count = httpRequest.Files.Count;
                if (count > 0)
                {
                    for (int i = 0; i < httpRequest.Files.Count; i++)
                    {
                        if (httpRequest.Files[i] != null)
                        {
                            var postedImg = httpRequest.Files[i];
                            var ext = postedImg.FileName.Substring(postedImg.FileName.LastIndexOf('.'));

                            var path = "~/Images/User/";

                            var imagePath = Path.GetFileNameWithoutExtension(postedImg.FileName) + "_" + date + ext.ToLower();
                            var fileName = path + imagePath;

                            if (!AllowedFileExtensions.Contains(ext))
                            {
                                return this.Ok(new
                                {
                                    status = false,
                                    message = Resource.file_extension_invalid
                                });
                            }

                            if (!Directory.Exists(HttpContext.Current.Server.MapPath(path)))
                                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(path));

                            postedImg.SaveAs(HttpContext.Current.Server.MapPath(fileName));

                            userModel.PicturePath = imagePath;
                        }
                    }
                }
            }

            userModel.FirstName = httpRequest.Form.Get("FirstName");
            userModel.LastName = httpRequest.Form.Get("LastName");
            userModel.Password = httpRequest.Form.Get("Password");
            userModel.Email = httpRequest.Form.Get("Email"); 
            userModel.DeviceId = Convert.ToInt32(httpRequest.Form.Get("DeviceId"));
            userModel.DeviceToken = httpRequest.Form.Get("DeviceToken");
           // userModel.Latitude = Convert.ToDecimal(httpRequest.Form.Get("Latitude"));
            //userModel.Longitude = Convert.ToDecimal(httpRequest.Form.Get("Longitude"));
            userModel.CountryId = Convert.ToInt32(httpRequest.Form.Get("CountryId"));
            userModel.CountryCode = httpRequest.Form.Get("CountryCode");
            userModel.PhoneNumber =  httpRequest.Form.Get("PhoneNumber");
            userModel.City = httpRequest.Form.Get("City");
            userModel.Address = httpRequest.Form.Get("Address");
            userModel.Zipcode = httpRequest.Form.Get("Zipcode");
            userModel.UserType = Convert.ToInt32(httpRequest.Form.Get("UserType"));

            if (string.IsNullOrEmpty(userModel.FirstName) || string.IsNullOrEmpty(userModel.Email)
                || string.IsNullOrEmpty(userModel.Password))
            {
                return this.Ok(new
                {
                    status = false,
                    message = Resource.fill_required_records,
                });
            }

            var userData = userService.SignUp(userModel);

            return this.Ok(new
            {
                status = userData == null ? false : true,
                message = userService.message,
                userData = userData
            });
        }
        [HttpPost]
        public IHttpActionResult UpdateDeviceInfo(UserModel model)
        {
            try
            { 
                var response = userService.UpdateDeviceInfo(model.UserId, model.DeviceId??0, model.DeviceToken);
                return this.Ok(new
                {
                    status = response,
                    message = userService.message
                });
            }
            catch (Exception ex)
            {
                return this.Ok(new
                {
                    Status = false,
                    Message = ex.Message
                });
            }
        }
        [HttpPost]
        public IHttpActionResult EditProfile()
        {
            UserModel userModel = new UserModel();

            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);
                IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".png", ".jpeg", ".JPG", ".PNG", ".JPEG" };
                var date = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss").Replace("-", "_");

                var count = httpRequest.Files.Count;
                if (count > 0)
                {
                    for (int i = 0; i < httpRequest.Files.Count; i++)
                    {
                        if (httpRequest.Files[i] != null)
                        {
                            var postedImg = httpRequest.Files[i];
                            var ext = postedImg.FileName.Substring(postedImg.FileName.LastIndexOf('.'));


                            var path = "~/Images/User/";

                            var imagePath = Path.GetFileNameWithoutExtension(postedImg.FileName) + "_" + date + ext.ToLower();
                            var fileName = path + imagePath;

                            if (!AllowedFileExtensions.Contains(ext))
                            {
                                return this.Ok(new
                                {
                                    status = false,
                                    message = Resource.file_extension_invalid
                                });
                            }

                            if (!Directory.Exists(HttpContext.Current.Server.MapPath(path)))
                                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(path));

                            postedImg.SaveAs(HttpContext.Current.Server.MapPath(fileName));

                            userModel.PicturePath = imagePath;
                        }
                    }
                }
            } 
            userModel.UserId = Convert.ToInt32(httpRequest.Form.Get("UserId"));

            userModel.FirstName = httpRequest.Form.Get("FirstName");
            userModel.LastName = httpRequest.Form.Get("LastName");
            userModel.Password = httpRequest.Form.Get("Password");
            userModel.Email = httpRequest.Form.Get("Email");
            userModel.DeviceId = Convert.ToInt32(httpRequest.Form.Get("DeviceId"));
            userModel.DeviceToken = httpRequest.Form.Get("DeviceToken");
            // userModel.Latitude = Convert.ToDecimal(httpRequest.Form.Get("Latitude"));
            //userModel.Longitude = Convert.ToDecimal(httpRequest.Form.Get("Longitude"));
            userModel.CountryId = Convert.ToInt32(httpRequest.Form.Get("CountryId"));
            userModel.CountryCode = httpRequest.Form.Get("CountryCode");
            userModel.PhoneNumber = httpRequest.Form.Get("PhoneNumber");
            userModel.City = httpRequest.Form.Get("City");
            userModel.Address = httpRequest.Form.Get("Address");
            userModel.Zipcode = httpRequest.Form.Get("Zipcode");  

            var status = userService.EditProfile(userModel);

            return this.Ok(new
            {
                status = status,
                message = userService.message
            });
        }
        /// <summary>
        /// Forgot Password Api
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ForgetPassword(ForgotPasswordViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Email))
                {
                    return this.Ok(new
                    {
                        Status = false,
                        Message =Resource.enteremail,
                    });
                }
                var response = userService.ForgotPassword(model.Email);
                return this.Ok(new
                {
                    status = response,
                    message = userService.message
                });
            }
            catch (Exception ex)
            {
                return this.Ok(new
                {
                    Status = false,
                    Message = ex.Message
                });
            }
        }
        [HttpPost]
        public IHttpActionResult ChangePassword(ChangePasswordModel model)
        { 
               var response = userService.ChangePassword(model.userId??0, model);
            return this.Ok(new
            {
                status = response,
                message = userService.message
            });
        }
        /// <summary>
        /// Get Profile Api
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetProfile(long userId)
        {
            var result = userService.GetProfile(userId); 
            return this.Ok(new
            {
                status = result == null ? false : true,
                message = userService.message,
                data = result
            });
        }
        /// <summary>
        /// About Us Api
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetAboutus()
        {
            var result = userService.GetAboutus();
            string msg = string.Empty;
            if (result == null)
            {
                msg = Resource.no_data_found;
            }
            else
            {
                msg = Resource.success;
            }
            return this.Ok(new
            {
                status = result == null ? false : true,
                message = msg,
                AboutUsData = result
            });
        }

       

        /// <summary>
        /// Terms Conditions Api
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetTermsConditions()
        {
            var result = userService.GetTermsConditions();
            string msg = string.Empty;
            if (result == null)
            {
                msg = Resource.no_data_found;
            }
            else
            {
                msg = Resource.success;
            }
            return this.Ok(new
            {
                status = result == null ? false : true,
                message = msg,
                TermsConditionsData = result
            });
        }

        /// <summary>
        /// Contact Us
        /// </summary>
        /// <returns>model</returns>
        [HttpPost]
        public IHttpActionResult ContactUs(ContactUsViewModel model)
        {
            var response = userService.ContactUs(model);
            return this.Ok(new
            {
                status = response,
                message = userService.message
            });
        }


        /// <summary>
        /// Test Psuh notification Api
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult TestPushNotification(TestPushViewModel model)
        {
            try
            {  
                var response = userService.TestPushNotification(model);
                return this.Ok(new
                {
                    status = response,
                    message = userService.message
                });
            }
            catch (Exception ex)
            {
                return this.Ok(new
                {
                    Status = false,
                    Message = ex.Message
                });
            }
        }
    }
}
