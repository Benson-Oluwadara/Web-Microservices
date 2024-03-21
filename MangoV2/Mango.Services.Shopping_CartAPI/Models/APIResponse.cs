using System.Net;

namespace Mango.Services.Shopping_CartAPI.Models
{
    public class APIResponse
    {
        public APIResponse()
        {
        }

        //ctor with parameters
        public APIResponse(HttpStatusCode statusCode, bool isSuccess, object result, List<string> errorMessages = null)
        {
            StatusCode = statusCode;
            IsSuccess = isSuccess;
            Result = result;
            ErrorMessages = errorMessages;
        }

        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public List<string> ErrorMessages { get; set; }
        public object Result { get; set; }
    }
}
