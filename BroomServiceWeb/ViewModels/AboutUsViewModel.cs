using BroomServiceWeb.Resources;
using System.ComponentModel.DataAnnotations;

namespace BroomServiceWeb.ViewModels
{
    public class AboutUsViewModel
    {
        public int AboutUsId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "please_enter_text")]
        public string Text { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Location { get; set; }
    }
}