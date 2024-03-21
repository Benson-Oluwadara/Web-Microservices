using System.Collections.Generic;
using System.Threading.Tasks;
using Mango.Services.CouponAPI.Models.DTO;

namespace Mango.Services.CouponAPI.Service.IService
{
    public interface ICouponService
    {
        Task<IEnumerable<CouponDTO>> GetAllCouponsAsync();
        Task<CouponDTO> GetCouponByIdAsync(int id);
        Task<CouponDTO> CreateCouponAsync(CreateCouponDTO createCouponDto);
        Task<bool> UpdateCouponAsync(int id, UpdateCouponDTO updateCouponDto);
        Task<bool> DeleteCouponAsync(int id);
    }
}
