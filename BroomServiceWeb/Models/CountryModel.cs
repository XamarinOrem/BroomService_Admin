using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BroomServiceWeb.Models
{
    public class CountryModel
    {
        public CountryModel()
        {
            this.Users = new HashSet<UserModel>();
        }

        public long CountryId { get; set; }
        
        public string CountryName { get; set; }
        [Required]
        public int? DisplayOrder { get; set; }
        public bool? IsActive { get; set; }
        public string CurrencyCode { get; set; }
        public string CountryCode { get; set; }
        public virtual ICollection<UserModel> Users { get; set; }
    }

    public class CountryViewModel
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string CurrencyCode { get; set; }
    }
}