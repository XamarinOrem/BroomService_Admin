using BroomServiceWeb.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BroomServiceWeb.Helpers
{
    public class Enums
    {
        public enum PaymentMethod
        {
            ByCreditCard = 1,
            ByCash = 2,
            ByCheque = 3,
            ByMoneyTransfer = 4
        }
        public static string GetPaymentMethod(int method)
        {
            string status = string.Empty;
            if (method == (int)PaymentMethod.ByCash)
            {
                status = Resource.by_cash;
            }
            else if (method == (int)PaymentMethod.ByCheque)
            {
                status = Resource.by_cheque;
            }
            else if (method == (int)PaymentMethod.ByMoneyTransfer)
            {
                status = Resource.by_money_transfer;
            }
            else  
            {
                status = Resource.by_credit_card;
            }
            return status;
        }
        public enum UserTypeEnum
        {
            Admin = 1,
            ServiceProvider = 2,
            Worker =3,
            Customer = 4            
        }
        public enum RequestStatus
        {
            Pending = 1,
            InProgress = 2,
            Completed = 3,
            Canceled = 4,
            QuoteCanceled = 5
        }
        public static string GetRequestStatus(int jobStatus)
        {
            string status = string.Empty;
            if(jobStatus ==(int) RequestStatus.Canceled)
            {
                status = RequestStatus.Canceled.ToString();
            }
            else if (jobStatus == (int)RequestStatus.Completed)
            {
                status = RequestStatus.Completed.ToString();
            }
            else if (jobStatus == (int)RequestStatus.InProgress)
            {
                status = RequestStatus.InProgress.ToString();
            }
            else if (jobStatus == (int)RequestStatus.QuoteCanceled)
            {
                status = Resource.quote_canceled;
            }
            else
            {
                status = RequestStatus.Pending.ToString();
            }
            return status;
        }
        public enum NotificationStatus
        {
            Pending = 1,
            Accepted = 2,
            Rejected = 3,
            SentQuotation = 4,
            AcceptedQuotation = 5,
            RejectedQuotation = 6,
            TimerStarted =7,
            TimerEnded = 8,
            Completed =9,
            Assigned = 10
        }
    }
}