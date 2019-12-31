using BroomServiceWeb.Helpers;
using BroomServiceWeb.Models;
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
    public class BookingsController : MyController
    {
        CategoryService categoryService;
        PropertyService propertyService;

        public BookingsController()
        {
            propertyService = new PropertyService();
            categoryService = new CategoryService();
        }

        #region My Bookings
        [WebAuthAttribute]
        // GET: Bookies
        public ActionResult MyBookings()
        {
            var userId = Convert.ToInt64(Session["UserId"]);
            if (userId != 0)
            {
                List<JobRequestViewModel> result = null;
                if ((int)Session["UserType"] == Enums.UserTypeEnum.Customer.GetHashCode())
                {
                    result = propertyService.GetCustomerJobRequests(userId);
                    if (result != null)
                    {
                        if (result.Count > 0)
                        {
                            result = result.OrderByDescending(a => a.Id).ToList();
                        }
                    }
                }
                else
                {
                    result = propertyService.GetJobRequests(userId);
                    if (result != null)
                    {
                        if (result.Count > 0)
                        {
                            result = result.OrderByDescending(a => a.Id).ToList();
                        }
                    }
                }
                return View(result);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }

        #endregion

        #region Job Details
        [WebAuthAttribute]
        public ActionResult JobDetails(long jobId)
        {
            var userId = Convert.ToInt64(Session["UserId"]);
            if (userId != 0)
            {
                var result = propertyService.GetJobRequestDetail(jobId, userId);
                var culVal = System.Globalization.CultureInfo.CurrentCulture.Name;

                result.Category.Name = culVal == "fr-FR" ? result.Category.Name_French
                    : culVal == "ru-RU" ? result.Category.Name_Russian
                    : culVal == "he-IL" ? result.Category.Name_Hebrew
                    : result.Category.Name;

                result.SubCategory.Name = culVal == "fr-FR" ? result.SubCategory.Name_French
                   : culVal == "ru-RU" ? result.SubCategory.Name_Russian
                   : culVal == "he-IL" ? result.SubCategory.Name_Hebrew
                   : result.SubCategory.Name;

                for (int i = 0; i < result.SubSubCategories.Count; i++)
                {
                    result.SubSubCategories[i].Name = culVal == "fr-FR" ? result.SubSubCategories[i].Name_French
                       : culVal == "ru-RU" ? result.SubSubCategories[i].Name_Russian
                       : culVal == "he-IL" ? result.SubSubCategories[i].Name_Hebrew
                       : result.SubSubCategories[i].Name;
                }

                if ((int)Session["UserType"] == Enums.UserTypeEnum.Worker.GetHashCode())
                {
                    if (result.TimerEndTime != null)
                    {
                        if (result.CheckList != null)
                        {
                            if (result.CheckList.Count > 0)
                            {
                                foreach (var item in result.CheckList)
                                {
                                    item.IsDone = true;
                                }
                            }
                        }
                    }
                }
                return View(result);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        #endregion

        #region Job Request

        [WebAuthAttribute]
        public ActionResult AddJobRequest(int propertyId)
        {
            var userId = Convert.ToInt64(Session["UserId"]);
            if (userId != 0)
            {
                var categories = categoryService.GetCategories(true);
                var result = propertyService.GetPropertiesById(propertyId);
                JobRequestVM jobRequestModel = new JobRequestVM();
                var culVal = System.Globalization.CultureInfo.CurrentCulture.Name;
                for (int i = 0; i < categories.Count; i++)
                {
                    categories[i].Name = culVal == "fr-FR" ? categories[i].Name_French
                    : culVal == "ru-RU" ? categories[i].Name_Russian
                    : culVal == "he-IL" ? categories[i].Name_Hebrew
                    : categories[i].Name;

                    categories[i].Description = culVal == "fr-FR" ? categories[i].Description_French
                    : culVal == "ru-RU" ? categories[i].Description_Russian
                    : culVal == "he-IL" ? categories[i].Description_Hebrew
                    : categories[i].Description;
                }
                jobRequestModel.Categories = categories;
                JobRequestViewModel jobRequest = new JobRequestViewModel();
                jobRequest.ServiceProviderId = result.Id;
                jobRequest.PropertyType = result.Type;
                jobRequest.JobStartDatetime = DateTime.Now;
                jobRequest.JobEndDatetime = DateTime.Now;
                jobRequest.JobStartTime = DateTime.Now.ToString("HH:mm");
                jobRequest.JobEndTime = DateTime.Now.ToString("HH:mm");
                jobRequestModel.JobRequestData = jobRequest;
                return View(jobRequestModel);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        public ActionResult CategoryDetails(int id)
        {
            var categories = categoryService.GetCategoryById(id);

            var culVal = System.Globalization.CultureInfo.CurrentCulture.Name;
            categories.Name = culVal == "fr-FR" ? categories.Name_French
            : culVal == "ru-RU" ? categories.Name_Russian
            : culVal == "he-IL" ? categories.Name_Hebrew
            : categories.Name;

            categories.Description = culVal == "fr-FR" ? categories.Description_French
            : culVal == "ru-RU" ? categories.Description_Russian
            : culVal == "he-IL" ? categories.Description_Hebrew
            : categories.Description;
            return PartialView("CategoryDetails", categories);
        }

        public ActionResult SubCategoryDetails(int id)
        {
            var subCategories = categoryService.GetCategoryBySubCatId(id);

            var culVal = System.Globalization.CultureInfo.CurrentCulture.Name;
            subCategories.Name = culVal == "fr-FR" ? subCategories.Name_French
            : culVal == "ru-RU" ? subCategories.Name_Russian
            : culVal == "he-IL" ? subCategories.Name_Hebrew
            : subCategories.Name;

            subCategories.Description = culVal == "fr-FR" ? subCategories.Description_French
            : culVal == "ru-RU" ? subCategories.Description_Russian
            : culVal == "he-IL" ? subCategories.Description_Hebrew
            : subCategories.Description;
            return PartialView("SubCategoryDetails", subCategories);
        }

        public ActionResult SubSubCategoryDetails(int id)
        {
            var subSubCategories = categoryService.GetSubSubCategoryById(id);

            var culVal = System.Globalization.CultureInfo.CurrentCulture.Name;
            subSubCategories.Name = culVal == "fr-FR" ? subSubCategories.Name_French
            : culVal == "ru-RU" ? subSubCategories.Name_Russian
            : culVal == "he-IL" ? subSubCategories.Name_Hebrew
            : subSubCategories.Name;

            subSubCategories.Description = culVal == "fr-FR" ? subSubCategories.Description_French
            : culVal == "ru-RU" ? subSubCategories.Description_Russian
            : culVal == "he-IL" ? subSubCategories.Description_Hebrew
            : subSubCategories.Description;
            return PartialView("SubSubCategoryDetails", subSubCategories);
        }


        public JsonResult GetSubCategories(int categoryId)
        {
            var subCategories = categoryService.GetSubCategoriesByCatId(categoryId);
            if (subCategories.Count > 0)
            {
                var culVal = System.Globalization.CultureInfo.CurrentCulture.Name;
                for (int i = 0; i < subCategories.Count; i++)
                {
                    subCategories[i].Name = culVal == "fr-FR" ? subCategories[i].Name_French
                    : culVal == "ru-RU" ? subCategories[i].Name_Russian
                    : culVal == "he-IL" ? subCategories[i].Name_Hebrew
                    : subCategories[i].Name;

                    subCategories[i].Description = culVal == "fr-FR" ? subCategories[i].Description_French
                    : culVal == "ru-RU" ? subCategories[i].Description_Russian
                    : culVal == "he-IL" ? subCategories[i].Description_Hebrew
                    : subCategories[i].Description;
                }
            }
            return Json(subCategories, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetSubSubCategories(int subCategoryId)
        {
            var subCategories = categoryService.GetSubSubCategoriesByID(subCategoryId);
            if (subCategories.Count > 0)
            {
                var culVal = System.Globalization.CultureInfo.CurrentCulture.Name;
                for (int i = 0; i < subCategories.Count; i++)
                {
                    subCategories[i].Name = culVal == "fr-FR" ? subCategories[i].Name_French
                    : culVal == "ru-RU" ? subCategories[i].Name_Russian
                    : culVal == "he-IL" ? subCategories[i].Name_Hebrew
                    : subCategories[i].Name;

                    subCategories[i].Description = culVal == "fr-FR" ? subCategories[i].Description_French
                   : culVal == "ru-RU" ? subCategories[i].Description_Russian
                   : culVal == "he-IL" ? subCategories[i].Description_Hebrew
                   : subCategories[i].Description;
                }
            }
            return Json(subCategories, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult AddJobRequest(FormCollection formCollection)
        {
            JobRequestModel model = new JobRequestModel();
            model.CategoryId = Convert.ToInt32(formCollection["CategoryId"]);
            model.PropertyId = Convert.ToInt32(formCollection["PropertyId"]);
            if (formCollection["JobStartDatetime"] != null && formCollection["JobStartTime"] != null)
            {
                var getDate = DateTime.ParseExact(formCollection["JobStartDatetime"], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                //var getDate = Convert.ToDateTime(formCollection["JobStartDatetime"]);
                TimeSpan time;
                if (TimeSpan.TryParse(formCollection["JobStartTime"], out time))
                {
                    string startDate = (getDate + time).ToString("yyyy-MM-dd HH:mm:ss");
                    model.JobStartDatetime = Convert.ToDateTime(startDate);
                }
            }
            if (formCollection["Price"] != "null" || formCollection["ClientPrice"] != "null")
            {
                model.HasPrice = true;
            }
            else
            {
                model.HasPrice = false;
            }
            if (formCollection["JobEndDatetime"] != null && formCollection["JobEndTime"] != null)
            {
                var getDate = DateTime.ParseExact(formCollection["JobEndDatetime"], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                //var getDate = Convert.ToDateTime(formCollection["JobEndDatetime"]);
                TimeSpan time;
                if (TimeSpan.TryParse(formCollection["JobEndTime"], out time))
                {
                    string endDate = (getDate + time).ToString("yyyy-MM-dd HH:mm:ss");
                    model.JobEndDatetime = Convert.ToDateTime(endDate);
                }
            }

            if (formCollection["CheckList"] != null)
            {
                var checklist = formCollection["CheckList"].ToString();
                model.CheckList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(checklist);
            }

            model.Description = formCollection["Description"];
            model.UserId = Convert.ToInt64(Session["UserId"]);
            if (formCollection["SubCategoryId"] != "undefined")
            {
                model.SubCategoryId = Convert.ToInt64(formCollection["SubCategoryId"]);
            }

            if (formCollection["Price"] != "null")
            {
                model.Price = Convert.ToDecimal(formCollection["Price"]);
            }
            if (formCollection["ClientPrice"] != "null")
            {
                model.ClientPrice = Convert.ToDecimal(formCollection["ClientPrice"]);
            }

            var json = formCollection["Attachments"].ToString();
            List<string> getAttachments = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(json);


            model.ReferenceImages = new List<string>();
            var date = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss").Replace("-", "_");

            if (getAttachments.Count > 0)
            {
                var count = getAttachments.Count();
                if (count > 0)
                {
                    for (int i = 0; i < getAttachments.Count(); i++)
                    {
                        string imageName = "JobRequest" + Guid.NewGuid() + date + ".jpg";

                        string[] splitAttachment = getAttachments[i].Split(',');

                        Common.SaveImage(splitAttachment[1], imageName);

                        model.ReferenceImages.Add(imageName);

                    }
                }
            }


            if (formCollection["SubSubCategories"] != null)
            {
                var subsubCatArr = formCollection["SubSubCategories"].ToString();
                model.SubSubCategories = Newtonsoft.Json.JsonConvert.DeserializeObject<List<long>>(subsubCatArr);
            }
            var status = propertyService.AddJobRequest(model);


            wsBase obj = new wsBase()
            {
                status = status,
                message = propertyService.message
            };
            return Json(obj, JsonRequestBehavior.AllowGet);

        }

        #endregion

        #region Start Job

        [WebAuthAttribute]
        public JsonResult StartJob(UpdateTimerTimeModel model)
        {
            var userId = Convert.ToInt64(Session["UserId"]);
            if (userId != 0)
            {
                model.UserId = userId;
                model.TimerTime = DateTime.Now.TimeOfDay;
                var status = propertyService.StartEndTimer(model);
                return Json(status, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Session Expired", JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Complete Job

        [WebAuthAttribute]
        public JsonResult CompleteJob(CompleteJobRequestModel model)
        {
            var userId = Convert.ToInt64(Session["UserId"]);
            if (userId != 0)
            {
                model.UserId = userId;
                var status = propertyService.CompleteJobRequest(model);
                return Json(status, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Session Expired", JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Send Quote

        [WebAuthAttribute]
        public JsonResult SendQuote(QuotePriceModel model)
        {
            var userId = Convert.ToInt64(Session["UserId"]);
            if (userId != 0)
            {
                model.UserId = userId;
                var status = propertyService.SendQuote(model);
                return Json(status, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Session Expired", JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Submit User Review
        [WebAuthAttribute]
        [HttpPost]
        public JsonResult SubmitUserReview(UserReviewModel model)
        {
            var userId = Convert.ToInt64(Session["UserId"]);
            if (userId != 0)
            {
                model.CustomerId = userId;
                var status = propertyService.SubmitUserReview(model);
                return Json(status, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Session Expired", JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}