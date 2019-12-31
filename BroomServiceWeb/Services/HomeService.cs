using BroomServiceWeb.Helpers;
using BroomServiceWeb.Models;
using BroomServiceWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BroomServiceWeb.Services
{
    public class HomeService
    {
        PropertyService propertyService;
        public string message = string.Empty;
        BroomServiceEntities _db;

        public HomeService()
        {
            _db = new BroomServiceEntities();
            propertyService = new PropertyService();
        }

        public HomeViewModel GetHomeData(long userID)
        {
            HomeViewModel data = new HomeViewModel();
            try
            {
                var categories = _db.Categories.Where(x => x.IsActive == true).ToList();
                if (categories.Count > 0)
                {
                    data.Categories = categories.Select(x => new CategoryViewModel()
                    {
                        Name = x.Name,
                        Name_French=x.Name_French,
                        Name_Hebrew=x.Name_Hebrew,
                        Name_Russian=x.Name_Russian,
                        Picture = x.Picture,
                        Id = x.Id,
                        IsActive = x.IsActive
                    }).ToList();
                }

                var getAboutUsData = _db.AboutUs.FirstOrDefault();
                if (getAboutUsData != null)
                {
                    data.AboutUsText = getAboutUsData.Text;
                }

                data.Testimonials = _db.Testimonials.Where(A => A.IsActive == true).ToList();

                var getProperties = _db.Properties.Where(A => A.CreatedBy == userID)
                    .OrderByDescending(a=>a.CreatedDate).ToList();
                data.Properties = getProperties.Select(x => new PropertyModel()
                {
                    Name = x.Name,
                    Type = x.Type,
                    Address = x.Address,
                    ShortTermApartment = x.ShortTermApartment,
                    Id = x.Id
                }).ToList();

                var getBookings= propertyService.GetJobRequests(userID).Where
                    (a=>a.JobStatus==Enums.RequestStatus.InProgress.GetHashCode())
                    .OrderByDescending(a=>a.Id).ToList();

                data.MyBookings = getBookings;
            }
            catch (Exception ex)
            {
            }
            return data;
        }
    }
}