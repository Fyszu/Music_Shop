using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Music_Shop.Data;
using Music_Shop.Services;
using System.Configuration;
using System.Web;

namespace Music_Shop.Pages
{
    [Authorize]
    public class PaymentModel : PageModel
    {
        private User _currentUser;
        private IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly string _merchantKey;
        private readonly string _salt;
        private readonly string _payuBaseUrl;

        public PaymentModel(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
            _merchantKey = _configuration.GetValue<string>("MerchantKey");
            _salt = _configuration.GetValue<string>("Salt");
            _payuBaseUrl = _configuration.GetValue<string>("PayUBaseUrl");
        }
        public async void OnGet()
        {
            _currentUser = await _userService.GetByName(User.Identity.Name);
        }

        public void PayNow()
        {
            tokenContext.setPaymentMessageSession(null);
            ViewData["PaymentMessage"] = null;
            if (_currentUser != null)
            {
                string firstName = _currentUser.UserName;
                string productInfo = "Lab Product Purchase Online";
                string email = _currentUser.Email;
                RemotePost myremotepost = new RemotePost();
                var DomainName = HttpContext.Request.Host;
                //posting all the parameters required for integration.
                myremotepost.Url = _payuBaseUrl;
                myremotepost.Add("key", _merchantKey);
                string txnid = Generatetxnid();
                myremotepost.Add("txnid", txnid);
                myremotepost.Add("amount", 1);
                myremotepost.Add("productinfo", productInfo);
                myremotepost.Add("firstname", firstName);
                myremotepost.Add("email", email);
                myremotepost.Add("surl", "http://" + DomainName + "/User/Return");//Change the success url here depending upon the port number of your local system.
                myremotepost.Add("furl", "http://" + DomainName + "/User/Return");//Change the failure url here depending upon the port number of your local system.
                myremotepost.Add("service_provider", "payu_paisa");

                string hashString = _merchantKey + "|" + txnid + "|" + 1 + "|" + productInfo + "|" + firstName + "|" + email + "|||||||||||" + _salt;
                string hash = Generatehash512(hashString);
                myremotepost.Add("hash", hash);
                myremotepost.Post();
            }
        }

        [AllowAnonymous]
        public ActionResult Return(FormCollection form = null)
        {
            try
            {
                string[] merc_hash_vars_seq;
                string merc_hash_string = string.Empty;
                string merc_hash = string.Empty;
                string order_id = string.Empty;
                string hash_seq = "key|txnid|amount|productinfo|firstname|email|udf1|udf2|udf3|udf4|udf5|udf6|udf7|udf8|udf9|udf10";
                if (form["status"].ToString() == "success")
                {
                    merc_hash_vars_seq = hash_seq.Split('|');
                    Array.Reverse(merc_hash_vars_seq);
                    merc_hash_string = ConfigurationManager.AppSettings["SALT"] + "|" + form["status"].ToString();
                    foreach (string merc_hash_var in merc_hash_vars_seq)
                    {
                        merc_hash_string += "|";
                        merc_hash_string = merc_hash_string + (form[merc_hash_var] != null ? form[merc_hash_var] : "");
                    }
                    Response.Write(merc_hash_string);
                    merc_hash = Generatehash512(merc_hash_string).ToLower();
                    order_id = Request.Form["txnid"];
                }
                else
                {
                    tokenContext.setPaymentMessageSession("Payment Fails! Order Pending to save.");
                }
            }
            catch (Exception ex)
            {
                ViewData["PaymentMessage"] = "Payment Fails! Order Pending to save.";
            }
            return RedirectToAction("ActionName");
        }
    }


}
