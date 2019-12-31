using BroomServiceWeb.Helpers;
using BroomServiceWeb.Models;
using BroomServiceWeb.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BroomServiceWeb.Services
{
    public class PaymentService
    {
        BroomServiceEntities _db;
        UserService userService;
        public PaymentService()
        {
            _db = new BroomServiceEntities();
            userService = new UserService();
        }
        public string message;
        public bool PayOrder(string orderId)
        {

            var jobRequest = _db.JobRequests.Where(x => x.Id.ToString() == orderId).FirstOrDefault();  
            if (jobRequest != null)
            {
                jobRequest.IsPaymentDone = true;
                _db.SaveChanges(); 
                return true;
            }
            else
            {
                return false;
            }

        }
  
        public bool IsPaymentDone(long orderId)
        {
            var jobRequest = _db.JobRequests.Where(x => x.Id == orderId).FirstOrDefault();
         
            if (jobRequest != null)
            {
                 return jobRequest.IsPaymentDone??false;
            }
            else
            {
                return false;
            }
            
        }
    }
}