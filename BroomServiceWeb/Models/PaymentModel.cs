using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BroomServiceWeb.Models
{
    public class PaymentResponseModel
    {
        public int status_code { get; set; }
        public string payme_sale_id { get; set; }
        public int payme_sale_code { get; set; }
        public int price { get; set; }
        public string transaction_id { get; set; }
        public string currency { get; set; }
        public string sale_payment_method { get; set; }
        public int status_error_code { get; set; }
        public object status_error_details { get; set; }
        public string redirect_url { get; set; }
        public string transaction_cc_auth_number { get; set; }
        public string payme_transaction_auth_number { get; set; }
        public string sale_status { get; set; }
        public string payme_status { get; set; }
        public string sale_created { get; set; }
        public string payme_sale_status { get; set; }
        public bool is_token_sale { get; set; }
        public string payme_signature { get; set; }
        public string payme_transaction_id { get; set; }
        public string payme_transaction_total { get; set; }
        public string payme_transaction_card_brand { get; set; }
        public string payme_transaction_voucher { get; set; }
        public string buyer_name { get; set; }
        public string buyer_email { get; set; }
        public string buyer_phone { get; set; }
        public string buyer_card_mask { get; set; }
        public string buyer_card_exp { get; set; }
        public bool buyer_card_is_foreign { get; set; }
        public string buyer_social_id { get; set; }
        public int installments { get; set; }
        public string sale_paid_date { get; set; }
        public object sale_release_date { get; set; }

    }
    public class PaymentRequestModel
    {
        public string seller_payme_id { get; set; }
        public decimal sale_price { get; set; }
        public string currency { get; set; }
        public string product_name { get; set; }
        public string transaction_id { get; set; }
        public int installments { get; set; }
        public string sale_callback_url { get; set; }
        public string sale_return_url { get; set; }
        public string buyer_key { get; set; }
        public int capture_buyer { get; set; }
        public string payme_sale_id { get; set; }
    }
}