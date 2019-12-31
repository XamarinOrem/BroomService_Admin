using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BroomServiceWeb.Resources;

namespace BroomServiceWeb.ViewModels
{
    public class CategoryViewModel
    {
        public long Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "enter_name")]
        public string Name { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "enter_name")]
        public string Name_Russian { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "enter_name")]
        public string Name_Hebrew { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "enter_name")]
        public string Name_French { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "please_enter_display_order")] 
        public int? DisplayOrder { get; set; }
        public string Description { get; set; }
        public string Description_Russian { get; set; }
        public string Description_Hebrew { get; set; }
        public string Description_French { get; set; }
        public string Picture { get; set; }
        public string Icon { get; set; }
        public bool HasPrice { get; set; }
        public bool ForWorkers { get; set; }
        public bool? IsActive { get; set; }
        public bool IsSelected { get; set; }
        public List<SubCategoryViewModel> SubCategories { get; set; }

    }
    public class CategoryViewModelApp
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Name_Russian { get; set; }
        public string Name_Hebrew { get; set; }
        public string Name_French { get; set; }
        public string Description { get; set; }
        public string Description_Russian { get; set; }
        public string Description_Hebrew { get; set; }
        public string Description_French { get; set; }
        public string Picture { get; set; }
        public string Icon { get; set; }
        public bool HasPrice { get; set; }
        public List<SubCategoryViewModelApp> SubCategories { get; set; }

    }
}