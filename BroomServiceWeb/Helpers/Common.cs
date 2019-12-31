using BroomServiceWeb.Resources;
using BroomServiceWeb.Services;
using BroomServiceWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace BroomServiceWeb.Helpers
{
    public class Common
    {
        //This percetage will be collected by admin from customer 
        public static string payme_currency = "ils ";//"USD";// "nis";//: New israeli shekel
        public static string payme_sale_id = "MPL15619-69223SA9-DM43Q9NW-6QQMKYVN";
        public static string payme_sale_callback_url = "http://app.broomservice.co.il/Payment/Success";// "http://appmantechnologies.com:8063/Payment/Success";
        public static string payme_sale_return_url = "http://app.broomservice.co.il/Payment/Success";//"http://appmantechnologies.com:8063/Payment/Success";
        /*Staging	https://preprod.paymeservice.com/api/generate-sale
        Production https://ng.paymeservice.com/api/generate-sale
        */
        public static string payme_PaymentUrl = "https://ng.paymeservice.com/api/generate-sale";
        public static decimal GetFinalQuotePrice(decimal quotePrice)
        {
            decimal adminPer;
            UserService userService = new UserService();
            var data = userService.GetSettingData();
            if(data !=null)
            {
                adminPer = data.AdminPricePer;
            }
            else
            {
                adminPer = 20;
            }
            decimal price = 0;
            try
            {
                price = quotePrice+ (quotePrice * adminPer) / 100;
            }
            catch
            {

            }
            return price;
        }
        public static double GetHours(DateTime startTime, DateTime endtime)
        {
            double hours=0; 
            try
            { 
                TimeSpan ts = endtime - startTime;
                hours = Math.Round(ts.TotalHours,2);// ts.TotalHours;
            }
            catch
            {

            }
            return hours;
        }
        #region Notification
        public static string PushNotification(int? usertype,int? deviceid, string devicetoken, string message)
        {
            if (!string.IsNullOrEmpty(devicetoken))
            {
                string appTitle = usertype==(int)Enums.UserTypeEnum.Customer? Resource.broom_service : Resource.bs_crew;
                string serverKey = string.Empty;
                serverKey = "AAAAnh5ebpg:APA91bEqBRuehc3GaY7z1JMTqB56pmgzSNbmfC2e6q0gWey958AHLsrhxm74iaDZRKnoDctH52qtdId_i2yhC7nCUmhf00c8F0sMrgl-BVW1EVj89G_x0fXQ6DrpjANcHYBU2Vr1dcnF";
                var result21 = "-1";
                var webAddr = "https://fcm.googleapis.com/fcm/send";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Headers.Add("Authorization:key=" + serverKey);
                httpWebRequest.Method = "POST";
                using (var streamWriter = new System.IO.StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = string.Empty;
                    if (deviceid == 1)
                    {
                        /*
                         * {"to":"fQqnIIUt9nE:APA91bHDW1cnmp_g_FGt6T4OwZEE4y85DfYP0L-LGquEe40q__uuhPY9hT_KYtnREi5w2YXLnYwSuN5NfSq8VF9-GNIoCYEviNLArYb2e359hMlXYDVEHnlctV_xM_Mx6YSQzJM6MJYl","notification":{"title":"موجود","body":"rewtrwetwetwet"},"priority":"high"}
                         * */
                        json = "{\"to\":\"" + devicetoken + "\",\"notification\":{\"title\":\""+ appTitle + "\",\"body\":\"" + message + "\"},\"priority\":\"high\"}";
                        //json = "{\"to\": \"" + devicetoken + "\",\"data\": {\"message\": \"" + message + "\",}}";
                    }
                    else
                    {
                        json = "{\"to\": \"" + devicetoken + "\",\"notification\": {\"body\": \"" + message + "\",\"sound\": \"default\",\"content_available\": true}}";
                    }
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new System.IO.StreamReader(httpResponse.GetResponseStream()))
                {
                    result21 = streamReader.ReadToEnd();
                }
                return result21;
            }
            return null;
        }
        #endregion

        public static string EncryptString(string Password, string salt)
        {

            if (Password == null)
                return null;
            if (Password == "")
                return "";

            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(salt));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the encoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]
            byte[] DataToEncrypt = UTF8.GetBytes(Password);

            // Step 5. Attempt to encrypt the string
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            // Step 6. Return the encrypted string as a base64 encoded string
            return Convert.ToBase64String(Results);
        }
        public static string DecryptString(string EncryptedString, string salt)
        {
            if (EncryptedString == null)
                return null;
            if (EncryptedString == "")
                return "";

            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(salt));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the decoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]
            byte[] DataToDecrypt = Convert.FromBase64String(EncryptedString);

            // Step 5. Attempt to decrypt the string
            try
            {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            // Step 6. Return the decrypted string in UTF8 format
            return UTF8.GetString(Results);
        }
        public static void GeneratePassword(string p, string userType, ref string _salt, ref string _password)
        {
            if (userType == "new")
            {
                _salt = FetchRandString(6);
            }
            _password = EncryptString(p, _salt);
        }
        public static string FetchRandString(int size)
        {
            try
            {
                Random random = new Random();
                string combination = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
                char[] passwords = new char[size];
                for (int i = 0; i < size; i++)
                {
                    passwords[i] = combination[random.Next(0, combination.Length)];
                }
                return new string(passwords);
            }
            catch
            {
                throw;
            }
        }
    
        public static int SendOTPMessage(string newOtp, string phone)
        {
            try
            {
                string msg = "Your OTP is " + newOtp;
                WebClient client = new WebClient();
                string apiUrl = "http://www.rsaily.net/api/sendsms.php?username=0555895167&password=fksa2006&message="+ msg + "&numbers=+91"+phone+ "&sender=mawjood&unicode=e&Rmduplicated=1&return=Json";
                return Newtonsoft.Json.JsonConvert.DeserializeObject<int>(client.DownloadString(apiUrl)); 
            }
            catch
            {
                return 0;
            }
        }
        public static bool SendSignupConfirmationEmail(string firstname, string email, string userID)
        {
            bool result = true;
            try
            {
                string _path = "http://" + System.Web.HttpContext.Current.Request.Url.Authority;
                string _controllerPath = "/Admin/EmailVerification?userId=" + userID;

                var callbackUrl = "<a href=" + _path + _controllerPath + ">"+Resource.verify_email + "</a>";

                MailMessage mailmsg = new MailMessage();
                mailmsg.From = new MailAddress(ConfigurationManager.AppSettings["MAIL_FROM"].ToString());
                mailmsg.Subject = Resource.verify_email_broom_service;
                mailmsg.IsBodyHtml = true;
                mailmsg.Body = Resource.hello+" " + firstname + ",<br/><br/>";
                mailmsg.Body += "<b>"+Resource.click_on_link+
                        "<br/><br/>" + callbackUrl;
                mailmsg.Body += "<br />";
                mailmsg.Body += Resource.thank_you;

                AlternateView view = AlternateView.CreateAlternateViewFromString(mailmsg.Body, Encoding.UTF8, "text/html");
                mailmsg.AlternateViews.Add(view);

                string mailTo = email;
                MailAddress addressTo = new MailAddress(mailTo);
                if (mailTo.Length > 0)
                {
                    mailmsg.To.Add(addressTo.ToString());
                    var smtpHost = ConfigurationManager.AppSettings["SMTP_Host"].ToString();
                    var smtpPort = ConfigurationManager.AppSettings["SMTP_Port"].ToString();
                    SmtpClient SmtpServer = new SmtpClient(smtpHost);
                    SmtpServer.UseDefaultCredentials = true;
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Port = !string.IsNullOrEmpty(smtpPort) ? Convert.ToInt32(smtpPort) : 0;
                    //Please enter here user password
                    SmtpServer.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["MAIL_FROM"].ToString(),
                        ConfigurationManager.AppSettings["MAIL_PWD"].ToString());
                    SmtpServer.Send(mailmsg);
                }
                return result;
            }
            catch (Exception)
            {
                return result = false;
            }
        }
        public static bool SendEmailResetPassword(string firstname, string email, string password)
        {
            bool result = true;
            try
            {
                MailMessage mailmsg = new MailMessage();
                mailmsg.From = new MailAddress(ConfigurationManager.AppSettings["MAIL_FROM"].ToString());
                mailmsg.Subject = Resource.password_details;
                mailmsg.IsBodyHtml = true;
                mailmsg.Body = Resource.hello + " " + firstname + ",";
                mailmsg.Body += "<br />"+Resource.your_login_credentials;
                mailmsg.Body += "<br />";
                mailmsg.Body += Resource.email_post + " " + email + "<br />";
                mailmsg.Body += Resource.password_post + " " + password + "<br />";
                mailmsg.Body += "<br />";
                mailmsg.Body += Resource.thank_you;

                AlternateView view = AlternateView.CreateAlternateViewFromString(mailmsg.Body, Encoding.UTF8, "text/html");
                mailmsg.AlternateViews.Add(view);

                string mailTo = email;
                MailAddress addressTo = new MailAddress(mailTo);
                if (mailTo.Length > 0)
                {
                    mailmsg.To.Add(addressTo.ToString());
                    var smtpHost = ConfigurationManager.AppSettings["SMTP_Host"].ToString();
                    var smtpPort = ConfigurationManager.AppSettings["SMTP_Port"].ToString();
                    SmtpClient SmtpServer = new SmtpClient(smtpHost);
                    SmtpServer.UseDefaultCredentials = true;
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Port = !string.IsNullOrEmpty(smtpPort) ? Convert.ToInt32(smtpPort) : 0;
                    //Please enter here user password
                    SmtpServer.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["MAIL_FROM"].ToString(),
                        ConfigurationManager.AppSettings["MAIL_PWD"].ToString());
                    SmtpServer.Send(mailmsg);
                }
                return result;
            }
            catch (Exception ex)
            {
                return result = false;
            }
        }

       
        public static bool SendEmailUserCredentials(string usertype,string name, string email, string password)
        {
            bool result = true;
            try
            {
                MailMessage mailmsg = new MailMessage();
                mailmsg.From = new MailAddress(ConfigurationManager.AppSettings["MAIL_FROM"].ToString());
                mailmsg.Subject = Resource.user_credentials;
                mailmsg.IsBodyHtml = true;
                mailmsg.Body = "<html><head><title>"+Resource.login_credentials+"</title>" +
                    "<style>@media only screen and(max-width:620px){table[class=body] h1{font-size:28px !important;margin-bottom:10px !important;} table[class=body] p,table[class=body] ul,table[class=body] ol,table[class=body] td,table[class=body] span,table[class=body] a{font-size:16px !important;} table[class=body].wrapper,table[class=body].article{padding:10px !important;} table[class=body].content{padding:0 !important;} table[class=body].container{padding:0 !important;width:100% !important;} table[class=body].main{border-left-width:0 !important;border-radius:0 !important;border-right-width:0 !important;} table[class=body].btn table{width:100% !important;} table[class=body].btn a{width:100% !important;} table[class=body].img-responsive{height:auto !important;max-width:100% !important;width:auto !important;}} @media all{.ExternalClass{width:100%;}.ExternalClass,.ExternalClass p,.ExternalClass span,.ExternalClass font,.ExternalClass td,.ExternalClass div{line-height:100 %;} .apple-link a{color:inherit !important;font-family:inherit !important;font-size:inherit !important;font-weight:inherit !important;line-height:inherit !important;text-decoration:none !important;} .btn-primary table td:hover{background-color:#34495e !important;} .btn-primary a:hover{background-color:#34495e !important;border-color:#34495e !important;}}</style></head><body style=" + "background-color:#f6f6f6;font-family:sans-serif;-webkit-font-smoothing:antialiased;font-size:14px;line-height:1.4;margin:0;padding:0;-ms-text-size-adjust:100%;-webkit-text-size-adjust:100%;" + "><table border=" + "0" + " cellpadding=" + "0" + " cellspacing=" + "0" + "class=" + "body" + " style=" + "border-collapse:separate;mso-table-lspace:0pt;mso-table-rspace:0pt;width:100%;background-color:#f6f6f6;" + "><tr><td style=" + "font-family:sans-serif;font-size:14px;vertical-align:top;" + ">&nbsp;</td><td class=" + "container" + " style=" + "font-family:sans-serif;font-size:14px;vertical-align:top;display:block;Margin:0 auto;max-width:580px;padding:10px;width:580px;" + "><div class=" + "content" + " style=" + "box-sizing:border-box;display:block;Margin:0 auto;max-width:580px;padding:10px;" + "><table class=" + "main" + " style=" + "border-collapse:separate;mso-table-lspace:0pt;mso-table-rspace:0pt;width:100%;background:#ffffff;border-radius:3px;" + "><center><img style=" + "width:100px" + " src=" + "http://appmantechnologies.com:8063/images/logo.png" + "></center><tr><td class=" + "wrapper" + " style=" + "font-family:sans-serif;font-size:14px;vertical-align:top;box-sizing:border-box;padding:20px;" + "><table border=" + "0" + " cellpadding=" + "0" + " cellspacing=" + "0" + " style=" + "border-collapse:separate;mso-table-lspace:0pt;mso-table-rspace:0pt;width:100%;" + "><tr><td style=" + "font-family:sans-serif;font-size:14px;vertical-align:top;" + "><p style=" + "font-family:sans-serif;font-size:14px;font-weight:normal;margin:0;Margin-bottom:15px;" + "><strong>Dear </strong>" +
                    name + ",</p><p style=" + "font-family:sans-serif;font-size:14px;font-weight:bold;margin:0;margin-bottom:15px;" + ">"+Resource.broom_service_added+""+usertype+" "+Resource.in_there_portal +
                    "</p><table border=" + "0" + " cellpadding=" + "0" + " cellspacing=" + "0" + " class=" + "btn btn-primary" + "style=" + "border-collapse:separate;mso-table-lspace:0pt;mso-table-rspace:0pt;width:100%;box-sizing:border-box;" + "><tbody><tr>" +
                    "<td align=" + "left" + " style=" + "font-family:sans-serif;font-size:14px;vertical-align:top;padding-bottom:15px;" + "><table border=" + "0" + " cellpadding=" + "0" + " cellspacing=" + "0" + " style=" + "border-collapse:separate;mso-table-lspace:0pt;mso-table-rspace:0pt;width:auto;" + ">" +
                    "<tbody><tr><p style=" + "font-family:sans-serif;font-size:14px;font-weight:normal;margin:0;Margin-bottom:15px;" + ">"+Resource.your_login_credentials + "<br/><br/>"+Resource.email_post +"" + email + "<br/>"+Resource.password_post+" " + password + "<br/></tr></tbody></table></td></tr></tbody></table>" +
                    "<p style=" + "font-family:sans-serif;font-size:14px;font-weight:normal;margin:0;Margin-bottom:15px;" + ">Best Regards,<br />Broom Service</p></td></tr></table></td></tr></table></div></td><td style = " + "font-family:sans-serif;font-size:14px;vertical-align:top;" + "> &nbsp;</td></tr></table></body></html>";

                AlternateView view = AlternateView.CreateAlternateViewFromString(mailmsg.Body, Encoding.UTF8, "text/html");
                mailmsg.AlternateViews.Add(view);

                string mailTo = email;
                MailAddress addressTo = new MailAddress(mailTo);
                if (mailTo.Length > 0)
                {
                    mailmsg.To.Add(addressTo.ToString());
                    var smtpHost = ConfigurationManager.AppSettings["SMTP_Host"].ToString();
                    var smtpPort = ConfigurationManager.AppSettings["SMTP_Port"].ToString();
                    SmtpClient SmtpServer = new SmtpClient(smtpHost);
                    SmtpServer.UseDefaultCredentials = true;
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Port = !string.IsNullOrEmpty(smtpPort) ? Convert.ToInt32(smtpPort) : 0;
                    SmtpServer.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["MAIL_FROM"].ToString(),
                        ConfigurationManager.AppSettings["MAIL_PWD"].ToString());
                    SmtpServer.Send(mailmsg);
                }
                return result;
            }
            catch (Exception ex)
            {
                return result = false;
            }
        }
        #region Save Image

        public static bool SaveImage(string ImgStr, string ImgName)
        {
            String path = System.Web.HttpContext.Current.Server.MapPath("~/Images/JobRequest/"); //Path

            //Check if directory exist
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path); //Create directory if it doesn't exist
            }

            //set the image path
            string imgPath = Path.Combine(path, ImgName);

            byte[] imageBytes = Convert.FromBase64String(ImgStr);

            File.WriteAllBytes(imgPath, imageBytes);

            return true;
        }

        #endregion

        /// <summary>
        /// converting date to time ago function
        /// </summary>
        /// <param name="theDate"></param>
        /// <returns></returns>
        public static string RelativeDate(DateTime dtEvent)
        {
            TimeSpan TS = DateTime.Now - dtEvent;
            int intYears = DateTime.Now.Year - dtEvent.Year;
            int intMonths = DateTime.Now.Month - dtEvent.Month;
            int intDays = DateTime.Now.Day - dtEvent.Day;
            int intHours = DateTime.Now.Hour - dtEvent.Hour;
            int intMinutes = DateTime.Now.Minute - dtEvent.Minute;
            int intSeconds = DateTime.Now.Second - dtEvent.Second;
            if (intYears > 0) return String.Format("{0} {1} " + Resource.ago, intYears, (intYears == 1) ? Resource.year : Resource.years);
            else if (intMonths > 0) return String.Format("{0} {1} " + Resource.ago, intMonths, (intMonths == 1) ? Resource.month : Resource.months);
            else if (intDays > 0) return String.Format("{0} {1} " + Resource.ago, intDays, (intDays == 1) ? Resource.day : Resource.days);
            else if (intHours > 0) return String.Format("{0} {1} " + Resource.ago, intHours, (intHours == 1) ? Resource.hour : Resource.hours);
            else if (intMinutes > 0) return String.Format("{0} {1} " + Resource.ago, intMinutes, (intMinutes == 1) ? Resource.minute : Resource.minutes);
            else if (intSeconds > 0) return String.Format("{0} {1} " + Resource.ago, intSeconds, (intSeconds == 1) ? Resource.second : Resource.seconds);
            else
            {
                return String.Format("{0} {1} " + Resource.ago, dtEvent.ToShortDateString(), dtEvent.ToShortTimeString());
            }
        }

        public static Location GetLatLongFromAddress(string address)
        {
            Location getLatLong = null;
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load("https://maps.googleapis.com/maps/api/geocode/xml?key=AIzaSyCAW0qqikPYbOKLu_aobSw04z1Dnfhgpv4&address=" + address + "&sensor=false");
                XmlNode element = doc.SelectSingleNode("//GeocodeResponse/status");
                if (element.InnerText == "ZERO_RESULTS")
                {
                    getLatLong = new Location();
                    getLatLong.lat = "30.704649";
                    getLatLong.lng = "76.717873";
                    return getLatLong;
                }
                else
                {
                    getLatLong = new Location();

                    element = doc.SelectSingleNode("//GeocodeResponse/result/geometry/location");

                    var elementLat = element.SelectNodes("lat");
                    foreach (XmlNode child in elementLat)
                    {
                        getLatLong.lat = child.InnerText;
                    }

                    var elementLng = element.SelectNodes("lng");
                    foreach (XmlNode child in elementLng)
                    {
                        getLatLong.lng = child.InnerText;
                    }

                    return getLatLong;
                }

            }
            catch (Exception ex)
            {
                getLatLong = new Location();
                getLatLong.lat = "30.704649";
                getLatLong.lng = "76.717873";
                return getLatLong;
            }
        }
    }
}