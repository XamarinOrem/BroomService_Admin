using BroomServiceWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BroomServiceWeb.Models
{
    public class ChatRequestModel
    {
        public ChatRequestModel()
        { 
        }
        public long? FromUserId { get; set; }
        public long? ToUserId { get; set; } 
    }
    public class ChatUser
    { 
        public long? UserId { get; set; }
        public string Name { get; set; }
        public string PicturePath { get; set; }
        public System.DateTime? CreatedDate { get; set; }
    }
    public class ChatListVM
    {
        public List<ChatUser> chatUser { get; set; }
        public List<ChatDetailListModel> listChat { get; set; }
    }
}