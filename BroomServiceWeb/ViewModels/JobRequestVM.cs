using BroomServiceWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BroomServiceWeb.ViewModels
{
    public class JobRequestVM
    {
        public List<CategoryViewModel> Categories { get; set; }
        public JobRequestViewModel JobRequestData { get; set; }
    }
}