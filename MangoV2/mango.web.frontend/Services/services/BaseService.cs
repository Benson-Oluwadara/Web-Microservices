
using mango.web.frontend.Models;
using mango.web.frontend.Services.Iservices;
using mango.web.frontend.Utility;
using Newtonsoft.Json;
using System.Text;
using Serilog;
namespace mango.web.frontend.Services.services
{
    public class BaseService : IBaseService
    {
        public WebAPIResponse responseModel { get; set; }
        public IHttpClientFactory httpClient { get; set; }

        public BaseService(IHttpClientFactory httpClient)
        {
            this.httpClient = httpClient;
            this.responseModel = new WebAPIResponse();
        }

        public async Task<T> SendAsync<T>(WebAPIRequest apiRequest, string clientName)
        {
            try
            {
                var client = httpClient.CreateClient(clientName);
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(apiRequest.Url);
                UriBuilder uriBuilder = new UriBuilder(apiRequest.Url);
                //if (uriBuilder.Scheme == Uri.UriSchemeHttp)
                //{
                //    uriBuilder.Scheme = Uri.UriSchemeHttps;
                //}

                // Assign the modified URL to the request message
                message.RequestUri = uriBuilder.Uri;
                if (apiRequest.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data),
                        Encoding.UTF8, "application/json");
                }
                switch (apiRequest.apiType)
                {
                    case SD.ApiType.POST:
                        //Console.WriteLine("POST Request");
                        message.Method = HttpMethod.Post;
                        //log in this method from which client the request is coming
                        Log.Information($"Request from {clientName} to {apiRequest.Url}");
                        break;
                    case SD.ApiType.PUT:
                        //Console.WriteLine("PUT Request");
                        message.Method = HttpMethod.Put;
                        //log in this method from which client the request is coming
                        Log.Information($"Request from {clientName} to {apiRequest.Url}");
                        break;
                    case SD.ApiType.DELETE:
                        //Console.WriteLine("DELETE Request");
                        message.Method = HttpMethod.Delete;
                        //log in this method from which client the request is coming
                        Log.Information($"Request from {clientName} to {apiRequest.Url}");
                        break;
                    default:
                        //Console.WriteLine("GET Request");
                        message.Method = HttpMethod.Get;
                        //log in this method from which client the request is coming
                        Log.Information($"Request from {clientName} to {apiRequest.Url}");
                        break;

                }
                //Console.WriteLine($"Request URL: {message.RequestUri}");
                //Console.WriteLine($"Request Type: {apiRequest.apiType}");
                if (apiRequest.Data != null)
                {
                    //Console.WriteLine($"Request Payload: {JsonConvert.SerializeObject(apiRequest.Data)}");
                }

                HttpResponseMessage apiResponse = null;
                //if (!string.IsNullOrEmpty(apiRequest.Token))
                //{
                //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiRequest.Token);
                //}

                //Console.WriteLine("Send MEssage is:" + message.ToString());
                apiResponse = await client.SendAsync(message);

                var apiContent = await apiResponse.Content.ReadAsStringAsync();
                //print APIResponse
                //Console.WriteLine($"Response Content: {apiContent}");
                //Console.WriteLine($"Response Status Code: {apiResponse.StatusCode}");

                try
                {
                    WebAPIResponse ApiResponse = JsonConvert.DeserializeObject<WebAPIResponse>(apiContent);
                    //Console.WriteLine("APIResponse is:" + ApiResponse.ToString());

                    if (ApiResponse != null && (apiResponse.StatusCode == System.Net.HttpStatusCode.BadRequest
                        || apiResponse.StatusCode == System.Net.HttpStatusCode.NotFound))
                    {
                        //log in this method
                        Log.Error($"Error Occurred: {ApiResponse.ErrorMessages}");
                        ApiResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
                        ApiResponse.IsSuccess = false;
                        var res = JsonConvert.SerializeObject(ApiResponse);
                        var returnObj = JsonConvert.DeserializeObject<T>(res);
                        return returnObj;
                    }
                }
                catch (Exception e)
                {
                    var exceptionResponse = JsonConvert.DeserializeObject<T>(apiContent);
                    Log .Error($"Exception: {e.Message}");
                    return exceptionResponse;
                }
                var APIResponse = JsonConvert.DeserializeObject<T>(apiContent);

                return APIResponse;

            }
            catch (Exception e)
            {
                var dto = new WebAPIResponse
                {
                    //Convert.ToString(e.Message)
                    ErrorMessages = new List<string> { Convert.ToString(e.Message) },
                    IsSuccess = false
                };
                var res = JsonConvert.SerializeObject(dto);
                var APIResponse = JsonConvert.DeserializeObject<T>(res);
                if (e.InnerException != null)
                {
                    Log.Error($"Inner Exception: {e.InnerException.Message}");
                    //Console.WriteLine($"Inner Exception: {e.InnerException.Message}");
                }
                return APIResponse;
            }
        }


        public void Dispose()
        {
            GC.SuppressFinalize(true);
        }
    }
}

