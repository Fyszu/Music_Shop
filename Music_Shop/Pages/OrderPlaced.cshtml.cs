using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Music_Shop.Pages
{
    public class OrderPlacedModel : PageModel
    {
        [FromQuery(Name = ("error"))]
        public string? Error { get; set; }
        public string Header { get; set; }
        public string Message { get; set; }
        public void OnGet()
        {
            if (string.IsNullOrEmpty(Error))
            {
                Header = "Dzi�kuj� za zam�wienie!";
                Message = "Zam�wienie zosta�o pomy�lnie z�o�one. Wkr�tce zostanie wys�ane na podany wcze�niej adres.";
            }
            else if (Error != "501")
            {
                Header = "Co� posz�o nie tak!";
                Message = $"Wyst�pi� niezidentyfikowany b��d (kod b��du: {Error}). Prosz� ponowi� zam�wienie, a je�li b��d wyst�pi ponownie skontaktuj si� ze sprzedawc�.";
            }
            else
            {
                Header = "Co� posz�o nie tak!";
                Message = $"Zam�wienie nie dosz�o do skutku. Prosz� ponowi� zam�wienie, a je�li b��d wyst�pi ponownie skontaktuj si� ze sprzedawc�.";
            }

        }
    }
}
