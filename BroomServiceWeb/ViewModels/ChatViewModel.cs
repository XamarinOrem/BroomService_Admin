using System.ComponentModel.DataAnnotations;

namespace BroomServiceWeb.ViewModels
{
    public class ChatViewModel
    { 
        public string Text { get; set; }
        public long FromUserId { get; set; }
        public long ToUserId { get; set; } 
    }
    public class ChatDetailListModel
    {
        //[JsonProperty("FromUserId")]
        public long SenderUserId { get; set; }
        //[JsonProperty("ToUserId")]
        public long RecieverUserId { get; set; }
        //[JsonIgnore]
        public string UserMessage { get; set; }
        //[JsonIgnore]
        public string UserMessageTime { get; set; }
        //[JsonIgnore]
        public bool IsSender { get; set; }
        public object TimeStamp { get; set; }
    }
}