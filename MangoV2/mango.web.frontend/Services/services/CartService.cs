using mango.web.frontend.Models;
using mango.web.frontend.Models.WebDTO;
using mango.web.frontend.Services.Iservices;
using mango.web.frontend.Utility;
using static System.Net.WebRequestMethods;

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
            return await this.SendAsync<T>(new WebAPIRequest()
            {
                apiType = SD.ApiType.POST,
                Data = cartDto,
                Url = SD.ShoppingCartAPIBase + "/api/cart/EmailCartRequest"
            }, "ShoppingCartAPI");
        }

        public Task<T> GetCartByUserIdAsnyc<T>(string userId)
        {
            //log the URI
            Console.WriteLine("URI is: " + SD.ShoppingCartAPIBase + "/api/cart/GetCart/" + userId);
            return this.SendAsync<T>(new WebAPIRequest()
            {
                apiType = SD.ApiType.GET,
                Url = SD.ShoppingCartAPIBase + "/api/cart/GetCart/" + userId
            }, "ShoppingCartAPI");
        }

        public Task<T> RemoveFromCartAsync<T>(int cartDetailsId)
        {
            
            return this.SendAsync<T>(new WebAPIRequest()
            {
                apiType = SD.ApiType.DELETE,
                Url = SD.ShoppingCartAPIBase + "/api/cart/RemoveCart/" + cartDetailsId
            }, "ShoppingCartAPI");
        }

        public Task<T> UpsertCartAsync<T>(CartDTO cartDto)
        {
            //log the URI
            Console.WriteLine("URI is: " + SD.ShoppingCartAPIBase + "/api/cart/UpsertCart");
            return this.SendAsync<T>(new WebAPIRequest()
            {
                apiType = SD.ApiType.POST,
                Data = cartDto,
                Url = SD.ShoppingCartAPIBase + "/api/cart/UpsertCart"
            }, "ShoppingCartAPI");
        }
    }
}
