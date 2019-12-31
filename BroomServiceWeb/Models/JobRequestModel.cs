using BroomServiceWeb.Helpers;
using BroomServiceWeb.Services;
using BroomServiceWeb.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BroomServiceWeb.Models
{
    public class JobRequestReturnModel
    {
        public long RequestId { get; set; }
        public int PaymentMethod { get; set; }
    }

    public class JobRequestModel
    {
        public long? UserId { get; set; }
        public List<string> CheckList { get; set; }
        public long? PropertyId { get; set; }
        public long? CategoryId { get; set; }
        public DateTime? JobStartDatetime { get; set; }
        public DateTime? JobEndDatetime { get; set; }
        public string Description { get; set; }
        public long? SubCategoryId { get; set; }
        public List<long> SubSubCategories { get; set; }
        public List<string> ReferenceImages { get; set; }
        public int? PaymentMethod { get; set; }
        public decimal? Price { get; set; }
        public decimal? ClientPrice { get; set; }
        public bool? HasPrice { get; set; }
    }
    public class JobRequestViewModel
    {
        PropertyService service = new PropertyService();
        public JobRequestViewModel()
        {
        }
        public long Id { get; set; }
        public DateTime? JobStartDatetime { get; set; }
        public string JobStartTime { get; set; }
        public string JobEndTime { get; set; }
        public DateTime? JobEndDatetime { get; set; }
        public string Description { get; set; }
        public List<string> ReferenceImages { get; set; }
        public List<JobRequestCheckListModel> CheckList { get; set; }
        public long PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string PropertyType { get; set; }
        public int? PropertyFloorNumber { get; set; }
        public int? PropertyApartmentNumber { get; set; }
        public string PropertyBuildingCode { get; set; }
        public string PropertyAddress { get; set; }
        public decimal? PropertyLatitude { get; set; }
        public decimal? PropertyLongitude { get; set; }
        public CategoryViewModelApp Category { get; set; }
        public SubCategoryViewModelApp SubCategory { get; set; }
        public List<SubSubCategoryViewModelApp> SubSubCategories { get; set; }
        public PropertyModel PropertyDataModel { get; set; }
        public string CustomerName { get; set; }
        public long? CustomerId { get; set; }
        public string CustomerImage { get; set; }
        public string ServiceProviderName { get; set; }
        public long? ServiceProviderId { get; set; }
        public string ServiceProviderImage { get; set; }
        public string ServiceProviderProfilePic { get; set; }
        public bool IsShownQuote { get; set; }
        public bool ForWorkers { get; set; }
        public int JobStatus { get; set; }
        public string JobStatusStr
        {
            get
            {
                return Enums.GetRequestStatus(JobStatus);
            }
        }
        public decimal? QuotePrice { get; set; }
        public decimal? CustomerQuotePrice { get; set; }
        //{
        //    get
        //    {
        //        return QuotePrice != null ? Common.GetFinalQuotePrice(QuotePrice ?? 0) : QuotePrice;
        //    }
        //}
        public bool? IsQuoteApproved { get; set; }
        public System.DateTime? TimerStartTime { get; set; }
        public System.DateTime? TimerEndTime { get; set; }
        public int? UserRating
        {
            get
            {
                if (JobStatus == (int)Enums.RequestStatus.Completed)
                {
                    return service.GetUserRating(CustomerId, ServiceProviderId);
                }
                else
                {
                    return null;
                }
            }
        }
        public double Hours
        {
            get
            {
                if (TimerStartTime != null && TimerEndTime != null)
                {
                    return Common.GetHours(TimerStartTime.Value, TimerEndTime.Value);
                }
                else
                {
                    return 0;
                }
            }
        }
        public int? UserJobRating
        {
            get
            {
                if (JobStatus == (int)Enums.RequestStatus.Completed)
                {
                    return service.GetUserJobRating(CustomerId, ServiceProviderId, Id);
                }
                else
                {
                    return null;
                }
            }
        }
        public int? PaymentMethod { get; set; }
        public bool? IsPaymentDone { get; set; }
        public long? AdminId { get; set; }
        public string JobReview
        {
            get
            {
                if (JobStatus == (int)Enums.RequestStatus.Completed)
                {
                    return service.GetUserJobReview(CustomerId, ServiceProviderId, Id);
                }
                else
                {
                    return null;
                }
            }
        }
        public string UserReview
        {
            get
            {
                if (JobStatus == (int)Enums.RequestStatus.Completed)
                {
                    return service.GetUserReview(CustomerId, ServiceProviderId);
                }
                else
                {
                    return null;
                }
            }
        }

    }
    public class AllOrderViewModel
    {
        public IPagedList<JobRequestViewModel> CompletedOrders { get; set; }
        public IPagedList<JobRequestViewModel> PendingOrders { get; set; }
        public IPagedList<JobRequestViewModel> InProgressOrders { get; set; }
        public IPagedList<JobRequestViewModel> CanceledOrders { get; set; }
    }
    public class AcceptRejectJobReqModel
    {
        public long UserId { get; set; }
        public long JobRequestId { get; set; }
        public bool IsAccept { get; set; }
    }
    public class AcceptRejectJobReqAdminModel
    {
        public long UserId { get; set; }
        public long JobRequestId { get; set; }
        public long NotificationId { get; set; }
        public bool IsAccept { get; set; }
    }
    public class QuotePriceModel
    {
        public long UserId { get; set; }
        public long JobRequestId { get; set; }
        public decimal QuotePrice { get; set; }
    }
    public class wsBase
    {
        public bool status { get; set; }
        public string message { get; set; }
    }
    public class AcceptRejectQuoteModel
    {
        public long UserId { get; set; }
        public long JobRequestId { get; set; }
        public long NotificationId { get; set; }
        public bool IsAccept { get; set; }
    }
    public class UpdateTimerTimeModel
    {
        public long UserId { get; set; }
        public long JobRequestId { get; set; }
        public TimeSpan TimerTime { get; set; }
        public bool IsStart { get; set; }
    }
    public class CompleteJobRequestModel
    {
        public long UserId { get; set; }
        public long JobRequestId { get; set; }
    }
    public class UserReviewModel
    {
        public long? CustomerId { get; set; }
        public long? ToUserId { get; set; }
        public int? UserRating { get; set; }
        public int? JobRating { get; set; }
        public string UserReview { get; set; }
        public string JobReview { get; set; }
        public long? JobRequestId { get; set; }
    }
    public class JobRequestCheckListModel
    {
        public long Id { get; set; }
        public string TaskDetail { get; set; }
        public bool? IsDone { get; set; }

    }
}