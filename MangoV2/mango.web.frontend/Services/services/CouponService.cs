
using mango.web.frontend.Models;
using mango.web.frontend.Models.VM;
using mango.web.frontend.Models.WebDTO;
using mango.web.frontend.Services.Iservices;
using mango.web.frontend.Services.services;
using mango.web.frontend.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;
using static mango.web.frontend.Utility.SD;
using static System.Net.WebRequestMethods;

namespace mango.web.frontend.Services.Services{
        public class CouponService : BaseService, ICouponService
            {
            
            private readonly IHttpClientFactory _clientFactory;
            public CouponService(IHttpClientFactory clientFactory) : base(clientFactory)
            {
                _clientFactory = clientFactory;
            }

        
        public Task<T> CreateCouponAsync<T>(CreateCouponDTO couponDTO)
        {
            Console.WriteLine("Create All Coupons!!!!!!!!!!!!!!!!!!!!!!!!!!\n!!!!!!!!!!!!!!!!!!!!!");

            return this.SendAsync<T>(new WebAPIRequest()
            {
                apiType = SD.ApiType.POST,
                Data = couponDTO,
                Url = SD.CouponAPIBase + "/api/coupons"
            }, "CouponAPI");
        }

        public Task<T> DeleteCouponAsync<T>(int couponId)
        {
            return this.SendAsync<T>(new WebAPIRequest()
            {
                apiType = SD.ApiType.DELETE,
                Url = SD.CouponAPIBase + "/api/coupons/" + couponId
            }, "CouponAPI");
        }


        public Task<T> GetAllCouponAsync<T>()
        {
            Console.WriteLine("Get All Coupons!!!!!!!!!!!!!!!!!!!!!!!!!!\n!!!!!!!!!!!!!!!!!!!!!");
            return this.SendAsync<T>(new WebAPIRequest()
            {
                apiType = SD.ApiType.GET,
                Url = SD.CouponAPIBase + "/api/coupons"
            }, "CouponAPI");
        }


        public Task<T> GetCouponByIdAsync<T>(int couponId)
        {
            return this.SendAsync<T>(new WebAPIRequest()
            {
                apiType = SD.ApiType.GET,
                Url = SD.CouponAPIBase + "/api/coupons/" + couponId,
            }, "CouponAPI");
        }


        public Task<T> UpdateCouponAsync<T>(UpdateCouponDTO couponDTO)
        {
            return this.SendAsync<T>(new WebAPIRequest()
            {
                apiType = SD.ApiType.PUT,
                Data = couponDTO,
                Url = SD.CouponAPIBase + "/api/coupons/"+ couponDTO.CouponId,
            }, "CouponAPI");
        }

        
    }
}
