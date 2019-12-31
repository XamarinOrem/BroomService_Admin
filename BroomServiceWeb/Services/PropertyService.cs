using BroomServiceWeb.Helpers;
using BroomServiceWeb.Models;
using BroomServiceWeb.Resources;
using BroomServiceWeb.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BroomServiceWeb.Services
{
    public class PropertyService
    {
        BroomServiceEntities _db;
        UserService userService;
        public PropertyService()
        {
            _db = new BroomServiceEntities();
            userService = new UserService();
        }
        public string message;
        public bool AddUpdateProperty(PropertyModel model)
        {
            bool status = false;
            try
            {
                if (model.Id != null && model.Id != 0)
                {

                    var result = _db.Properties.Where(x => x.Id == model.Id).FirstOrDefault();
                    if (result != null)
                    {
                        result.AccessToProperty = model.AccessToProperty;
                        result.ApartmentNumber = model.ApartmentNumber;
                        result.Balcony = model.Balcony;
                        result.BuildingCode = model.BuildingCode;
                        result.CreatedBy = model.CreatedBy;
                        result.Dishwasher = model.Dishwasher;
                        result.Doorman = model.Doorman;
                        result.DuvetSize = model.DuvetSize;
                        result.Elevator = model.Elevator;
                        result.FloorNumber = model.FloorNumber;
                        result.Garden = model.Garden;
                        result.ModifiedDate = DateTime.Now;
                        result.Name = model.Name;
                        result.NoOfBathrooms = model.NoOfBathrooms;
                        result.NoOfDoubleBeds = model.NoOfDoubleBeds;
                        result.NoOfDoubleSofaBeds = model.NoOfDoubleSofaBeds;
                        result.NoOfDuvet = model.NoOfDuvet;
                        result.NoOfPillows = model.NoOfPillows;
                        result.NoOfToilets = model.NoOfToilets;
                        result.NoOfBedRooms = model.NoOfBedRooms;
                        result.Size = model.Size;
                        result.NoOfQueenBeds = model.NoOfQueenBeds;
                        result.NoOfSingleBeds = model.NoOfSingleBeds;
                        result.NoOfSingleSofaBeds = model.NoOfSingleSofaBeds;
                        result.Parking = model.Parking;
                        result.Pool = model.Pool;
                        result.ShortTermApartment = model.ShortTermApartment;
                        result.Type = model.Type;
                        result.Address = model.Address;
                        result.Latitude = model.Latitude;
                        result.Longitude = model.Longitude;
                        _db.SaveChanges();
                        message = Resource.property_update_success;
                        status = true;
                    }
                    else
                    {
                        message = Resource.property_not_exists;
                    }
                }
                else
                {
                    Property property = new Property()
                    {
                        AccessToProperty = model.AccessToProperty,
                        ApartmentNumber = model.ApartmentNumber,
                        Balcony = model.Balcony,
                        BuildingCode = model.BuildingCode,
                        CreatedBy = model.CreatedBy,
                        CreatedDate = DateTime.Now,
                        Dishwasher = model.Dishwasher,
                        Doorman = model.Doorman,
                        DuvetSize = model.DuvetSize,
                        Elevator = model.Elevator,
                        FloorNumber = model.FloorNumber,
                        Garden = model.Garden,
                        IsActive = true,
                        Name = model.Name,
                        NoOfBathrooms = model.NoOfBathrooms,
                        NoOfDoubleBeds = model.NoOfDoubleBeds,
                        NoOfDoubleSofaBeds = model.NoOfDoubleSofaBeds,
                        NoOfDuvet = model.NoOfDuvet,
                        NoOfPillows = model.NoOfPillows,
                        NoOfBedRooms = model.NoOfBedRooms,
                        NoOfToilets = model.NoOfToilets,
                        Size = model.Size,
                        NoOfQueenBeds = model.NoOfQueenBeds,
                        NoOfSingleBeds = model.NoOfSingleBeds,
                        NoOfSingleSofaBeds = model.NoOfSingleSofaBeds,
                        Parking = model.Parking,
                        Pool = model.Pool,
                        ShortTermApartment = model.ShortTermApartment,
                        Type = model.Type,
                        Address = model.Address,
                        Latitude = model.Latitude,
                        Longitude = model.Longitude
                    };
                    _db.Properties.Add(property);
                    _db.SaveChanges();
                    message = Resource.property_add_success;
                    status = true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }
        public bool AcceptRejectRequest(AcceptRejectJobReqModel model)
        {
            bool status = false;
            try
            {
                var jobReq = _db.JobRequests.Where(x => x.Id == model.JobRequestId
                && x.JobStatus == (int)Enums.RequestStatus.Pending).FirstOrDefault();
                if (jobReq != null)
                {
                    if (model.IsAccept)
                    {
                        jobReq.JobStatus = (int)Enums.RequestStatus.InProgress;
                        string userName = string.Empty;

                        jobReq.ServiceProviderId = model.UserId;
                        var userData = userService.GetUserDetail(model.UserId);
                        userName = userData.FirstName + " " + userData.LastName;


                        string msgText = " has accepted your job request.";
                        var propUserData = jobReq.Property.User;
                        Notification notification = new Notification()
                        {
                            CreatedDate = DateTime.Now,
                            FromUserId = model.UserId,
                            ToUserId = propUserData.UserId,
                            IsActive = true,
                            NotificationStatus = (int)Enums.NotificationStatus.Accepted,
                            Text = msgText,
                            JobRequestId = jobReq.Id
                        };
                        _db.Notifications.Add(notification);
                        Common.PushNotification(propUserData.UserType, propUserData.DeviceId, propUserData.DeviceToken,
                               userName + msgText);


                        msgText = "You have accepted " + propUserData.FirstName + " " + propUserData.LastName + "'s job request.";
                        Notification notification1 = new Notification()
                        {
                            CreatedDate = DateTime.Now,
                            FromUserId = propUserData.UserId,
                            ToUserId = model.UserId,
                            IsActive = true,
                            NotificationStatus = (int)Enums.NotificationStatus.Accepted,
                            Text = msgText,
                            JobRequestId = jobReq.Id
                        };
                        _db.Notifications.Add(notification1);

                        _db.SaveChanges();

                        status = true;
                        message = Resource.job_request_accept_success;
                    }

                    else
                    {
                        var rejectData = _db.RejectJobRequests.Where(x => x.UserId == model.UserId &&
                        x.JobRequestId == model.JobRequestId).FirstOrDefault();
                        if (rejectData == null)
                        {
                            RejectJobRequest rejectJobRequest = new RejectJobRequest();
                            rejectJobRequest.CreatedDate = DateTime.Now;
                            rejectJobRequest.JobRequestId = model.JobRequestId;
                            rejectJobRequest.UserId = model.UserId;
                            _db.RejectJobRequests.Add(rejectJobRequest);

                            var propUserData = jobReq.Property.User;
                            string msgText = "You have rejected " + propUserData.FirstName + " "
                                + propUserData.LastName + "'s job request.";
                            Notification notification1 = new Notification()
                            {
                                CreatedDate = DateTime.Now,
                                FromUserId = propUserData.UserId,
                                ToUserId = model.UserId,
                                IsActive = true,
                                NotificationStatus = (int)Enums.NotificationStatus.Rejected,
                                Text = msgText,
                                JobRequestId = jobReq.Id
                            };
                            _db.Notifications.Add(notification1);

                            _db.SaveChanges();
                            status = true;
                            message = Resource.job_request_reject_success;
                        }
                        else
                        {
                            message = Resource.already_reject_job;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }

        public List<PropertyModel> GetPropertiesByUserId(long userId)
        {
            List<PropertyModel> lstData = null;
            try
            {
                var user = _db.Users.Where(x => x.UserId == userId).FirstOrDefault();
                if (user != null)
                {
                    lstData = _db.Properties.Where(x => x.CreatedBy == userId && x.IsActive == true).Select
                        (x => new PropertyModel()
                        {
                            AccessToProperty = x.AccessToProperty,
                            ApartmentNumber = x.ApartmentNumber,
                            Balcony = x.Balcony,
                            BuildingCode = x.BuildingCode,
                            CreatedBy = x.CreatedBy,
                            CreatedDate = x.CreatedDate,
                            Dishwasher = x.Dishwasher,
                            Doorman = x.Doorman,
                            DuvetSize = x.DuvetSize,
                            Elevator = x.Elevator,
                            FloorNumber = x.FloorNumber,
                            Garden = x.Garden,
                            Id = x.Id,
                            Name = x.Name,
                            NoOfBathrooms = x.NoOfBathrooms,
                            NoOfDoubleBeds = x.NoOfDoubleBeds,
                            NoOfDoubleSofaBeds = x.NoOfDoubleSofaBeds,
                            NoOfDuvet = x.NoOfDuvet,
                            NoOfPillows = x.NoOfPillows,
                            NoOfBedRooms = x.NoOfBedRooms,
                            NoOfToilets = x.NoOfToilets,
                            Size = x.Size,
                            NoOfQueenBeds = x.NoOfQueenBeds,
                            NoOfSingleBeds = x.NoOfSingleBeds,
                            NoOfSingleSofaBeds = x.NoOfSingleSofaBeds,
                            Parking = x.Parking,
                            Pool = x.Pool,
                            ShortTermApartment = x.ShortTermApartment,
                            Type = x.Type,
                            Address = x.Address
                        }).ToList();

                    message = Resource.success;
                }
                else
                {
                    message = Resource.user_not_exist;
                }
            }
            catch (Exception ex)
            {
                lstData = null;
            }
            return lstData;
        }



        public bool AddJobRequest(JobRequestModel model)
        {
            bool status = false;
            try
            {
                JobRequest request = new JobRequest()
                {
                    CategoryId = model.CategoryId,
                    SubCategoryId = model.SubCategoryId,
                    Description = model.Description,
                    QuotePrice = model.Price,
                    ClientQuotePrice = model.ClientPrice,
                    JobStartDatetime = model.JobStartDatetime,
                    JobEndDatetime = model.JobEndDatetime,
                    PropertyId = model.PropertyId,
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    JobStatus = (int)Enums.RequestStatus.Pending,
                    HasPrice = model.HasPrice
                };
                _db.JobRequests.Add(request);
                _db.SaveChanges();
                if (model.CheckList != null)
                {
                    foreach (var item in model.CheckList)
                    {
                        JobRequestCheckList task = new JobRequestCheckList();
                        task.JobRequestId = request.Id;
                        task.TaskDetail = item.ToString();
                        _db.JobRequestCheckLists.Add(task);
                    }
                }
                if (model.ReferenceImages != null)
                {
                    foreach (var item in model.ReferenceImages)
                    {
                        JobRequestRefImage jobRequestRefImage = new JobRequestRefImage();
                        jobRequestRefImage.JobRequestId = request.Id;
                        jobRequestRefImage.PicturePath = item.ToString();
                        _db.JobRequestRefImages.Add(jobRequestRefImage);
                    }
                }
                if (model.SubSubCategories != null && model.SubSubCategories.Count > 0)
                {
                    foreach (var item in model.SubSubCategories)
                    {
                        var id = Convert.ToInt64(item);
                        JobRequestSubSubCategory jobRequestSubCategory =
                            new JobRequestSubSubCategory();
                        jobRequestSubCategory.JobRequestId = request.Id;
                        jobRequestSubCategory.SubSubCategoryId = id;
                        _db.JobRequestSubSubCategories.Add(jobRequestSubCategory);

                        ///// send requests to service providers
                        //var serviceProviders = _db.Users.Where(x => x.IsActive == true && x.EmailVerified == true
                        //&& x.UserType == (int)Enums.UserTypeEnum.ServiceProvider && x.UserSubCategories
                        //.Select(y => y.SubCategoryId).Contains(subCategoryId)).ToList();
                    }
                }


                //notification 
                string msgText = " has sent job request.";
                Notification notification = new Notification()
                {
                    CreatedDate = DateTime.Now,
                    FromUserId = model.UserId,
                    IsActive = true,
                    NotificationStatus = (int)Enums.NotificationStatus.Pending,
                    Text = msgText,
                    JobRequestId = request.Id
                };
                _db.Notifications.Add(notification);
                var catData = _db.Categories.Where(x => x.Id == model.CategoryId).FirstOrDefault();
                if (catData != null)
                {
                    if (catData.ForWorkers == false)
                    {
                        var userIds = new List<long?>();
                        if (model.SubSubCategories != null && model.SubSubCategories.Count > 0)
                        {
                            userIds = _db.UserSubCategories.Where(x => model.SubSubCategories.Contains(x.SubSubCategoryId ?? 0))
                                .Select(y => y.UserId).ToList();
                        }
                        else
                        {
                            userIds = _db.UserSubCategories.Where(x => x.SubCategoryId == model.SubCategoryId)
                                .Select(y => y.UserId).ToList();
                        }
                        var users = _db.Users.Where(x => x.IsActive == true && x.EmailVerified == true &&
                          x.UserType == (int)Enums.UserTypeEnum.ServiceProvider && userIds.Contains(x.UserId)).ToList();

                        foreach (var item in users)
                        {
                            if (item.DeviceId != 0 && item.DeviceId != null && !string.IsNullOrEmpty(item.DeviceToken))
                            {
                                var propUserData = userService.GetUserDetail(model.UserId ?? 0);
                                Common.PushNotification(item.UserType, item.DeviceId, item.DeviceToken,
                                    propUserData.FirstName + " " + propUserData.LastName + msgText);
                            }
                        }
                    }
                    else
                    {
                        msgText = " has sent job request.";
                        Notification notification1 = new Notification()
                        {
                            CreatedDate = DateTime.Now,
                            FromUserId = model.UserId,
                            ToUserId = userService.GetAdminId(),
                            IsActive = true,
                            NotificationStatus = (int)Enums.NotificationStatus.Pending,
                            Text = msgText,
                            JobRequestId = request.Id
                        };
                        _db.Notifications.Add(notification1);
                    }
                }
                _db.SaveChanges();
                if (model.HasPrice == true)
                {
                    message = Resource.job_request_add_success;
                }
                else
                {
                    message = Resource.get_back_with_quote;
                }
                status = true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }


        public int? GetUserRating(long? customerId, long? toUserId)
        {
            int? rating = null;
            try
            {
                var review = _db.UserReviews.Where(x => x.CustomerId == customerId && x.ToUserId == toUserId).FirstOrDefault();
                if (review != null)
                {
                    rating = review.Rating;
                }
            }
            catch
            {

            }
            return rating;
        }

        public string GetUserReview(long? customerId, long? toUserId)
        {
            string _review = string.Empty;
            try
            {
                var review = _db.UserReviews.Where(x => x.CustomerId == customerId && x.ToUserId == toUserId).FirstOrDefault();
                if (review != null)
                {
                    _review = review.Review;
                }
            }
            catch
            {

            }
            return _review;
        }


        public int? GetUserJobRating(long? customerId, long? toUserId, long? jobRequestId)
        {
            int? rating = null;
            try
            {
                var review = _db.UserJobReviews.Where(x => x.CustomerId == customerId && x.ToUserId == toUserId
                && x.JobRequestId == jobRequestId).FirstOrDefault();
                if (review != null)
                {
                    rating = review.Rating;
                }
            }
            catch
            {

            }
            return rating;
        }

        public string GetUserJobReview(long? customerId, long? toUserId, long? jobRequestId)
        {
            string _review = string.Empty;
            try
            {
                var review = _db.UserJobReviews.Where(x => x.CustomerId == customerId && x.ToUserId == toUserId
                && x.JobRequestId == jobRequestId).FirstOrDefault();
                if (review != null)
                {
                    _review = review.Review;
                }
            }
            catch
            {

            }
            return _review;
        }

        public List<JobRequestViewModel> GetCustomerJobRequests(long userId)
        {
            List<JobRequestViewModel> lstData = null;
            try
            {
                var AdminId = userService.GetAdminId();
                lstData = _db.JobRequests.Where(x => x.Property.CreatedBy == userId && x.IsActive == true)
                    .Select
                    (x => new JobRequestViewModel()
                    {
                        Id = x.Id,
                        IsShownQuote = x.HasPrice == true ? false : true,
                        IsQuoteApproved = x.IsQuoteApproved,
                        QuotePrice = x.QuotePrice,
                        TimerEndTime = x.TimerEndTime,
                        TimerStartTime = x.TimerStartTime,
                        PropertyName = x.Property.Name,
                        PropertyAddress = x.Property.Address,
                        PropertyLatitude = x.Property.Latitude,
                        PropertyLongitude = x.Property.Longitude,
                        PropertyApartmentNumber = x.Property.ApartmentNumber,
                        PropertyFloorNumber = x.Property.FloorNumber,
                        PropertyBuildingCode = x.Property.BuildingCode,
                        PropertyType = x.Property.Type,
                        PropertyId = x.Property.Id,
                        Category = new CategoryViewModelApp()
                        {
                            Id = x.Category.Id,
                            Name = x.Category.Name,
                            Name_French = x.Category.Name_French,
                            Name_Hebrew = x.Category.Name_Hebrew,
                            Name_Russian = x.Category.Name_Russian,
                            Icon = x.Category.Icon,
                            Picture = x.Category.Picture,
                            //HasPrice = x.Category.HasPrice ?? false,
                        },
                        SubCategory = new SubCategoryViewModelApp()
                        {
                            Id = x.SubCategory.Id,
                            Name = x.SubCategory.Name,
                            Name_French = x.SubCategory.Name_French,
                            Name_Hebrew = x.SubCategory.Name_Hebrew,
                            Name_Russian = x.SubCategory.Name_Russian,
                            Price = x.SubCategory.Price,
                            ClientPrice = x.SubCategory.ClientPrice,
                            Icon = x.SubCategory.Icon,
                            Picture = x.SubCategory.Picture
                        },
                        Description = x.Description,
                        CustomerId = x.Property.User.UserId,
                        CustomerName = x.Property.User.FirstName + " " + x.Property.User.LastName,
                        CustomerImage = x.Property.User.PicturePath,
                        JobStartDatetime = x.JobStartDatetime,
                        JobEndDatetime = x.JobEndDatetime,
                        ServiceProviderId = x.User != null ? x.User.UserId : 0,
                        ServiceProviderName = x.User != null ? x.User.FirstName + " " + x.User.LastName : "",
                        ServiceProviderImage = x.User != null ? x.User.PicturePath : "",
                        ServiceProviderProfilePic = x.User.PicturePath,
                        ReferenceImages = x.JobRequestRefImages.Select(y => y.PicturePath).ToList(),
                        SubSubCategories = x.JobRequestSubSubCategories.Select(z => new SubSubCategoryViewModelApp()
                        {
                            Id = z.SubSubCategory.Id,
                            Name = z.SubSubCategory.Name,
                            Name_French = z.SubSubCategory.Name_French,
                            Name_Hebrew = z.SubSubCategory.Name_Hebrew,
                            Name_Russian = z.SubSubCategory.Name_Russian,
                            Price = z.SubSubCategory.Price,
                            ClientPrice = z.SubSubCategory.ClientPrice,
                            Icon = z.SubSubCategory.Icon,
                            Picture = z.SubSubCategory.Picture,
                        }).ToList(),
                        CheckList = x.JobRequestCheckLists.Select(p => new JobRequestCheckListModel()
                        {
                            Id = p.Id,
                            TaskDetail = p.TaskDetail,
                            IsDone = x.JobStatus == (int)Enums.RequestStatus.Completed ? true : false
                        }).ToList(),
                        JobStatus = x.JobStatus ?? (int)Enums.RequestStatus.InProgress,
                        PaymentMethod = x.Property.User.PaymentMethod,
                        IsPaymentDone = x.IsPaymentDone ?? false,
                        AdminId = AdminId,
                        CustomerQuotePrice = x.ClientQuotePrice,
                    }).ToList();



                message = Resource.success;
            }
            catch (Exception ex)
            {
                lstData = null;
                message = ex.Message;
            }
            return lstData;
        }
        #region Worker Orders in Admin section
        public bool AcceptRejectRequestAdmin(AcceptRejectJobReqAdminModel model)
        {
            bool status = false;
            try
            {
                var jobReq = _db.JobRequests.Where(x => x.Id == model.JobRequestId
                && x.JobStatus == (int)Enums.RequestStatus.Pending).FirstOrDefault();
                if (jobReq != null)
                {
                    if (model.IsAccept)
                    {
                        jobReq.JobStatus = (int)Enums.RequestStatus.InProgress;
                        string userName = string.Empty;
                        userName = Resource.broom_service + " ";


                        string msgText = " has accepted your job request.";
                        var propUserData = jobReq.Property.User;
                        Notification notification = new Notification()
                        {
                            CreatedDate = DateTime.Now,
                            FromUserId = model.UserId,
                            ToUserId = propUserData.UserId,
                            IsActive = true,
                            NotificationStatus = (int)Enums.NotificationStatus.Accepted,
                            Text = msgText,
                            JobRequestId = jobReq.Id
                        };
                        _db.Notifications.Add(notification);
                        Common.PushNotification(propUserData.UserType, propUserData.DeviceId, propUserData.DeviceToken,
                               userName + msgText);


                        if (model.NotificationId != 0)
                        {
                            msgText = " You have accepted" + propUserData.FirstName + " " + propUserData.LastName + "'s job request.";

                            var notData = _db.Notifications.Where(x => x.Id == model.NotificationId).FirstOrDefault();

                            if (notData != null)
                            {

                                notData.NotificationStatus = (int)Enums.NotificationStatus.Accepted;
                                notData.Text = msgText;
                            }
                        }



                        _db.SaveChanges();

                        status = true;
                        message = Resource.job_request_accept_success;
                    }

                    else
                    {
                        var rejectData = _db.RejectJobRequests.Where(x => x.UserId == model.UserId &&
                        x.JobRequestId == model.JobRequestId).FirstOrDefault();
                        if (rejectData == null)
                        {
                            jobReq.JobStatus = (int)Enums.RequestStatus.Canceled;

                            RejectJobRequest rejectJobRequest = new RejectJobRequest();
                            rejectJobRequest.CreatedDate = DateTime.Now;
                            rejectJobRequest.JobRequestId = model.JobRequestId;
                            rejectJobRequest.UserId = model.UserId;
                            _db.RejectJobRequests.Add(rejectJobRequest);

                            var propUserData = jobReq.Property.User;
                            string msgText = " You have rejected" + propUserData.FirstName + " "
                                + propUserData.LastName + "'s job request.";



                            if (model.NotificationId != 0)
                            {

                                var notData = _db.Notifications.Where(x => x.Id == model.NotificationId).FirstOrDefault();

                                if (notData != null)
                                {

                                    notData.NotificationStatus = (int)Enums.NotificationStatus.Rejected;
                                    notData.Text = msgText;
                                }
                            }
                            msgText = "Broom Service has rejected your " + jobReq.Property.Name + "'s job request.";

                            Notification notification1 = new Notification()
                            {
                                CreatedDate = DateTime.Now,
                                FromUserId = model.UserId,
                                ToUserId = propUserData.UserId,
                                IsActive = true,
                                NotificationStatus = (int)Enums.NotificationStatus.Rejected,
                                Text = msgText,
                                JobRequestId = jobReq.Id
                            };
                            _db.Notifications.Add(notification1);
                            Common.PushNotification(propUserData.UserType, propUserData.DeviceId, propUserData.DeviceToken,
                       msgText);

                            _db.SaveChanges();
                            status = true;
                            message = Resource.job_request_reject_success;
                        }
                        else
                        {
                            message = Resource.already_reject_job;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }
        public List<JobRequestViewModel> GetJobRequests(long? userId)
        {
            List<JobRequestViewModel> lstData = null;
            try
            {
                lstData = _db.JobRequests.Where(x => x.ServiceProviderId == userId && x.IsActive == true)
                    .Select
                    (x => new JobRequestViewModel()
                    {
                        Id = x.Id,
                        IsShownQuote = x.HasPrice == true ? false : true,
                        IsQuoteApproved = x.IsQuoteApproved,
                        QuotePrice = x.QuotePrice,
                        TimerEndTime = x.TimerEndTime,
                        TimerStartTime = x.TimerStartTime,
                        PropertyName = x.Property.Name,
                        PropertyAddress = x.Property.Address,
                        PropertyLatitude = x.Property.Latitude,
                        PropertyLongitude = x.Property.Longitude,
                        PropertyApartmentNumber = x.Property.ApartmentNumber,
                        PropertyFloorNumber = x.Property.FloorNumber,
                        PropertyBuildingCode = x.Property.BuildingCode,
                        PropertyId = x.Property.Id,
                        PropertyType = x.Property.Type,
                        Category = new CategoryViewModelApp()
                        {
                            Id = x.Category.Id,
                            Name = x.Category.Name,
                            Name_Russian = x.Category.Name_Russian,
                            Name_Hebrew = x.Category.Name_Hebrew,
                            Name_French = x.Category.Name_French,
                            Icon = x.Category.Icon,
                            Picture = x.Category.Picture,
                            // HasPrice = x.Category.HasPrice ?? false,
                        },
                        SubCategory = new SubCategoryViewModelApp()
                        {
                            Id = x.SubCategory.Id,
                            Name = x.SubCategory.Name,
                            Name_Russian = x.SubCategory.Name_Russian,
                            Name_Hebrew = x.SubCategory.Name_Hebrew,
                            Name_French = x.SubCategory.Name_French,
                            Price = x.SubCategory.Price,
                            ClientPrice = x.SubCategory.ClientPrice,
                            Icon = x.SubCategory.Icon,
                            Picture = x.SubCategory.Picture
                        },
                        Description = x.Description,
                        CustomerId = x.Property.User.UserId,
                        CustomerName = x.Property.User.FirstName + " " + x.Property.User.LastName,
                        CustomerImage = x.Property.User.PicturePath,
                        JobStartDatetime = x.JobStartDatetime,
                        JobEndDatetime = x.JobEndDatetime,
                        ServiceProviderId = x.User.UserId,
                        ServiceProviderName = x.User.FirstName + " " + x.User.LastName,
                        ServiceProviderImage = x.User.PicturePath,
                        ServiceProviderProfilePic = x.User.PicturePath,
                        ReferenceImages = x.JobRequestRefImages.Select(y => y.PicturePath).ToList(),
                        SubSubCategories = x.JobRequestSubSubCategories.Select(z => new SubSubCategoryViewModelApp()
                        {
                            Id = z.SubSubCategory.Id,
                            Name = z.SubSubCategory.Name,
                            Name_Russian = z.SubSubCategory.Name_Russian,
                            Name_Hebrew = z.SubSubCategory.Name_Hebrew,
                            Name_French = z.SubSubCategory.Name_French,
                            Price = z.SubSubCategory.Price,
                            ClientPrice = z.SubSubCategory.ClientPrice,
                            Icon = z.SubSubCategory.Icon,
                            Picture = z.SubSubCategory.Picture,
                        }).ToList(),
                        CheckList = x.JobRequestCheckLists.Select(p => new JobRequestCheckListModel()
                        {
                            Id = p.Id,
                            TaskDetail = p.TaskDetail,
                            IsDone = x.JobStatus == (int)Enums.RequestStatus.Completed ? true : false
                        }).ToList(),
                        PropertyDataModel = new PropertyModel
                        {
                            AccessToProperty = x.Property.AccessToProperty,
                            Address = x.Property.Address,
                            ApartmentNumber = x.Property.ApartmentNumber,
                            Balcony = x.Property.Balcony,
                            BuildingCode = x.Property.BuildingCode,
                            CreatedBy = x.Property.CreatedBy,
                            CreatedDate = x.Property.CreatedDate,
                            Dishwasher = x.Property.Dishwasher,
                            Doorman = x.Property.Doorman,
                            DuvetSize = x.Property.DuvetSize,
                            Elevator = x.Property.Elevator,
                            FloorNumber = x.Property.FloorNumber,
                            Garden = x.Property.Garden,
                            Name = x.Property.Name,
                            NoOfBathrooms = x.Property.NoOfBathrooms,
                            NoOfBedRooms = x.Property.NoOfBedRooms,
                            NoOfDoubleBeds = x.Property.NoOfDoubleBeds,
                            NoOfDoubleSofaBeds = x.Property.NoOfDoubleSofaBeds,
                            NoOfDuvet = x.Property.NoOfDuvet,
                            NoOfPillows = x.Property.NoOfPillows,
                            NoOfQueenBeds = x.Property.NoOfQueenBeds,
                            NoOfSingleBeds = x.Property.NoOfSingleBeds,
                            NoOfSingleSofaBeds = x.Property.NoOfSingleSofaBeds,
                            NoOfToilets = x.Property.NoOfToilets,
                            Parking = x.Property.Parking,
                            Pool = x.Property.Pool,
                            ShortTermApartment = x.Property.ShortTermApartment,
                            Size = x.Property.Size,
                            Type = x.Property.Type,
                            Latitude = x.Property.Latitude,
                            Longitude = x.Property.Longitude
                        },
                        JobStatus = x.JobStatus ?? (int)Enums.RequestStatus.InProgress,
                        PaymentMethod = x.Property.User.PaymentMethod,
                        IsPaymentDone = x.IsPaymentDone ?? false,
                        CustomerQuotePrice = x.ClientQuotePrice
                    }).ToList();

                var lstCancelled = _db.JobRequests.Where(x => x.RejectJobRequests.Select(y => y.UserId)
                .ToList().Contains(userId) && x.IsActive == true)
                    .Select
                    (x => new JobRequestViewModel()
                    {
                        Id = x.Id,
                        IsShownQuote = x.HasPrice == true ? false : true,
                        IsQuoteApproved = x.IsQuoteApproved,
                        QuotePrice = x.QuotePrice,
                        TimerEndTime = x.TimerEndTime,
                        TimerStartTime = x.TimerStartTime,
                        PropertyName = x.Property.Name,
                        PropertyAddress = x.Property.Address,
                        PropertyLatitude = x.Property.Latitude,
                        PropertyLongitude = x.Property.Longitude,
                        PropertyApartmentNumber = x.Property.ApartmentNumber,
                        PropertyFloorNumber = x.Property.FloorNumber,
                        PropertyBuildingCode = x.Property.BuildingCode,
                        PropertyType = x.Property.Type,
                        Category = new CategoryViewModelApp()
                        {
                            Id = x.Category.Id,
                            Name = x.Category.Name,
                            Icon = x.Category.Icon,
                            Picture = x.Category.Picture,
                            //HasPrice = x.Category.HasPrice ?? false,
                        },
                        SubCategory = new SubCategoryViewModelApp()
                        {
                            Id = x.SubCategory.Id,
                            Name = x.SubCategory.Name,
                            Price = x.SubCategory.Price,
                            ClientPrice = x.SubCategory.ClientPrice,
                            Icon = x.SubCategory.Icon,
                            Picture = x.SubCategory.Picture
                        },
                        Description = x.Description,
                        CustomerId = x.Property.User.UserId,
                        CustomerName = x.Property.User.FirstName + " " + x.Property.User.LastName,
                        CustomerImage = x.Property.User.PicturePath,
                        JobStartDatetime = x.JobStartDatetime,
                        JobEndDatetime = x.JobEndDatetime,
                        ServiceProviderProfilePic = x.User.PicturePath,
                        ReferenceImages = x.JobRequestRefImages.Select(y => y.PicturePath).ToList(),
                        SubSubCategories = x.JobRequestSubSubCategories.Select(z => new SubSubCategoryViewModelApp()
                        {
                            Id = z.SubSubCategory.Id,
                            Name = z.SubSubCategory.Name,
                            Price = z.SubSubCategory.Price,
                            ClientPrice = z.SubSubCategory.ClientPrice,
                            Icon = z.SubSubCategory.Icon,
                            Picture = z.SubSubCategory.Picture,
                        }).ToList(),
                        CheckList = x.JobRequestCheckLists.Select(p => new JobRequestCheckListModel()
                        {
                            Id = p.Id,
                            TaskDetail = p.TaskDetail,
                            IsDone = x.JobStatus == (int)Enums.RequestStatus.Completed ? true : false
                        }).ToList(),
                        JobStatus = (int)Enums.RequestStatus.Canceled,
                        PaymentMethod = x.Property.User.PaymentMethod,
                        IsPaymentDone = x.IsPaymentDone ?? false
                    }).ToList();

                lstData.AddRange(lstCancelled);
                message = Resource.success;
            }
            catch (Exception ex)
            {
                lstData = null;
                message = ex.Message;
            }
            return lstData;
        }


        public List<NotificationViewModel> GetAdminNotifications(long AdminId)
        {

            List<NotificationViewModel> lstData = null;
            List<Notification> lstNoti = new List<Notification>();
            try
            {
                lstNoti = _db.Notifications.Where(x => x.ToUserId == AdminId && x.IsActive == true).ToList();




                lstData = lstNoti.Select
                   (x => new NotificationViewModel()
                   {
                       CreatedDate = x.CreatedDate,
                       FromUserId = x.FromUserId,
                       FromUserName = x.User.FirstName + " " + x.User.LastName,
                       FromUserImage = x.User.PicturePath,
                       ToUserId = x.ToUserId,
                       ToUserName = x.User2 != null ? x.User2.FirstName + " " + x.User2.LastName : "",
                       ToUserImage = x.User2 != null ? x.User2.PicturePath : "",
                       Id = x.Id,
                       JobRequestId = x.JobRequestId,
                       NotificationStatus = x.NotificationStatus,
                       Text = x.Text,
                       QuotePrice = x.QuotePrice
                   }).OrderByDescending(x => x.CreatedDate).ToList();

                foreach (var item in lstData)
                {
                    if (item.Text.Trim() == "has accepted your job request.")
                    {
                        item.Text = " " + Resource.has_accepted_job_request;
                    }
                    if (item.Text.Trim() == "has sent job request.")
                    {
                        item.Text = " " + Resource.has_sent_job_request;
                    }
                    if (item.Text.Trim() == "Broom Service has rejected your")
                    {
                        item.Text = Resource.broom_service_has_rejected;
                    }
                    if (item.Text.Trim() == "has assigned this new job to you.")
                    {
                        item.Text = " " + Resource.has_assigned_job_to_you;
                    }
                    if (item.Text.Trim() == "has assigned by Broom Service for your job.")
                    {
                        item.Text = " " + Resource.worker_assigned_message;
                    }
                    if (item.Text.Trim().Contains("has sent you quote for"))
                    {
                        item.Text = item.Text.Replace("has sent you quote for", Resource.has_sent_quote_for);
                    }
                    if (item.Text.Trim() == "has accepted your quote price request.")
                    {
                        item.Text = " " + Resource.accepted_your_quote_price;
                    }
                    if (item.Text.Trim() == "has rejected your quote price request.")
                    {
                        item.Text = " " + Resource.rejected_your_quote_price;
                    }
                    if (item.Text.Trim() == "You have rejected Broom Service's quote request.")
                    {
                        item.Text = Resource.rejected_broom_service_request;
                    }
                    if (item.Text.Trim().Contains("has started timer for"))
                    {
                        item.Text = item.Text.Replace("has started timer for", Resource.has_started_timer_for);
                    }
                    if (item.Text.Trim().Contains("has ended timer for"))
                    {
                        item.Text = item.Text.Replace("has ended timer for", Resource.has_ended_timer_for);
                    }
                    if (item.Text.Trim().Contains("has completed job for"))
                    {
                        item.Text = item.Text.Replace("has completed job for", Resource.has_completed_job_for);
                    }
                    if (item.Text.Trim().Contains("You have accepted")
                        && item.Text.Trim().Contains("'s job request."))
                    {
                        item.Text = Resource.you_have_accepted + " " + item.FromUserName + Resource.s_job_request; ;
                    }
                    if (item.Text.Trim().Contains("You have rejected")
                        && item.Text.Trim().Contains("'s job request."))
                    {
                        item.Text = Resource.you_have_rejected + " " + item.FromUserName + Resource.s_job_request; ;
                    }
                    if (item.Text.Trim().Contains("You have accepted")
                        && item.Text.Trim().Contains("'s quote request."))
                    {
                        item.Text = Resource.you_have_accepted + " " + item.FromUserName + Resource.s_quote_request;
                    }
                    if (item.Text.Trim().Contains("You have rejected")
                            && item.Text.Trim().Contains("'s quote request."))
                    {
                        item.Text = Resource.you_have_rejected + " " + item.FromUserName + Resource.s_quote_request;
                    }
                }

                message = Resource.success;
            }
            catch (Exception ex)
            {
                lstData = null;
                message = ex.Message;
            }
            return lstData;
        }
        public AllOrderViewModel GetAllOrders(int? pageNumber)
        {
            AllOrderViewModel allData = new AllOrderViewModel();
            List<JobRequestViewModel> lstData = null;
            try
            {

                lstData = _db.JobRequests.Where(x => x.IsActive == true)
                    .Select
                    (x => new JobRequestViewModel()
                    {
                        Id = x.Id,
                        IsShownQuote = x.HasPrice == true ? false : true,
                        IsQuoteApproved = x.IsQuoteApproved,
                        QuotePrice = x.QuotePrice,
                        TimerEndTime = x.TimerEndTime,
                        TimerStartTime = x.TimerStartTime,
                        PropertyName = x.Property.Name,
                        PropertyAddress = x.Property.Address,
                        PropertyType = x.Property.Type,
                        Category = new CategoryViewModelApp()
                        {
                            Id = x.Category.Id,
                            Name = x.Category.Name,
                            Icon = x.Category.Icon,
                            Picture = x.Category.Picture,
                            //HasPrice = x.Category.HasPrice ?? false,
                        },
                        SubCategory = new SubCategoryViewModelApp()
                        {
                            Id = x.SubCategory.Id,
                            Name = x.SubCategory.Name,
                            Price = x.SubCategory.Price,
                            Icon = x.SubCategory.Icon,
                            Picture = x.SubCategory.Picture
                        },
                        Description = x.Description,
                        CustomerId = x.Property.User.UserId,
                        CustomerName = x.Property.User.FirstName + " " + x.Property.User.LastName,
                        CustomerImage = x.Property.User.PicturePath,
                        JobStartDatetime = x.JobStartDatetime,
                        JobEndDatetime = x.JobEndDatetime,
                        ServiceProviderId = x.User.UserId,
                        ServiceProviderName = x.User.FirstName + " " + x.User.LastName,
                        ServiceProviderImage = x.User.PicturePath,
                        ServiceProviderProfilePic = x.User.PicturePath,
                        ReferenceImages = x.JobRequestRefImages.Select(y => y.PicturePath).ToList(),
                        SubSubCategories = x.JobRequestSubSubCategories.Select(z => new SubSubCategoryViewModelApp()
                        {
                            Id = z.SubSubCategory.Id,
                            Name = z.SubSubCategory.Name,
                            Price = z.SubSubCategory.Price,
                            Icon = z.SubSubCategory.Icon,
                            Picture = z.SubSubCategory.Picture,
                        }).ToList(),
                        JobStatus = x.JobStatus ?? (int)Enums.RequestStatus.Pending,
                        ForWorkers = x.Category.ForWorkers ?? false
                    }).ToList();
                //propertyService.GetAllOrders().ToPagedList(pageNumber ?? 1, 10);
                allData.PendingOrders = lstData.Where(x => x.JobStatus == (int)Enums.RequestStatus.Pending).ToPagedList(pageNumber ?? 1, 10);
                allData.InProgressOrders = lstData.Where(x => x.JobStatus == (int)Enums.RequestStatus.InProgress).ToPagedList(pageNumber ?? 1, 10);
                allData.CompletedOrders = lstData.Where(x => x.JobStatus == (int)Enums.RequestStatus.Completed).ToPagedList(pageNumber ?? 1, 10);
                allData.CanceledOrders = lstData.Where(x => x.JobStatus == (int)Enums.RequestStatus.Canceled
               || x.JobStatus == (int)Enums.RequestStatus.QuoteCanceled).ToPagedList(pageNumber ?? 1, 10);

                message = Resource.success;
            }
            catch (Exception ex)
            {
                allData = null;
                message = ex.Message;
            }
            return allData;
        }
        public int GetPendingJobRequestCount()
        {
            int count = 0;
            try
            {

                count = _db.JobRequests.Where(x => x.Category.ForWorkers == true && x.IsActive == true
                && x.JobStatus == (int)Enums.RequestStatus.Pending).Count();
                message = Resource.success;
            }
            catch (Exception ex)
            {
                count = 0;
                message = ex.Message;
            }
            return count;
        }
        #endregion
        public JobRequestViewModel GetJobRequestDetail(long requestId, long userId)
        {
            JobRequestViewModel data = null;
            try
            {
                var AdminId = userService.GetAdminId();
                data = _db.JobRequests.Where(x => x.Id == requestId && x.IsActive == true)
                    .Select
                    (x => new JobRequestViewModel()
                    {
                        Id = x.Id,
                        IsShownQuote = x.HasPrice == true ? false : true,
                        IsQuoteApproved = x.IsQuoteApproved,
                        QuotePrice = x.QuotePrice,
                        TimerEndTime = x.TimerEndTime,
                        TimerStartTime = x.TimerStartTime,
                        PropertyName = x.Property.Name,
                        PropertyAddress = x.Property.Address,
                        PropertyLatitude = x.Property.Latitude,
                        PropertyLongitude = x.Property.Longitude,
                        PropertyApartmentNumber = x.Property.ApartmentNumber,
                        PropertyFloorNumber = x.Property.FloorNumber,
                        PropertyBuildingCode = x.Property.BuildingCode,
                        PropertyType = x.Property.Type,
                        PropertyId = x.Property.Id,
                        Category = new CategoryViewModelApp()
                        {
                            Id = x.Category.Id,
                            Name = x.Category.Name,
                            Name_Hebrew = x.Category.Name_Hebrew,
                            Name_French = x.Category.Name_French,
                            Name_Russian = x.Category.Name_Russian,
                            Icon = x.Category.Icon,
                            Picture = x.Category.Picture,
                            //HasPrice = x.Category.HasPrice ?? false,
                        },
                        SubCategory = new SubCategoryViewModelApp()
                        {
                            Id = x.SubCategory.Id,
                            Name = x.SubCategory.Name,
                            Name_Hebrew = x.SubCategory.Name_Hebrew,
                            Name_French = x.SubCategory.Name_French,
                            Name_Russian = x.SubCategory.Name_Russian,
                            Price = x.SubCategory.Price,
                            ClientPrice = x.SubCategory.ClientPrice,
                            Icon = x.SubCategory.Icon,
                            Picture = x.SubCategory.Picture
                        },
                        Description = x.Description,
                        CustomerId = x.Property.User.UserId,
                        CustomerName = x.Property.User.FirstName + " " + x.Property.User.LastName,
                        CustomerImage = x.Property.User.PicturePath,
                        JobStartDatetime = x.JobStartDatetime,
                        JobEndDatetime = x.JobEndDatetime,
                        ServiceProviderId = x.User.UserId,
                        ServiceProviderName = x.User.FirstName + " " + x.User.LastName,
                        ServiceProviderImage = x.User.PicturePath,
                        ServiceProviderProfilePic = x.User.PicturePath,
                        ReferenceImages = x.JobRequestRefImages.Select(y => y.PicturePath).ToList(),
                        SubSubCategories = x.JobRequestSubSubCategories.Select(z => new SubSubCategoryViewModelApp()
                        {
                            Id = z.SubSubCategory.Id,
                            Name = z.SubSubCategory.Name,
                            Name_Hebrew = z.SubSubCategory.Name_Hebrew,
                            Name_French = z.SubSubCategory.Name_French,
                            Name_Russian = z.SubSubCategory.Name_Russian,
                            Price = z.SubSubCategory.Price,
                            ClientPrice = z.SubSubCategory.ClientPrice,
                            Icon = z.SubSubCategory.Icon,
                            Picture = z.SubSubCategory.Picture,
                        }).ToList(),
                        CheckList = x.JobRequestCheckLists.Select(p => new JobRequestCheckListModel()
                        {
                            Id = p.Id,
                            TaskDetail = p.TaskDetail,
                            IsDone = x.JobStatus == (int)Enums.RequestStatus.Completed ? true : false
                        }).ToList(),
                        PropertyDataModel = new PropertyModel
                        {
                            AccessToProperty = x.Property.AccessToProperty,
                            Address = x.Property.Address,
                            ApartmentNumber = x.Property.ApartmentNumber,
                            Balcony = x.Property.Balcony,
                            BuildingCode = x.Property.BuildingCode,
                            CreatedBy = x.Property.CreatedBy,
                            CreatedDate = x.Property.CreatedDate,
                            Dishwasher = x.Property.Dishwasher,
                            Doorman = x.Property.Doorman,
                            DuvetSize = x.Property.DuvetSize,
                            Elevator = x.Property.Elevator,
                            FloorNumber = x.Property.FloorNumber,
                            Garden = x.Property.Garden,
                            Name = x.Property.Name,
                            NoOfBathrooms = x.Property.NoOfBathrooms,
                            NoOfBedRooms = x.Property.NoOfBedRooms,
                            NoOfDoubleBeds = x.Property.NoOfDoubleBeds,
                            NoOfDoubleSofaBeds = x.Property.NoOfDoubleSofaBeds,
                            NoOfDuvet = x.Property.NoOfDuvet,
                            NoOfPillows = x.Property.NoOfPillows,
                            NoOfQueenBeds = x.Property.NoOfQueenBeds,
                            NoOfSingleBeds = x.Property.NoOfSingleBeds,
                            NoOfSingleSofaBeds = x.Property.NoOfSingleSofaBeds,
                            NoOfToilets = x.Property.NoOfToilets,
                            Parking = x.Property.Parking,
                            Pool = x.Property.Pool,
                            ShortTermApartment = x.Property.ShortTermApartment,
                            Size = x.Property.Size,
                            Type = x.Property.Type,
                            Latitude = x.Property.Latitude,
                            Longitude = x.Property.Longitude
                        },
                        JobStatus = x.RejectJobRequests.Where(j => j.JobRequestId == x.Id && j.UserId == userId).FirstOrDefault() != null ? (int)Enums.RequestStatus.Canceled :
                        x.JobStatus ?? (int)Enums.RequestStatus.InProgress,
                        PaymentMethod = x.Property.User.PaymentMethod,
                        IsPaymentDone = x.IsPaymentDone ?? false,
                        AdminId = AdminId,
                        CustomerQuotePrice = x.ClientQuotePrice
                    }).FirstOrDefault();



                message = Resource.success;
            }
            catch (Exception ex)
            {
                data = null;
                message = ex.Message;
            }
            return data;
        }
        public List<NotificationViewModel> GetNotifications(long userId)
        {
            List<NotificationViewModel> lstData = null;
            List<Notification> lstNoti = new List<Notification>();
            try
            {
                var user = _db.Users.Where(x => x.UserId == userId).FirstOrDefault();
                if (user != null)
                {
                    if (user.UserType == (int)Enums.UserTypeEnum.Customer)
                    {
                        lstNoti = _db.Notifications.Where(x => x.ToUserId == userId && x.IsActive == true).ToList();
                    }
                    else if (user.UserType == (int)Enums.UserTypeEnum.Worker ||
                        user.UserType == (int)Enums.UserTypeEnum.ServiceProvider)
                    {

                        var forWorkers = user.UserType == (int)Enums.UserTypeEnum.Worker;

                        var lst = _db.Notifications.Where(z => z.ToUserId == userId && z.JobRequest.Category.ForWorkers == forWorkers
                        && z.IsActive == true).ToList();



                        foreach (var item in lst)
                        {
                            var jobSubSubCategories = item.JobRequest.JobRequestSubSubCategories.ToList();
                            if (jobSubSubCategories.Count != 0)
                            {
                                var userSubSubcat = user.UserSubCategories.Select(x => x.SubSubCategoryId).ToList();
                                foreach (var subitem in item.JobRequest.JobRequestSubSubCategories)
                                {
                                    if (userSubSubcat.Contains(subitem.SubSubCategoryId))
                                    {
                                        lstNoti.Add(item);
                                        goto there;//continue;
                                    }
                                }
                                there: var isAdded = true;
                            }
                            else
                            {
                                var userSubcat = user.UserSubCategories.Select(x => x.SubCategoryId).ToList();
                                if (userSubcat.Contains(item.JobRequest.SubCategoryId))
                                {
                                    lstNoti.Add(item);
                                }
                            }
                        }
                        if (user.UserType == (int)Enums.UserTypeEnum.ServiceProvider)
                        {
                            var lstNotAssign = _db.Notifications.Where(z => z.ToUserId == null && z.JobRequest.JobStatus ==
                      (int)Enums.RequestStatus.Pending && z.IsActive == true && z.JobRequest.Category.ForWorkers == forWorkers).ToList();

                            var lstRejAssign = _db.RejectJobRequests.Where(z => z.UserId == userId).Select(p => p.JobRequestId).ToList();

                            var jobIds = lst.Select(x => x.JobRequestId).ToList();
                            foreach (var item in lstNotAssign)
                            {
                                if (!jobIds.Contains(item.JobRequestId))
                                {
                                    if (!lstRejAssign.Contains(item.JobRequestId))
                                    {
                                        var jobSubSubCategories = item.JobRequest.JobRequestSubSubCategories.ToList();
                                        if (jobSubSubCategories.Count != 0)
                                        {
                                            var userSubSubcat = user.UserSubCategories.Select(x => x.SubSubCategoryId).ToList();
                                            foreach (var subitem in item.JobRequest.JobRequestSubSubCategories)
                                            {
                                                if (userSubSubcat.Contains(subitem.SubSubCategoryId))
                                                {
                                                    lstNoti.Add(item);
                                                    continue;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var userSubcat = user.UserSubCategories.Select(x => x.SubCategoryId).ToList();
                                            if (userSubcat.Contains(item.JobRequest.SubCategoryId))
                                            {
                                                lstNoti.Add(item);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    var adminId = userService.GetAdminId();
                    lstData = lstNoti.Select
                       (x => new NotificationViewModel()
                       {
                           CreatedDate = x.CreatedDate,
                           FromUserId = x.FromUserId,
                           FromUserName = (x.FromUserId == adminId) ? Resource.broom_service : x.User.FirstName + " " + x.User.LastName,
                           FromUserImage = x.User.PicturePath,
                           ToUserId = x.ToUserId,
                           ToUserName = x.User2 != null ? x.User2.FirstName + " " + x.User2.LastName : "",
                           ToUserImage = x.User2 != null ? x.User2.PicturePath : "",
                           Id = x.Id,
                           JobRequestId = x.JobRequestId,
                           NotificationStatus = x.NotificationStatus,
                           Text = x.Text,
                           QuotePrice = x.QuotePrice
                       }).ToList();

                    foreach (var item in lstData)
                    {
                        if (item.Text.Trim() == "has accepted your job request.")
                        {
                            item.Text = " " + Resource.has_accepted_job_request;
                        }
                        if (item.Text.Trim() == "has sent job request.")
                        {
                            item.Text = " " + Resource.has_sent_job_request;
                        }
                        if (item.Text.Trim() == "Broom Service has rejected your")
                        {
                            item.Text = Resource.broom_service_has_rejected;
                        }
                        if (item.Text.Trim() == "has assigned this new job to you.")
                        {
                            item.Text = " " + Resource.has_assigned_job_to_you;
                        }
                        if (item.Text.Trim() == "has assigned by Broom Service for your job.")
                        {
                            item.Text = " " + Resource.worker_assigned_message;
                        }
                        if (item.Text.Trim().Contains("has sent you quote for"))
                        {
                            item.Text = item.Text.Replace("has sent you quote for", Resource.has_sent_quote_for);
                        }
                        if (item.Text.Trim() == "has accepted your quote price request.")
                        {
                            item.Text = " " + Resource.accepted_your_quote_price;
                        }
                        if (item.Text.Trim() == "has rejected your quote price request.")
                        {
                            item.Text = " " + Resource.rejected_your_quote_price;
                        }
                        if (item.Text.Trim() == "You have rejected Broom Service's quote request.")
                        {
                            item.Text = Resource.rejected_broom_service_request;
                        }
                        if (item.Text.Trim().Contains("has started timer for"))
                        {
                            item.Text=item.Text.Replace("has started timer for", Resource.has_started_timer_for);
                        }
                        if (item.Text.Trim().Contains("has ended timer for"))
                        {
                            item.Text = item.Text.Replace("has ended timer for", Resource.has_ended_timer_for);
                        }
                        if (item.Text.Trim().Contains("has completed job for"))
                        {
                            item.Text = item.Text.Replace("has completed job for", Resource.has_completed_job_for);
                        }
                        if (item.Text.Trim().Contains("You have accepted")
                            && item.Text.Trim().Contains("'s job request."))
                        {
                            item.Text = Resource.you_have_accepted + " " + item.FromUserName + Resource.s_job_request; ;
                        }
                        if (item.Text.Trim().Contains("You have rejected")
                            && item.Text.Trim().Contains("'s job request."))
                        {
                            item.Text = Resource.you_have_rejected + " " + item.FromUserName + Resource.s_job_request; ;
                        }
                        if (item.Text.Trim().Contains("You have accepted")
                            && item.Text.Trim().Contains("'s quote request."))
                        {
                            item.Text = Resource.you_have_accepted + " " + item.FromUserName + Resource.s_quote_request;
                        }
                        if (item.Text.Trim().Contains("You have rejected")
                            && item.Text.Trim().Contains("'s quote request."))
                        {
                            item.Text = Resource.you_have_rejected + " " + item.FromUserName + Resource.s_quote_request;
                        }
                    }

                }
                message = Resource.success;
            }
            catch (Exception ex)
            {
                lstData = null;
                message = ex.Message;
            }
            return lstData;
        }

        /// <summary>
        /// Assign Job To Worker Method.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AssignJobToWorker(long requestId, long workerId, long AdminId)
        {
            bool status = false;
            try
            {
                var jobReq = _db.JobRequests.Where(x => x.Id == requestId).FirstOrDefault();
                if (jobReq != null)
                {
                    jobReq.ServiceProviderId = workerId;
                    jobReq.JobStatus = (int)(Enums.RequestStatus.InProgress);
                    var propUserData = jobReq.Property.User;
                    var workerData = _db.Users.Where(x => x.UserId == workerId).FirstOrDefault();
                    string msgText = " has assigned this new job to you.";

                    Notification notification = new Notification()
                    {
                        CreatedDate = DateTime.Now,
                        FromUserId = AdminId,
                        ToUserId = workerId,
                        IsActive = true,
                        NotificationStatus = (int)Enums.NotificationStatus.Assigned,
                        Text = msgText,
                        JobRequestId = jobReq.Id
                    };
                    _db.Notifications.Add(notification);

                    msgText = Resource.broom_service + " " + msgText;
                    Common.PushNotification(workerData.UserType, workerData.DeviceId, workerData.DeviceToken,
                        msgText);

                    msgText = " has assigned by Broom Service for your job.";
                    Notification notification1 = new Notification()
                    {
                        CreatedDate = DateTime.Now,
                        FromUserId = workerId,
                        ToUserId = propUserData.UserId,
                        IsActive = true,
                        NotificationStatus = (int)Enums.NotificationStatus.Assigned,
                        Text = msgText,
                        JobRequestId = jobReq.Id
                    };
                    _db.Notifications.Add(notification1);

                    msgText = workerData.FirstName + " " + workerData.LastName + msgText;
                    Common.PushNotification(propUserData.UserType, propUserData.DeviceId, propUserData.DeviceToken,
                        msgText);
                    _db.SaveChanges();

                    status = true;
                    message = Resource.job_successfully_assigned + " " + workerData.FirstName + " " + workerData.LastName + ".";

                }
                else
                {
                    message = Resource.job_not_exists;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }

        /// <summary>
        /// Send Quote Method.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool SendQuote(QuotePriceModel model, bool SendByAdmin = false)
        {
            bool status = false;
            try
            {
                var jobReq = _db.JobRequests.Where(x => x.Id == model.JobRequestId).FirstOrDefault();
                if (jobReq != null)
                {
                    if (jobReq.QuotePrice == null)
                    {
                        decimal clientQuotePrice = 0;
                        if (SendByAdmin)
                        {
                            clientQuotePrice = model.QuotePrice;
                        }
                        else
                        {
                            clientQuotePrice = Common.GetFinalQuotePrice(model.QuotePrice);
                        }
                        jobReq.QuotePrice = model.QuotePrice;
                        jobReq.ClientQuotePrice = clientQuotePrice;

                        var propUserData = jobReq.Property.User;
                        var serviceProviderData = jobReq.User;
                        string msgText = " has sent you quote for "
                            + jobReq.Property.Name + " i.e. ";

                        var QuotePrice = Common.payme_currency.ToUpper() + " " + clientQuotePrice;
                        Notification notification = new Notification()
                        {
                            CreatedDate = DateTime.Now,
                            FromUserId = model.UserId,
                            ToUserId = propUserData.UserId,
                            IsActive = true,
                            NotificationStatus = (int)Enums.NotificationStatus.SentQuotation,
                            Text = msgText,
                            JobRequestId = jobReq.Id,
                            QuotePrice = QuotePrice
                        };
                        _db.Notifications.Add(notification);



                        _db.SaveChanges();

                        if (SendByAdmin)
                        {
                            msgText = Resource.broom_service + " " + msgText + QuotePrice;
                        }
                        else
                        {
                            msgText = serviceProviderData.FirstName + " " + serviceProviderData.LastName + msgText +
                                 QuotePrice;
                        }
                        Common.PushNotification(propUserData.UserType, propUserData.DeviceId, propUserData.DeviceToken,
                            msgText);



                        status = true;
                        message = Resource.quote_sent_success;
                    }
                    else
                    {
                        message = Resource.quote_already_sent;
                    }
                }
                else
                {
                    message = Resource.job_not_exists;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }



        public bool AcceptRejectQuote(AcceptRejectQuoteModel model)
        {
            bool status = false;
            try
            {
                var jobReq = _db.JobRequests.Where(x => x.Id == model.JobRequestId
                && x.Property.User.UserId == model.UserId).FirstOrDefault();
                if (jobReq != null)
                {
                    var propUserData = jobReq.Property.User;
                    var servicProvUserData = jobReq.User;

                    if (jobReq.IsQuoteApproved == true)
                    {
                        message = Resource.quote_already_accepted;
                        return false;
                    }
                    if (jobReq.IsQuoteApproved == false)
                    {
                        message = Resource.quote_already_rejected;
                        return false;
                    }
                    if (model.IsAccept)
                    {
                        jobReq.IsQuoteApproved = true;

                        string msgText = " has accepted your quote price request.";

                        if (servicProvUserData != null)
                        {
                            Notification notification = new Notification()
                            {
                                CreatedDate = DateTime.Now,
                                FromUserId = propUserData.UserId,
                                ToUserId = servicProvUserData.UserId,
                                IsActive = true,
                                NotificationStatus = (int)Enums.NotificationStatus.AcceptedQuotation,
                                Text = msgText,
                                JobRequestId = jobReq.Id
                            };
                            _db.Notifications.Add(notification);
                            Common.PushNotification(servicProvUserData.UserType, servicProvUserData.DeviceId, servicProvUserData.DeviceToken,
                              propUserData.FirstName + " " + propUserData.LastName + msgText);
                            msgText = "You have accepted " + servicProvUserData.FirstName + " " + servicProvUserData.LastName + "'s quote request.";

                        }
                        else
                        {
                            Notification notification = new Notification()
                            {
                                CreatedDate = DateTime.Now,
                                FromUserId = propUserData.UserId,
                                ToUserId = userService.GetAdminId(),
                                IsActive = true,
                                NotificationStatus = (int)Enums.NotificationStatus.AcceptedQuotation,
                                Text = msgText,
                                JobRequestId = jobReq.Id
                            };
                            _db.Notifications.Add(notification);
                            msgText = "You have accepted Broom Service's quote request";

                        }

                        var notifData = _db.Notifications.Where(x => x.Id == model.NotificationId).FirstOrDefault();

                        if (notifData != null)
                        {
                            notifData.NotificationStatus = (int)Enums.NotificationStatus.AcceptedQuotation;
                            notifData.Text = msgText;
                        }
                        _db.SaveChanges();

                        status = true;
                        message = Resource.quote_accepted_success;
                    }
                    else
                    {
                        jobReq.IsQuoteApproved = false;
                        jobReq.JobStatus = (int)Enums.RequestStatus.QuoteCanceled;


                        string msgText = " has rejected your quote price request.";

                        if (servicProvUserData != null)
                        {
                            Notification notification = new Notification()
                            {
                                CreatedDate = DateTime.Now,
                                FromUserId = propUserData.UserId,
                                ToUserId = servicProvUserData.UserId,
                                IsActive = true,
                                NotificationStatus = (int)Enums.NotificationStatus.RejectedQuotation,
                                Text = msgText,
                                JobRequestId = jobReq.Id
                            };
                            _db.Notifications.Add(notification);
                            Common.PushNotification(servicProvUserData.UserType, servicProvUserData.DeviceId, servicProvUserData.DeviceToken,
                              propUserData.FirstName + " " + propUserData.LastName + msgText);
                            msgText = "You have rejected " + servicProvUserData.FirstName + " " + servicProvUserData.LastName + "'s quote request.";

                        }
                        else
                        {
                            Notification notification = new Notification()
                            {
                                CreatedDate = DateTime.Now,
                                FromUserId = propUserData.UserId,
                                ToUserId = userService.GetAdminId(),
                                IsActive = true,
                                NotificationStatus = (int)Enums.NotificationStatus.RejectedQuotation,
                                Text = msgText,
                                JobRequestId = jobReq.Id
                            };
                            _db.Notifications.Add(notification);
                            msgText = "You have rejected Broom Service's quote request.";

                        }


                        var notifData = _db.Notifications.Where(x => x.Id == model.NotificationId).FirstOrDefault();

                        if (notifData != null)
                        {
                            notifData.NotificationStatus = (int)Enums.NotificationStatus.RejectedQuotation;
                            notifData.Text = msgText;
                        }

                        _db.SaveChanges();

                        status = true;
                        message = Resource.quote_rejected_success;
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }



        /// <summary>
        /// Start End Timer Method.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool StartEndTimer(UpdateTimerTimeModel model)
        {
            bool status = false;
            try
            {
                var jobReq = _db.JobRequests.Where(x => x.Id == model.JobRequestId).FirstOrDefault();
                if (jobReq != null)
                {
                    if (model.IsStart)
                    {
                        if (jobReq.TimerStartTime == null)
                        {
                            jobReq.TimerStartTime = DateTime.Now;

                            var propUserData = jobReq.Property.User;
                            var serviceProviderData = jobReq.User;
                            string msgText = " has started timer for "
                                + jobReq.Property.Name;
                            Notification notification = new Notification()
                            {
                                CreatedDate = DateTime.Now,
                                FromUserId = model.UserId,
                                ToUserId = propUserData.UserId,
                                IsActive = true,
                                NotificationStatus = (int)Enums.NotificationStatus.TimerStarted,
                                Text = msgText,
                                JobRequestId = jobReq.Id,
                            };
                            _db.Notifications.Add(notification);

                            _db.SaveChanges();
                            Common.PushNotification(propUserData.UserType, propUserData.DeviceId, propUserData.DeviceToken,
                                serviceProviderData.FirstName + " " + serviceProviderData.LastName + msgText);



                            status = true;
                            message = Resource.timer_started_success;
                        }
                        else
                        {
                            message = Resource.timer_already_started;
                        }
                    }
                    else
                    {
                        if (jobReq.TimerEndTime == null)
                        {
                            jobReq.TimerEndTime = DateTime.Now;

                            var propUserData = jobReq.Property.User;
                            var serviceProviderData = jobReq.User;
                            string msgText = " has ended timer for "
                                + jobReq.Property.Name;
                            Notification notification = new Notification()
                            {
                                CreatedDate = DateTime.Now,
                                FromUserId = model.UserId,
                                ToUserId = propUserData.UserId,
                                IsActive = true,
                                NotificationStatus = (int)Enums.NotificationStatus.TimerStarted,
                                Text = msgText,
                                JobRequestId = jobReq.Id,
                            };
                            _db.Notifications.Add(notification);


                            _db.SaveChanges();
                            Common.PushNotification(propUserData.UserType, propUserData.DeviceId, propUserData.DeviceToken,
                                serviceProviderData.FirstName + " " + serviceProviderData.LastName + msgText);


                            status = true;
                            message = Resource.timer_ended_success;
                        }
                        else
                        {
                            message = Resource.timer_already_ended;
                        }
                    }
                }
                else
                {
                    message = Resource.job_not_exists;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }


        /// <summary>
        /// Complete Job Request Method.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool CompleteJobRequest(CompleteJobRequestModel model)
        {
            bool status = false;
            try
            {
                var jobReq = _db.JobRequests.Where(x => x.Id == model.JobRequestId).FirstOrDefault();
                if (jobReq != null)
                {

                    jobReq.JobStatus = (int)Enums.RequestStatus.Completed;

                    var propUserData = jobReq.Property.User;
                    var serviceProviderData = jobReq.User;
                    string msgText = " has completed job for "
                        + jobReq.Property.Name;
                    Notification notification = new Notification()
                    {
                        CreatedDate = DateTime.Now,
                        FromUserId = model.UserId,
                        ToUserId = propUserData.UserId,
                        IsActive = true,
                        NotificationStatus = (int)Enums.NotificationStatus.Completed,
                        Text = msgText,
                        JobRequestId = jobReq.Id,
                    };
                    _db.Notifications.Add(notification);



                    _db.SaveChanges();
                    Common.PushNotification(propUserData.UserType, propUserData.DeviceId, propUserData.DeviceToken,
                        serviceProviderData.FirstName + " " + serviceProviderData.LastName + msgText);



                    status = true;
                    message = Resource.job_complete_success;

                }
                else
                {
                    message = Resource.job_not_exists;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }


        /// <summary>
        /// Submit UserReview Method.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool SubmitUserReview(UserReviewModel model)
        {
            bool status = false;
            try
            {
                var review = _db.UserReviews.Where(x => x.CustomerId == model.CustomerId
                && x.ToUserId == model.ToUserId).FirstOrDefault();
                if (review == null)
                {
                    UserReview userReview = new UserReview()
                    {
                        CreatedDate = DateTime.Now,
                        CustomerId = model.CustomerId,
                        IsActive = true,
                        Rating = model.UserRating,
                        Review = model.UserReview,
                        ToUserId = model.ToUserId
                    };
                    _db.UserReviews.Add(userReview);

                    status = true;
                    message = Resource.review_success;

                }
                else
                {
                    review.Review = model.UserReview;
                    review.Rating = model.UserRating;
                    review.ModifiedDate = DateTime.Now;
                    status = true;
                    message = Resource.review_success;
                }
                var jobreview = _db.UserJobReviews.Where(x => x.CustomerId == model.CustomerId
               && x.ToUserId == model.ToUserId && x.JobRequestId == model.JobRequestId).FirstOrDefault();
                if (jobreview == null)
                {
                    UserJobReview userJobReview = new UserJobReview()
                    {
                        CreatedDate = DateTime.Now,
                        CustomerId = model.CustomerId,
                        IsActive = true,
                        Rating = model.JobRating,
                        Review = model.JobReview,
                        ToUserId = model.ToUserId,
                        JobRequestId = model.JobRequestId
                    };
                    _db.UserJobReviews.Add(userJobReview);



                    status = true;
                    message = Resource.review_success;

                }
                else
                {
                    jobreview.Rating = model.JobRating;
                    jobreview.Review = model.JobReview;
                    jobreview.ModifiedDate = DateTime.Now;
                    status = true;
                    message = Resource.review_success;
                }
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }
        /// <summary>
        /// Add ChatRequest Method.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddChatRequest(ChatRequestModel model)
        {
            bool status = false;
            try
            {
                var chatRequestData = _db.UserChats.Where(a => (a.FromUserId == model.FromUserId && a.ToUserId == model.ToUserId)
                   || (a.FromUserId == model.ToUserId && a.ToUserId == model.FromUserId)).FirstOrDefault();
                if (chatRequestData == null)
                {
                    UserChat chat = new UserChat();
                    chat.CreatedDate = DateTime.Now;
                    chat.FromUserId = model.FromUserId;
                    chat.ToUserId = model.ToUserId;

                    _db.UserChats.Add(chat);
                    _db.SaveChanges();
                    status = true;
                    message = Resource.success;
                }
                else
                {
                    chatRequestData.CreatedDate = DateTime.Now;
                    _db.SaveChanges();
                    status = true;
                    message = Resource.success;
                }
                var toUserDta = _db.Users.Where(x => x.UserId == model.ToUserId).FirstOrDefault();
                if (toUserDta != null && toUserDta.DeviceId != null && !string.IsNullOrEmpty(toUserDta.DeviceToken))
                {
                    var fromUserDta = _db.Users.Where(x => x.UserId == model.FromUserId).FirstOrDefault();
                    if (fromUserDta != null)
                    {
                        string msg = Resource.you_have_new_msg + " " + fromUserDta.FirstName + " "
                            + fromUserDta.LastName;
                        Common.PushNotification(toUserDta.UserType, toUserDta.DeviceId, toUserDta.DeviceToken, msg);
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }
        public List<ChatUser> GetChat(long userId)
        {
            List<ChatUser> lstData = null;
            try
            {

                lstData = _db.UserChats.Where(x => x.FromUserId == userId || x.ToUserId == userId)
                    .Select
                    (x => new ChatUser()
                    {
                        Name = x.FromUserId == userId ? x.User1.FirstName + " " + x.User1.LastName
                        : x.User.FirstName + " " + x.User.LastName,
                        UserId = x.FromUserId == userId ? x.ToUserId : x.FromUserId,
                        PicturePath = x.FromUserId == userId ? x.User1.PicturePath : x.User.PicturePath,
                        CreatedDate = x.CreatedDate
                    }).OrderByDescending(x => x.CreatedDate).ToList();
                message = Resource.success;
            }
            catch (Exception ex)
            {
                lstData = null;
                message = ex.Message;
            }
            return lstData;
        }

        public PropertyModel GetPropertiesById(long Id)
        {
            PropertyModel Data = null;
            try
            {
                Data = _db.Properties.Where(x => x.Id == Id).Select
                    (x => new PropertyModel()
                    {
                        AccessToProperty = x.AccessToProperty,
                        ApartmentNumber = x.ApartmentNumber,
                        Balcony = x.Balcony,
                        BuildingCode = x.BuildingCode,
                        CreatedBy = x.CreatedBy,
                        CreatedDate = x.CreatedDate,
                        Dishwasher = x.Dishwasher,
                        Doorman = x.Doorman,
                        DuvetSize = x.DuvetSize,
                        Elevator = x.Elevator,
                        FloorNumber = x.FloorNumber,
                        Garden = x.Garden,
                        Id = x.Id,
                        Name = x.Name,
                        NoOfBedRooms = x.NoOfBedRooms,
                        NoOfToilets = x.NoOfToilets,
                        Size = x.Size,
                        NoOfBathrooms = x.NoOfBathrooms,
                        NoOfDoubleBeds = x.NoOfDoubleBeds,
                        NoOfDoubleSofaBeds = x.NoOfDoubleSofaBeds,
                        NoOfDuvet = x.NoOfDuvet,
                        NoOfPillows = x.NoOfPillows,
                        NoOfQueenBeds = x.NoOfQueenBeds,
                        NoOfSingleBeds = x.NoOfSingleBeds,
                        NoOfSingleSofaBeds = x.NoOfSingleSofaBeds,
                        Parking = x.Parking,
                        Pool = x.Pool,
                        ShortTermApartment = x.ShortTermApartment,
                        Type = x.Type,
                        Address = x.Address,
                        IsActive = x.IsActive
                    }).FirstOrDefault();

            }
            catch (Exception ex)
            {
                Data = null;
            }
            return Data;
        }

    }
}