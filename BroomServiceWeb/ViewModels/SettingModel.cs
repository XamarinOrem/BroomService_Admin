using BroomServiceWeb.Resources;
using System.ComponentModel.DataAnnotations;

namespace BroomServiceWeb.ViewModels
{
    public class SettingModel
    {
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "please_enter_price_prcnt")]
        public decimal AdminPricePer { get; set; } 
    }
}