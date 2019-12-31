using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BroomServiceWeb.ViewModels
{
    public class NotificationViewModel
    {  
        public long Id { get; set; }
        public long? JobRequestId { get; set; }
        public string Text { get; set; }
        public long? FromUserId { get; set; }
        public string FromUserImage { get; set; }
        public bool NoOtherUser
        {
            get
            {
                char first = Text[0];
                if (first == ' ')
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public string FromUserName { get; set; }
        public long? ToUserId { get; set; }
        public string ToUserName { get; set; }
        public string ToUserImage { get; set; }
        public int? NotificationStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string  QuotePrice { get; set; }
    }

}