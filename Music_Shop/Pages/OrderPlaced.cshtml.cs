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
                Header = "Dziêkujê za zamówienie!";
                Message = "Zamówienie zosta³o pomyœlnie z³o¿one. Wkrótce zostanie wys³ane na podany wczeœniej adres.";
            }
            else if (Error != "501")
            {
                Header = "Coœ posz³o nie tak!";
                Message = $"Wyst¹pi³ niezidentyfikowany b³¹d (kod b³êdu: {Error}). Proszê ponowiæ zamówienie, a jeœli b³¹d wyst¹pi ponownie skontaktuj siê ze sprzedawc¹.";
            }
            else
            {
                Header = "Coœ posz³o nie tak!";
                Message = $"Zamówienie nie dosz³o do skutku. Proszê ponowiæ zamówienie, a jeœli b³¹d wyst¹pi ponownie skontaktuj siê ze sprzedawc¹.";
            }

        }
    }
}
