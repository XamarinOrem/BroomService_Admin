using BroomServiceWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BroomServiceWeb.ViewModels
{
    public class HomeViewModel
    {
        public List<PropertyModel> Properties { get; set; }
        public List<JobRequestViewModel> MyBookings { get; set; }
        public String AboutUsText { get; set; }
        public List<CategoryViewModel> Categories { get; set; }
        public List<Testimonial> Testimonials { get; set; }
    }
}