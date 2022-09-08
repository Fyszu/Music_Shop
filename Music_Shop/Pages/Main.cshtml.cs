using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Music_Shop.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Music_Shop.Services;

namespace Music_Shop.Pages
{
    public class MainModel : PageModel
    {
        private readonly ILogger<MainModel> _logger;
        private readonly IOrderService _orderService;

        public MainModel(ILogger<MainModel> logger, IOrderService service)
        {
            _logger = logger;
            _orderService = service;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostSendOrderAsync()
        {
            string imie = Request.Form["Imie"];
            string nazwisko = Request.Form["Nazwisko"];
            string telefon = Request.Form["Telefon"];
            string email = Request.Form["Email"];
            string ulica = Request.Form["Ulica"];
            string kodPocztowy = Request.Form["Kod"];
            string miasto = Request.Form["Miasto"];
            string productName = Request.Form["Produkt"];
            int ilosc = Int32.Parse(Request.Form["Ilosc"]);

            User buyer = new User
            {
                Email = email,
                Phone = telefon,
                FirstName = imie,
                LastName = nazwisko,
                Street = ulica,
                City = miasto,
                PostalCode = kodPocztowy
            };

            int cena;
            if (productName == "Album")
                cena = 2900;
            else
                cena = 4850;

            Cart cart = new Cart();
            CartItem cartItem = new CartItem
            {
                Name = productName,
                Price = cena,
                Quantity = ilosc,
                Cart = cart
            };

            Order order = new Order()
            {
                Id = 3,
                Buyer = buyer,
                CustomerIp = "127.0.0.1"
            };

            order.Products.Add(cartItem);
            return Redirect(_orderService.GetPaymentRedirectionUrl(order));
        }
    }
}