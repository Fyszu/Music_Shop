using Music_Shop.Data;
using Music_Shop.Repositories;
using MySqlX.XDevAPI;
using System.Net;
using System.Text;
using System.Text.Json;
using XSystem.Security.Cryptography;

namespace Music_Shop.Services
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly TransactionRepository _repository;
        private readonly IConfiguration _configuration;
        public OrderService(ILogger<OrderService> logger, TransactionRepository transactionRepository, IConfiguration configuration)
        {
            _logger = logger;
            _repository = transactionRepository;
            _configuration = configuration;
        }


        public async Task<Order> GetById(int id) { return await _repository.GetById(id); }
        public async Task Add(Order transaction) { await _repository.Add(transaction); }
        public async Task Update(Order transaction) { await _repository.Update(transaction); }
        public async Task Remove(Order transaction) { await _repository.Remove(transaction); }
        public async Task<List<Order>> GetAll() { return await _repository.GetAll(); }
        public async Task<List<Order>> GetByBuyer(User buyer) { return await _repository.GetByBuyer(buyer); }
        public async Task<List<Order>> GetByAlbum(Album album) { return await _repository.GetByAlbum(album); }
        public async Task<List<Order>> GetByDateTime(DateTime dateTime) { return await _repository.GetByDateTime(dateTime); }
        public async Task<List<Order>> GetByPrice(float price) { return await _repository.GetByPrice(price); }

        public InternalResponse GetBearerToken()
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

            InternalResponse response = new();

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
                        {
                            response.Message = "Problem occured with JSON Deserialization: access token is null or empty.";
                            response.Status = InternalResponse.StatusCode.DeserializeProblem;
                        }
                        else
                        {
                            response.Message = oAuthResponse.Access_token;
                            response.Status = InternalResponse.StatusCode.Success;
                            oAuthResponse.AcquisitionTime = DateTime.Now;
                        }
                    }
                    else
                    {
                        response.Message = "Problem occured with JSON Deserialization: oAuthResponse is null.";
                        response.Status = InternalResponse.StatusCode.DeserializeProblem;
                    }
                }
                else
                {
                    response.Message = "Problem with getting bearer token - status code is not ok. Status code: " + httpResponse.StatusCode;
                    response.Status = InternalResponse.StatusCode.OtherError;
                }
            }
            catch (WebException ex)
            {
                if(ex.Response != null)
                {
                    string result;
                    using (var errorHttpResponse = (HttpWebResponse)ex.Response)
                        using (var streamReader = new StreamReader(errorHttpResponse.GetResponseStream()))
                            result = streamReader.ReadToEnd();
                    JsonSerializerOptions jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(result, jsonSerializerOptions);
                    if (errorResponse != null && errorResponse.Code != null)
                    {
                        switch(Int32.Parse(errorResponse.Code))
                        {
                            case 8011:
                                response.Status = InternalResponse.StatusCode.InvalidRequestValue;
                                response.Message = errorResponse.CodeLiteral;
                                break;

                            case 401:
                                response.Status = InternalResponse.StatusCode.Unauthorized;
                                response.Message = errorResponse.CodeLiteral;
                                break;

                            default:
                                response.Status = InternalResponse.StatusCode.OtherError;
                                response.Message = "Problem has occured while getting bearer token. Status code: " + errorResponse.Code;
                                break;
                        }
                    }
                    else
                    {
                        response.Message = "Problem has occured while getting bearer token:\n" + ex.Message;
                        response.Status = InternalResponse.StatusCode.OtherError;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = "Problem has occured while getting bearer token:\n" + ex.Message;
                response.Status = InternalResponse.StatusCode.OtherError;
            }
            return response;
        }

        public InternalResponse OrderRequest(Order order, string bearerAuth)
        {
            string url = _configuration.GetValue<string>("PayUBaseUrl");

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";

            httpRequest.ContentType = "application/json";
            httpRequest.Headers["Authorization"] = "Bearer " + bearerAuth;

            string httpRequestData = @"{
                ""notifyUrl"": """ + _configuration.GetValue<string>("NotifyUrl") + @""",
                ""customerIp"": """ + order.CustomerIp + @""",
                ""merchantPosId"": """ + _configuration.GetValue<int>("MerchantPosId") + @""",
                ""description"": """ + order.Description + @""",
                ""currencyCode"": """ + order.Currency + @""",
                ""totalAmount"": """ + order.GetTotalPrice() + @""",
                ""buyer"": {
                    ""email"": """ + order.Buyer.Email + @""",
                    ""phone"": """ + order.Buyer.Phone + @""",
                    ""firstName"": """ + order.Buyer.FirstName + @""",
                    ""lastName"": """ + order.Buyer.LastName + @""",
                    ""language"": """ + order.Buyer.Language + @"""
                },
                ";

            httpRequestData += @"""products"": [";
            foreach (Product product in order.Products)
            {
                httpRequestData += @"
                    {
                        ""name"": """ + product.Name + @""",
                        ""unitPrice"": """ + product.Price + @""",
                        ""quantity"": """ + product.Quantity + @"""
                    },";
            }
            httpRequestData = httpRequestData.Remove(httpRequestData.Length - 1); // Usunięcie zbędnego przecinka
            httpRequestData += "\n\t\t\t\t]\n}";

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(httpRequestData);
            }
            HttpWebResponse? httpResponse = null;
            InternalResponse response = new();

            try
            {
                httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    string result;
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        result = streamReader.ReadToEnd();
                    httpResponse.Close();
                }
                else
                {
                    response.Message = "Problem with placing order request - status code is not ok. Status code: " + httpResponse.StatusCode;
                    response.Status = InternalResponse.StatusCode.OtherError;
                }
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
                                response.Status = InternalResponse.StatusCode.InvalidRequestValue;
                                response.Message = errorResponse.CodeLiteral;
                                break;

                            case 401:
                                response.Status = InternalResponse.StatusCode.Unauthorized;
                                response.Message = errorResponse.CodeLiteral;
                                break;

                            default:
                                response.Status = InternalResponse.StatusCode.OtherError;
                                response.Message = "Problem has occured while placing order request. Status code: " + errorResponse.Code;
                                break;
                        }
                    }
                    else
                    {
                        response.Message = "Problem has occured while placing order request:\n" + ex.Message;
                        response.Status = InternalResponse.StatusCode.OtherError;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = "Problem has occured while placing order request:\n" + ex.Message;
                response.Status = InternalResponse.StatusCode.OtherError;
            }
            return response;
        }
    }
}
