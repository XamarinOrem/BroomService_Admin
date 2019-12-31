using BroomServiceWeb.Helpers;
using BroomServiceWeb.Resources;
using BroomServiceWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BroomServiceWeb.Models
{
    public class UserModel
    {
        public UserModel()
        { 
        }
        public long UserId { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public string PhoneNumber { get; set; }
        public long? CountryId { get; set; }
        public string CountryCode { get; set; }
        public string Location { get; set; }
        public string Image { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? UserType { get; set; }
        public bool? IsActive { get; set; } 
        public int? DeviceId { get; set; }
        public string DeviceToken { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; } 
        public virtual CountryModel Country { get; set; } 
        public string UserTypeText { get; set; } 
        public string CountryName { get; set; }
        public string PicturePath { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Zipcode { get; set; }
        public int? PaymentMethod { get; set; }
        public string  PaymentMethodName
        {
            get
            {
                return PaymentMethod!=null? Enums.GetPaymentMethod(PaymentMethod??1) :"";
            }
        }
    }
    public class UserViewModel
    {
        public long UserId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "enter_first_name")]
        public string FirstName { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "enter_last_name")]
        public string LastName { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "enter_email")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "please_enter_valid_email")]
        public string Email { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "enter_phone_number")]
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string UserTypeText { get; set; }
        public int? DeviceId { get; set; }
        public string DeviceToken { get; set; } 
        public int? UserType { get; internal set; } 
        public  bool? IsActive { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "new_password")]
        public string Password { get; set; }
        [Compare("Password", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "password_confirm_mismatch")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "enter_confirm_password")]
        public string ConfirmPassword { get; set; }
        public List<CategoryViewModel> Categories { get; set; }
        public string PicturePath { get; set; }
    }

    public class CustomerDetail
    {
        public UserViewModel User { get; set; }
        public List<PropertyModel> Properties { get; set; }
    }
    public class WorkerDetail
    {
        public UserViewModel User { get; set; }
        public List<JobRequestViewModel> Orders { get; set; }
    }
    public class UserExcelData
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Location { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string RegistrationNumber { get;  set; }
        public DateTime? ReviseTillDate { get; set; }
    }

    public class CustomerExcelData
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string RegistrationNumber { get; set; }
    }
    public class Base
    {
        public bool status { get; set; }
        public string message { get; set; }
    }
    public class OtpResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public string otp { get; set; }
        public long userId { get; set; }
        public string url { get; set; }
    }

    public class UpdatedData
    {
        public string ForgotPswdRequests { get; set; }
        public string AddedAmountRequests { get; set; }
    }
}