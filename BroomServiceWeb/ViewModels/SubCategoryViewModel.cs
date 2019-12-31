using BroomServiceWeb.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BroomServiceWeb.ViewModels
{
    public class SubCategoryViewModel
    {
        public long Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "enter_name")]
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "please_choose_category")]
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string Description_Russian { get; set; }
        public string Description_Hebrew { get; set; }
        public string Description_French { get; set; }
        public decimal? Price { get; set; }
        public decimal? ClientPrice { get; set; }
        public string HasPrice { get; set; }
        public bool IsSelected { get; set; }
        public string Picture { get; set; }
        public string Icon { get; set; }
        public bool HasSubSubCategories { get; set; }
        public string HasSubSubCategoriesStr { get; set; }
        public List<SubSubCategoryViewModel> SubSubCategories { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "enter_name")]
        public string Name_Russian { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "enter_name")]
        public string Name_Hebrew { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "enter_name")]
        public string Name_French { get; set; }
    }
    public class SubCategoryViewModelApp
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
        public decimal? Price { get; set; }
        public decimal? ClientPrice { get; set; }
        public string Picture { get; set; }
        public string Icon { get; set; }
        public bool HasSubSubCategories { get; set; }
        public List<SubSubCategoryViewModelApp> SubSubCategories { get; set; }
    }
}