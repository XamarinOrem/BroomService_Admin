using BroomServiceWeb.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BroomServiceWeb.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "enter_email")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "please_enter_valid_email")]
        public string Email { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "enter_password")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "enter_email")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "please_enter_valid_email")]
        public string Email { get; set; }
    }
    public class TestPushViewModel
    {
        public long UserId { get; set; }
        public string Message { get; set; }
    }
    public class ProfileViewModel
    {
        public long UserId { get; set; } 

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "enter_first_name")]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "enter_last_name")]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "enter_email")]
        [EmailAddress(ErrorMessage = "please_enter_valid_email")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "enter_phone_number")]
        public string PhoneNumber { get; set; }

    }
}