using Music_Shop.Data;
using Music_Shop.Repositories;
using MySqlX.XDevAPI;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Web.Http;
using XSystem.Security.Cryptography;

namespace Music_Shop.Services
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly OrderRepository _repository;
        private readonly IConfiguration _configuration;
        public OrderService(ILogger<OrderService> logger, OrderRepository orderRepository, IConfiguration configuration)
        {
            _logger = logger;
            _repository = orderRepository;
            _configuration = configuration;
        }


        public async Task<Order> GetById(int id) { return await _repository.GetById(id); }
        public async Task Add(Order transaction) { await _repository.Add(transaction); }
        public async Task Update(Order transaction) { await _repository.Update(transaction); }
        public async Task Remove(Order transaction) { await _repository.Remove(transaction); }
        public async Task<List<Order>> GetAll() { return await _repository.GetAll(); }
        public async Task<List<Order>> GetByBuyer(User buyer) { return await _repository.GetByBuyer(buyer); }
        public async Task<List<Order>> GetByDate(DateTime dateTime) { return await _repository.GetByDate(dateTime); }
        public async Task<List<Order>> GetBetweenTwoDates(DateTime from, DateTime to) { return await _repository.GetBetweenTwoDates(from, to); }
        public async Task<List<Order>> GetByPrice(int price) { return await _repository.GetByPrice(price); }

        private string GetBearerToken()
        {
            var url = _configuration.GetValue<string>("BearerUrl");

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/x-www-form-urlencoded";

            string clientId = _configuration.GetValue<string>("ClientId");
            string clientSecret = _configuration.GetValue<string>("ClientSecret");

            var httpRequestData = "grant_type=client_credentials&client_id=" + clientId + "&client_secret=" + clientSecret;

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                streamWriter.Write(httpRequestData);

            try
            {
                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    string result;
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        result = streamReader.ReadToEnd();
                    httpResponse.Close();
                    JsonSerializerOptions jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
                    OAuthResponse? oAuthResponse = JsonSerializer.Deserialize<OAuthResponse>(result, jsonSerializerOptions);
                    if (oAuthResponse != null)
                    {
                        if (string.IsNullOrEmpty(oAuthResponse.Access_token))
                            throw new JsonDeserializationException("Error while deserialization OAuthResponse: Access token is null.");
                        else
                            return oAuthResponse.Access_token;
                    }
                    else
                        throw new JsonDeserializationException("Error while deserialization OAuthResponse: deserialized object is null.");
                }
                else
                    throw new HttpResponseException(httpResponse.StatusCode);
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    string result;
                    using (var errorHttpResponse = (HttpWebResponse)ex.Response)
                    using (var streamReader = new StreamReader(errorHttpResponse.GetResponseStream()))
                        result = streamReader.ReadToEnd();
                    JsonSerializerOptions jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(result, jsonSerializerOptions);
                    if (errorResponse != null && errorResponse.Code != null)
                    {
                        switch (Int32.Parse(errorResponse.Code))
                        {
                            case 8011:
                                throw new Exception($"Error while getting bearer token: Invalid request value. Code literal: {errorResponse.CodeLiteral}");

                            case 401:
                                throw new Exception($"Error while getting bearer token: Unauthorized. Code literal: {errorResponse.CodeLiteral}");

                            default:
                                throw new Exception($"Error while getting bearer token: other error. Code literal: {errorResponse.CodeLiteral}, exception message: {ex.Message}");
                        }
                    }
                    else
                        throw new JsonDeserializationException("Error has occured when getting bearer token and deserialization of error message has failed. " +
                            $"Exception which has occured is: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while getting bearer token - other exception. Exception message: {ex.Message}");
            }
            return "";
        }

        private string GetPaymentRedirectionResponse(Order order, string bearerAuth)
        {
            string paymentRedirection = "";
            string url = _configuration.GetValue<string>("PayUBaseUrl");

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.AllowAutoRedirect = false;
            httpRequest.ContentType = "application/json";
            httpRequest.Headers["Authorization"] = "Bearer " + bearerAuth;

            string httpRequestData = @"{
                ""notifyUrl"": """ + _configuration.GetValue<string>("NotifyUrl") + @""",
                ""customerIp"": """ + order.CustomerIp + @""",
                ""merchantPosId"": """ + _configuration.GetValue<int>("MerchantPosId") + @""",
                ""description"": """ + order.Description + @""",
                ""additionalDescription"": """ + order.AdditionalDescription + @""",
                ""currencyCode"": """ + order.Currency + @""",
                ""totalAmount"": """ + order.TotalPrice + @""",
                ""continueUrl"": """ + _configuration.GetValue<string>("ReturnPage") +@""",
                ""buyer"": {
                    ""email"": """ + order.Buyer.Email + @""",
                    ""phone"": """ + order.Buyer.Phone + @""",
                    ""firstName"": """ + order.Buyer.FirstName + @""",
                    ""lastName"": """ + order.Buyer.LastName + @""",
                    ""language"": """ + order.Buyer.Language + @""",
                    ""delivery"": {
                        ""street"": """ + order.Buyer.Street + @""",
                        ""postalCode"": """ + order.Buyer.PostalCode + @""",
                        ""city"": """ + order.Buyer.City + @""",
                        ""recipientName"": """ + order.Buyer.FirstName + " " + order.Buyer.LastName + @""",
                        ""recipientEmail"": """ + order.Buyer.Email + @""",
                        ""recipientPhone"": """ + order.Buyer.Phone + @"""
                    }
                },
                ";

            httpRequestData += @"""products"": [";
            foreach (CartItem product in order.Products)
            {
                httpRequestData += @"
                    {
                        ""name"": """ + product.Name + @""",
                        ""unitPrice"": """ + product.Price + @""",
                        ""quantity"": """ + product.Quantity + @"""
                    },";
            }
            httpRequestData = httpRequestData.Remove(httpRequestData.Length - 1); // Usunięcie zbędnego przecinka
            httpRequestData += "\r\n\t\t]\r\n}";

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(httpRequestData);
            }
            HttpWebResponse? httpResponse = null;

            try
            {
                httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                if (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Redirect)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        paymentRedirection = streamReader.ReadToEnd();
                    httpResponse.Close();
                }
                else
                    throw new Exception($"Problem with placing order request - response status code is not ok. Status code: {httpResponse.StatusCode}");
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    string result;
                    using (var errorHttpResponse = (HttpWebResponse)ex.Response)
                    using (var streamReader = new StreamReader(errorHttpResponse.GetResponseStream()))
                        result = streamReader.ReadToEnd();
                    JsonSerializerOptions jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(result, jsonSerializerOptions);
                    if (errorResponse != null && errorResponse.Code != null)
                    {
                        switch (Int32.Parse(errorResponse.Code))
                        {
                            case 8011:
                                throw new Exception($"Error while placing order request: Invalid request value. Code literal: {errorResponse.CodeLiteral}, exception message: {ex.Message}");

                            case 401:
                                throw new Exception($"Error while placing order request: Unauthorized. Code literal: {errorResponse.CodeLiteral}, exception message: {ex.Message}");

                            default:
                                throw new Exception($"Error while placing order request: other error. Code literal: {errorResponse.CodeLiteral}, exception message: {ex.Message}");
                        }
                    }
                    else
                        throw new JsonDeserializationException($"Error occured while placing order request and error response or it's code is null. Exception message: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while placing order request: other error. Exception message: {ex.Message}");

            }
            return paymentRedirection;
        }

        private string DeserializePaymentRedirection(string jsonOrderRequestResponse)
        {
            JsonSerializerOptions jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
            RedirectionResponse? orderRedirectionResponse = JsonSerializer.Deserialize<RedirectionResponse>(jsonOrderRequestResponse, jsonSerializerOptions);
            string redirectionPageUrl = "";
            if (orderRedirectionResponse != null)
            {
                if (orderRedirectionResponse.Status.StatusCode == "SUCCESS")
                    redirectionPageUrl = orderRedirectionResponse.RedirectUri;
                else
                    throw new JsonDeserializationException($"Error while getting redirection page. Error code: {orderRedirectionResponse.Status.StatusCode}");
            }
            else
                throw new JsonDeserializationException($"Error while getting redirection page: deserialization has failed."); // przekierowanie do płatności
            return redirectionPageUrl;
        }

        public string GetPaymentRedirectionUrl(Order order)
        {
            return DeserializePaymentRedirection(GetPaymentRedirectionResponse(order, GetBearerToken()));
        }
    }
}
