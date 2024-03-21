using Mango.Services.Shopping_CartAPI.Models.DTO;
using Mango.Services.Shopping_CartAPI.Repository.IRepository;
using Mango.Services.Shopping_CartAPI.Service.IService;
using Newtonsoft.Json;

namespace Mango.Services.Shopping_CartAPI.Service.Service
{
    public class CouponService: ICouponService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CouponService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<CouponDTO> GetCoupon(string couponCode)
        {
            var client = _httpClientFactory.CreateClient("CouponAPI");
            var response = await client.GetAsync($"api/coupon/{couponCode}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var coupon = JsonConvert.DeserializeObject<CouponDTO>(content);
                return coupon;
                
            }
            return new CouponDTO();
        }

    }
}
