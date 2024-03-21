using System.Collections.Generic;
using System.Threading.Tasks;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.DTO;

namespace Mango.Services.CouponAPI.Repository.IRepository
{
    public interface ICouponRepository
    {
        Task<int> CreateCouponAsync(CreateCouponDTO createCouponDto);
        Task<bool> DeleteCouponAsync(int couponId);
        Task<IEnumerable<Coupon>> GetAllCouponAsync();
        Task<Coupon> GetCouponByIdAsync(int couponId);
        Task<bool> UpdateCouponAsync(UpdateCouponDTO updateCouponDto);
        Task<bool> UpdateCouponAsync(int couponId, UpdateCouponDTO updateCouponDto);


    }
}
