using mango.web.frontend.Models;
using mango.web.frontend.Models.WebDTO;
using mango.web.frontend.Services.Iservices;
using mango.web.frontend.Utility;
using static System.Net.WebRequestMethods;
using Serilog;
namespace mango.web.frontend.Services.services
{
    public class CartService : BaseService,ICartService
    {
        private readonly IHttpClientFactory _clientFactory;
        public CartService(IHttpClientFactory clientFactory) : base(clientFactory)
        {
            _clientFactory = clientFactory;
        }


        public Task<T> ApplyCouponAsync<T>(CartDTO cartDto)
        {
            //Log the method function
            Log.Information("ApplyCouponAsync method called");
            //Log in this method from which client the request is coming
            Log.Information($"Request from ShoppingCartAPI to {SD.ShoppingCartAPIBase + "/api/cart/ApplyCoupon"}");
            
            
            //implement this method
            return this.SendAsync<T>(new WebAPIRequest()
            {
                apiType = SD.ApiType.POST,
                Data = cartDto,
                Url = SD.ShoppingCartAPIBase + "/api/cart/ApplyCoupon"
                
            }, "ShoppingCartAPI");
        }

        public async Task<T> EmailCart<T>(CartDTO cartDto)
        {
            //Log the method function
            Log.Information("EmailCart method called");
            //Log in this method from which client the request is coming
            Log.Information($"Request from ShoppingCartAPI to {SD.ShoppingCartAPIBase + "/api/cart/EmailCartRequest"}");
            return await this.SendAsync<T>(new WebAPIRequest()
            {
                apiType = SD.ApiType.POST,
                Data = cartDto,
                Url = SD.ShoppingCartAPIBase + "/api/cart/EmailCartRequest"
            }, "ShoppingCartAPI");
        }

        public Task<T> GetCartByUserIdAsnyc<T>(string userId)
        {
            //Log the method function
            Log.Information("GetCartByUserIdAsnyc method called");
            //Log in this method from which client the request is coming
            Log.Information($"Request from ShoppingCartAPI to {SD.ShoppingCartAPIBase + "/api/cart/GetCart/" + userId}");
            //log the URI
            //Console.WriteLine("URI is: " + SD.ShoppingCartAPIBase + "/api/cart/GetCart/" + userId);
            return this.SendAsync<T>(new WebAPIRequest()
            {
                apiType = SD.ApiType.GET,
                Url = SD.ShoppingCartAPIBase + "/api/cart/GetCart/" + userId
            }, "ShoppingCartAPI");
        }

        public Task<T> RemoveFromCartAsync<T>(int cartDetailsId)
        {
            //Log the method function
            Log.Information("RemoveFromCartAsync method called");
            //Log in this method from which client the request is coming
            Log.Information($"Request from ShoppingCartAPI to {SD.ShoppingCartAPIBase + "/api/cart/RemoveCart/" + cartDetailsId}");
            return this.SendAsync<T>(new WebAPIRequest()
            {
                apiType = SD.ApiType.DELETE,
                Url = SD.ShoppingCartAPIBase + "/api/cart/RemoveCart/" + cartDetailsId
            }, "ShoppingCartAPI");
        }

        public Task<T> UpsertCartAsync<T>(CartDTO cartDto)
        {
            //Log the method function
            Log.Information("UpsertCartAsync method called");
            //Log in this method from which client the request is coming
            Log.Information($"Request from ShoppingCartAPI to {SD.ShoppingCartAPIBase + "/api/cart/UpsertCart"}");
            
            //log the URI
            //Console.WriteLine("URI is: " + SD.ShoppingCartAPIBase + "/api/cart/UpsertCart");
            return this.SendAsync<T>(new WebAPIRequest()
            {
                apiType = SD.ApiType.POST,
                Data = cartDto,
                Url = SD.ShoppingCartAPIBase + "/api/cart/UpsertCart"
            }, "ShoppingCartAPI");
        }
    }
}
