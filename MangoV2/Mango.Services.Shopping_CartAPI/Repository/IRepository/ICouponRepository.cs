using Mango.Services.Shopping_CartAPI.Models.DTO;

namespace Mango.Services.Shopping_CartAPI.Repository.IRepository
{
    public interface ICouponRepository
    {
        public Task<CouponDTO> GetCouponAsync(string couponCode);
    }
}
