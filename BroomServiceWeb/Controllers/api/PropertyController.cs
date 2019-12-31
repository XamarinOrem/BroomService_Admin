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
    public class PropertyController : ApiController
    {
        PropertyService propertyService;
        CategoryService categoryService;
        public PropertyController()
        {
            propertyService = new PropertyService();
            categoryService = new CategoryService();
        }


        /// <summary>
        /// Get Categories api
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetCategories()
        {
            var result = categoryService.GetCatSubCategoriesApp();
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
        /// Get Categories api
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetSubCategoriesByCatId(long catId)
        {
            var result = categoryService.GetSubCategoriesByCatId(catId);
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
        /// Add Update Property Api
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult AddUpdateProperty(PropertyModel model)
        {
            if (model == null)
            {
                return this.Ok(new
                {
                    status = false,
                    message = Resource.fill_required_records,
                });
            } 
            var status = propertyService.AddUpdateProperty(model);

            return this.Ok(new
            {
                status = status,
                message = propertyService.message, 
            });
        }
        /// <summary>
        /// Get Property by UserId Api
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetPropertiesByUserId(long userId)
        {
            var result = propertyService.GetPropertiesByUserId(userId);
            return this.Ok(new
            {
                status = result == null ? false : true,
                message = propertyService.message,
                data = result
            });
        }
         /// <summary>
         /// 
         /// </summary>
         /// <returns></returns>

        [HttpPost]
        public IHttpActionResult AddJobRequest()
        {
            JobRequestModel model = new JobRequestModel();

            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                model.ReferenceImages = new List<string>();
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


                            var path = "~/Images/JobRequest/";

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

                            model.ReferenceImages.Add(imagePath);

                        }
                    }
                }
            }
            model.UserId = Convert.ToInt64(httpRequest.Form.Get("UserId"));
            if (httpRequest.Form.Get("CheckList") != null)
            {
                var checklist = httpRequest.Form.Get("CheckList").ToString();
                model.CheckList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(checklist);
            }
            model.PropertyId = Convert.ToInt64(httpRequest.Form.Get("PropertyId"));
            model.CategoryId = Convert.ToInt64(httpRequest.Form.Get("CategoryId"));
            if (httpRequest.Form.Get("JobStartDatetime") != null)
            {
                model.JobStartDatetime = Convert.ToDateTime(httpRequest.Form.Get("JobStartDatetime"));
            }
            if (httpRequest.Form.Get("JobEndDatetime") != null)
            {
                model.JobEndDatetime = Convert.ToDateTime(httpRequest.Form.Get("JobEndDatetime"));
            }
            if (httpRequest.Form.Get("Price") != null)
            {
                model.Price = Convert.ToDecimal(httpRequest.Form.Get("Price"));
            } 
            if (httpRequest.Form.Get("ClientPrice") != null)
            {
                model.ClientPrice = Convert.ToDecimal(httpRequest.Form.Get("ClientPrice"));
            }
            if (httpRequest.Form.Get("HasPrice") != null)
            {
                model.HasPrice = Convert.ToBoolean(httpRequest.Form.Get("HasPrice"));
            }
            model.Description = httpRequest.Form.Get("Description");
            model.SubCategoryId = Convert.ToInt64(httpRequest.Form.Get("SubCategoryId"));
            var subsubCat= httpRequest.Form.Get("SubSubCategories");
            List<long> lst = new List<long>();
            if (!string.IsNullOrEmpty(subsubCat))
            {
                var subsubCatArr = subsubCat.Split(',');
                for (int i = 0; i < subsubCatArr.Length; i++)
                {
                    var id = Convert.ToInt64(subsubCatArr[i]);
                    lst.Add(id);
                }
            }
            model.SubSubCategories = lst;
            var status = propertyService.AddJobRequest(model);

            return this.Ok(new
            {
                status = status,
                message = propertyService.message
            });
        }
       
        /// <summary>
        /// Get Customer JobRequests Api
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetCustomerJobRequests(long userId)
        {
            var result = propertyService.GetCustomerJobRequests(userId);
            return this.Ok(new
            {
                status = result == null ? false : true,
                message = propertyService.message,
                data = result
            });
        }
        /// <summary>
        /// Get JobRequests Api
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetJobRequests(long userId)
        {
            var result = propertyService.GetJobRequests(userId);
            return this.Ok(new
            {
                status = result == null ? false : true,
                message = propertyService.message,
                data = result
            });
        }
        /// <summary>
        /// Get JobRequest Detail Api
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetJobRequestDetail(long requestId,long userId)
        {
            var result = propertyService.GetJobRequestDetail(requestId,userId);
            return this.Ok(new
            {
                status = result == null ? false : true,
                message = propertyService.message,
                data = result
            });
        }
        /// <summary>
        /// Add ChatRequest Api
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult AddChatRequest(ChatRequestModel model)
        {
            if (model == null)
            {
                return this.Ok(new
                {
                    status = false,
                    message = Resource.fill_required_records,
                });
            } 
            var status = propertyService.AddChatRequest(model);

            return this.Ok(new
            {
                status = status,
                message = propertyService.message, 
            });
        }
        /// <summary>
        /// Get Customer JobRequests Api
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetChat(long userId)
        {
            var result = propertyService.GetChat(userId);
            return this.Ok(new
            {
                status = result == null ? false : true,
                message = propertyService.message,
                data = result
            });
        }

        /// <summary>
        /// Get Notifications
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetNotifications(long userId)
        {
            var result = propertyService.GetNotifications(userId);
            return this.Ok(new 
            {
                status = result == null ? false : true,
                message = propertyService.message,
                data = result
            });
        }


        /// <summary>
        /// Accept Reject Request Api
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult AcceptRejectRequest(AcceptRejectJobReqModel model  )
        { 
            var status = propertyService.AcceptRejectRequest(model );

            return this.Ok(new
            {
                status = status,
                message = propertyService.message,
            });
        }

        /// <summary>
        /// Send Quote
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult SendQuote(QuotePriceModel model)
        {
            if (model == null)
            {
                return this.Ok(new
                {
                    status = false,
                    message = Resource.fill_required_records,
                });
            }
            var status = propertyService.SendQuote(model);

            return this.Ok(new
            {
                status = status,
                message = propertyService.message,
            });
        }

        /// <summary>
        /// Accept Reject Quote Api
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult AcceptRejectQuote(AcceptRejectQuoteModel model)
        {
            var status = propertyService.AcceptRejectQuote(model);

            return this.Ok(new
            {
                status = status,
                message = propertyService.message,
            });
        }
        /// <summary>
        /// Start End Timer api
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult StartEndTimer(UpdateTimerTimeModel model)
        {
            if (model == null)
            {
                return this.Ok(new
                {
                    status = false,
                    message = Resource.fill_required_records,
                });
            }
            var status = propertyService.StartEndTimer(model);

            return this.Ok(new
            {
                status = status,
                message = propertyService.message,
            });
        }


        /// <summary>
        /// Complete Job Request api
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult CompleteJobRequest(CompleteJobRequestModel model)
        {
            if (model == null)
            {
                return this.Ok(new
                {
                    status = false,
                    message = Resource.fill_required_records,
                });
            }
            var status = propertyService.CompleteJobRequest(model);

            return this.Ok(new
            {
                status = status,
                message = propertyService.message,
            });
        }
        /// <summary>
        /// Submit User review
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult SubmitUserReview(UserReviewModel model)
        {
            if (model == null)
            {
                return this.Ok(new
                {
                    status = false,
                    message = Resource.fill_required_records,
                });
            }
            var status = propertyService.SubmitUserReview(model);

            return this.Ok(new
            {
                status = status,
                message = propertyService.message,
            });
        }
    }
}
