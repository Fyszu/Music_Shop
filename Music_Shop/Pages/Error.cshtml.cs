using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Music_Shop.Data;
using System.Diagnostics;
using System.Web.Http;

namespace Music_Shop.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string? ExceptionMessage { get; set; }

        public void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            var exceptionHandlerPathFeature =
                HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionHandlerPathFeature.Error is JsonDeserializationException)
                ExceptionMessage = $"JSON deserialization problem. \n{exceptionHandlerPathFeature.Error.Message}";
            else if (exceptionHandlerPathFeature.Error is HttpResponseException)
                ExceptionMessage = "Getting bearer token has failed - HttpResponseException has been thrown. " +
                    $"Exception: {exceptionHandlerPathFeature.Error.Message}";
            else
                ExceptionMessage = exceptionHandlerPathFeature.Error.Message;

            if (string.IsNullOrEmpty(ExceptionMessage))
                ExceptionMessage = "Something went wrong. Exception was thrown, but no details are available.";
        }
    }
}