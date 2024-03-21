

using mango.web.frontend.Models.VM;
using mango.web.frontend.Models.WebDTO;

namespace mango.web.frontend.Services.Iservices
{
    public interface ICouponService
    {
        //Task<IEnumerable<CouponViewModel>> GetAllCouponsAsync();
        Task<T> GetAllCouponAsync<T>();
        Task<T> GetCouponByIdAsync<T>(int couponId);
        Task<T> CreateCouponAsync<T>(CreateCouponDTO couponDTO);
        //Task<bool> UpdateCouponAsync(int couponId, UpdateCouponDTO updateCouponDto);
        Task<T> UpdateCouponAsync<T>(UpdateCouponDTO couponDTO);
        Task<T> DeleteCouponAsync<T>(int couponId);
    }
}
