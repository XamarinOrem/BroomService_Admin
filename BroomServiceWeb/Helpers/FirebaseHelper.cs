using BroomServiceWeb.ViewModels;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BroomServiceWeb.Helpers
{
    public static class FirebaseHelper
    {
        static FirebaseClient firebase = new FirebaseClient("https://broomserviceapp-1cf32.firebaseio.com/");

        public static async Task<List<ChatDetailListModel>> GetChatForUserID(long senderUserId, long recieverUserId)
        {
            return (await firebase
              .Child("Chat").Child(senderUserId.ToString()).Child(recieverUserId.ToString())
              .OnceAsync<ChatDetailListModel>()).Select(item => new ChatDetailListModel
              {
                  IsSender = item.Object.IsSender,
                  RecieverUserId = item.Object.RecieverUserId,
                  SenderUserId = item.Object.SenderUserId,
                  UserMessage = item.Object.UserMessage,
                  UserMessageTime = item.Object.UserMessageTime,
                  TimeStamp=item.Object.TimeStamp
              }).OrderBy(a=>a.TimeStamp).ToList();            
        }

        public static async Task<bool> AddChatMessage(ChatDetailListModel chatModel )
        { 
            try
            {
                await firebase
                      .Child("Chat").Child(chatModel.SenderUserId.ToString()).Child(chatModel.RecieverUserId.ToString()).PostAsync(chatModel);
                var chatModel1 = new ChatDetailListModel()
                {
                    IsSender = !chatModel.IsSender,
                    RecieverUserId = chatModel.SenderUserId,
                    SenderUserId = chatModel.RecieverUserId,
                    UserMessage = chatModel.UserMessage,
                    UserMessageTime = chatModel.UserMessageTime,
                    TimeStamp=chatModel.TimeStamp
                };
                await firebase
                      .Child("Chat").Child(chatModel1.SenderUserId.ToString()).Child(chatModel1.RecieverUserId.ToString()).PostAsync(chatModel1);
                 
                    return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("AddingChatToFirebase_Exception:- " + ex.Message);
                return false;
            }
        }
    }
}