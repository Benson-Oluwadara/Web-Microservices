using Mango.Services.EmailAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Mango.Services.EmailAPI.Repository.IRepository
{
    public interface IEmailRepository
    {
        Task<int> CreateEmailAsync(EmailLogger email);
        //Task<bool> DeleteCouponAsync(int couponId);
        //Task<IEnumerable<Coupon>> GetAllCouponAsync();
        //Task<Coupon> GetCouponByIdAsync(int couponId);
        //Task<bool> UpdateCouponAsync(UpdateCouponDTO updateCouponDto);
        //Task<bool> UpdateCouponAsync(int couponId, UpdateCouponDTO updateCouponDto);


    }
}
