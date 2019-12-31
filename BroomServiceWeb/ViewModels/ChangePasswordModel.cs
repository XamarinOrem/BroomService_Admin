using BroomServiceWeb.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BroomServiceWeb.ViewModels
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "enter_old_password")]
        public string oldPassword { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "enter_new_password")]
        public string newPassword { get; set; }
        [Compare("newPassword", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "password_confirm_mismatch")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "enter_confirm_password")]
        public string confirmPassword { get; set; }
        public long? userId { get; set; }
    }
}