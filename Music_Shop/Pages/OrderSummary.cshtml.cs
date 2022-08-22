using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Music_Shop.Data;
using Music_Shop.Services;

namespace Music_Shop.Pages
{
    public class OrderSummaryModel : PageModel
    {
        private Cart Cart { get; set; }
        private User? LoggedUser { get; set; }
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly ICartService _cartService;
        public OrderSummaryModel(IOrderService orderService, IUserService userService, CartService cartService)
        {
            _orderService = orderService;
            _userService = userService;
            _cartService = cartService;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            if (User.Identity != null && User.Identity.Name != null)
                LoggedUser = _userService.GetByName(User.Identity.Name).Result;
            if (LoggedUser == null)
                Cart = await _cartService.GetCartForGuest(HttpContext);
            else
                Cart = LoggedUser.Cart;

            // Get IP Address
            var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;
            string ipAddress;
            if (remoteIpAddress == null)
                ipAddress = "127.0.0.1";
            else
                ipAddress = remoteIpAddress.ToString();

            Order order = new()
            {
                CustomerIp = ipAddress,
                Buyer = LoggedUser,
                Products = Cart.Items,
                OrderDateTime = DateTime.Now
            };

            await _orderService.Add(order);

            return Redirect(_orderService.GetPaymentRedirectionUrl(order));
        }
    }
}
