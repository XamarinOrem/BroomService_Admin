using BroomServiceWeb.Controllers.Web;
using BroomServiceWeb.Helpers;
using BroomServiceWeb.Models;
using BroomServiceWeb.Resources;
using BroomServiceWeb.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BroomServiceWeb.Controllers
{
    public class PaymentController : MyController
    {
        PaymentService paymentService;
        UserService userService;
        public PaymentController()
        {
            paymentService = new PaymentService();
            userService = new UserService();
        }
        [HttpGet]
        public ActionResult DoPayment(long userId,long requestId,decimal price)
        {
            if (!paymentService.IsPaymentDone(requestId))
            { 
                var userData = userService.GetUserDetail(userId);
                ViewBag.requestId = requestId;
                ViewBag.price = price;
                return View(userData);
            }
            else
            {
                return HttpNotFound(); 
            }
        
        }
        // GET: Payment
        [HttpGet]
        public ActionResult Success(string payme_status,string payme_signature,string payme_sale_id,string payme_transaction_id,int? price,string currency
            , string transaction_id,int? is_token_sale,int? is_foreign_card)
        {
            string status = "";
            string message = "";
            if (payme_status == "success")
            {
                if(paymentService.PayOrder(transaction_id))
                {
                    status = "success";
                    message = Resource.payment_success;
                }
                else
                {
                    status = "failure";
                    message = Resource.transaction_does_not_exists;
                }
            }
            else
            {
                status = "failure";
                message = Resource.some_error_occured;
            }
            ViewBag.Status = status;
            ViewBag.Message = message; 
            return View();
        }
        #region Payment
        [HttpPost]
        public async Task<JsonResult> Payment(long requestId, decimal price, string buyer_key)
        {
            var responseData = await PayOrder(requestId, price, buyer_key);

            return Json(responseData, JsonRequestBehavior.AllowGet);
        }

        async Task<PaymentResponseModel> PayOrder(long requestId, decimal price, string buyer_key)
        {
            PaymentResponseModel responseData = new PaymentResponseModel();


            System.Threading.CancellationTokenSource cts;
            HttpClient httpClient = new HttpClient();
            TimeSpan time = new TimeSpan(0, 0, 60);
            httpClient.Timeout = time;
            cts = new System.Threading.CancellationTokenSource();
            cts.CancelAfter(time);

            PaymentRequestModel model = new PaymentRequestModel()
            {
                buyer_key = buyer_key,
                capture_buyer = 0,
                currency = Common.payme_currency,
                installments = 1,
                payme_sale_id = Common.payme_sale_id,
                product_name = "Order #" + requestId,
                sale_callback_url = Common.payme_sale_callback_url,
                sale_price = 1,//price,
                sale_return_url = Common.payme_sale_return_url,
                seller_payme_id = Common.payme_sale_id,
                transaction_id = requestId.ToString(),// "12345",
            };
            string postData = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            string url = Common.payme_PaymentUrl;
            StringContent str = new StringContent(postData, System.Text.Encoding.UTF8, "application/json");
            var result = await httpClient.PostAsync(new Uri(url), str);
            var place = result.Content.ReadAsStringAsync().Result;
            // deserialization of the response returning from the api
            responseData = Newtonsoft.Json.JsonConvert.DeserializeObject<PaymentResponseModel>
                (await result.Content.ReadAsStringAsync());
            return responseData;
        }
        #endregion
    }
}