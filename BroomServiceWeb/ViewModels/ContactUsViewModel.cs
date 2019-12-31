using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BroomServiceWeb.ViewModels
{
    public class ContactUsViewModel
    {
        public string Name { get; set; } 
        public string Email { get; set; }
        public string Message { get; set; }
        public long UserId { get; set; }
    } 
}