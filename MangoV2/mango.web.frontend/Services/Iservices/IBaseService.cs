using mango.web.frontend.Models;

namespace mango.web.frontend.Services.Iservices
{
    public interface IBaseService: IDisposable
    {
        WebAPIResponse responseModel { get; set; }
        public Task<T> SendAsync<T>(WebAPIRequest apiRequest, string clientName);
    }
}
