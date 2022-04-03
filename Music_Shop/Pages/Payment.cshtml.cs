using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Music_Shop.Data;
using Music_Shop.Services;
using System.Configuration;
using System.Text;
using System.Web;

namespace Music_Shop.Pages
{
    [Authorize]
    public class PaymentModel : PageModel
    {
        private User _currentUser;
        private IUserService _userService;
        private ITransactionService _transactionService;
        private readonly IConfiguration _configuration;
        private readonly string _merchantKey;
        private readonly string _salt;
        private readonly string _payuBaseUrl;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentModel(IUserService userService, ITransactionService transactionService, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _transactionService = transactionService;
            _configuration = configuration;
            _merchantKey = _configuration.GetValue<string>("MerchantKey");
            _salt = _configuration.GetValue<string>("Salt");
            _payuBaseUrl = _configuration.GetValue<string>("PayUBaseUrl");
            _httpContextAccessor = httpContextAccessor;
        }
        public async void OnGet()
        {
            _currentUser = await _userService.GetByName(User.Identity.Name);
        }

        public void PayNow()
        {
            string albumDetails = "TBC";
            RemotePost myRemotePost = new RemotePost(_httpContextAccessor);
            var DomainName = _httpContextAccessor.HttpContext.Request.Host;
            string txnid = _transactionService.Generatetxnid();

            //Posting all the parameters required for payment.
            myRemotePost.Url = _payuBaseUrl;
            myRemotePost.Add("key", _merchantKey);
            myRemotePost.Add("txnid", txnid);
            myRemotePost.Add("amount", "1");
            myRemotePost.Add("productinfo", albumDetails);
            myRemotePost.Add("firstname", _currentUser.UserName);
            myRemotePost.Add("email", _currentUser.Email);
            myRemotePost.Add("surl", "http://" + DomainName + "/User/Return"); //Change the success url here depending upon the port number of your local system.
            myRemotePost.Add("furl", "http://" + DomainName + "/User/Return"); //Change the failure url here depending upon the port number of your local system.
            myRemotePost.Add("service_provider", "payu_paisa");

            string hashString = _merchantKey + "|" + txnid + "|" + 1 + "|" + albumDetails + "|" + _currentUser.UserName + "|" + _currentUser.Email + "|||||||||||" + _salt;
            string hash = _transactionService.Generatehash512(hashString);
            myRemotePost.Add("hash", hash);
            myRemotePost.Post();
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
                    merc_hash_string = _salt + "|" + form["status"].ToString();
                    foreach (string merc_hash_var in merc_hash_vars_seq)
                    {
                        merc_hash_string += "|";
                        merc_hash_string = merc_hash_string + (String.IsNullOrEmpty(form[merc_hash_var]) ? "" : form[merc_hash_var]);
                    }
                    _httpContextAccessor.HttpContext.Response.Body.Write(Encoding.UTF8.GetBytes(merc_hash_string));
                    merc_hash = _transactionService.Generatehash512(merc_hash_string).ToLower();
                    order_id = Request.Form["txnid"];
                }
                else
                {
                    //tokenContext.setPaymentMessageSession("Payment Fails! Order Pending to save.");
                }
            }
            catch (Exception ex)
            {
                ViewData["PaymentMessage"] = "Payment Fails! Order Pending to save.";
            }
            return RedirectToAction("ActionName");
        }


        public class RemotePost
        {
            private System.Collections.Specialized.NameValueCollection Inputs = new System.Collections.Specialized.NameValueCollection();

            public string Url = "";
            public string Method = "post";
            public string FormName = "form1";
            private readonly IHttpContextAccessor _httpContextAccessor;

            public RemotePost(IHttpContextAccessor httpContextAccessor)
            {
                _httpContextAccessor = httpContextAccessor;
            }
            public void Add(string name, string value)
            {
                Inputs.Add(name, value);
            }

            public async void Post()
            {
                _httpContextAccessor.HttpContext.Response.Clear();

                await _httpContextAccessor.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("<html><head>"));
                await _httpContextAccessor.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(string.Format("</head><body onload=\"document.{0}.submit()\">", FormName)));
                await _httpContextAccessor.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(string.Format("<form name=\"{0}\" method=\"{1}\" action=\"{2}\" >", FormName, Method, Url)));
                for (int i = 0; i < Inputs.Keys.Count; i++)
                {
                    await _httpContextAccessor.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(string.Format("<input name=\"{0}\" type=\"hidden\" value=\"{1}\">", Inputs.Keys[i], Inputs[Inputs.Keys[i]])));
                }
                await _httpContextAccessor.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("</form>"));
                await _httpContextAccessor.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("</body></html>"));

                //_httpContextAccessor.HttpContext.Response.Body.EndWrite();
            }
        }
    }


}
