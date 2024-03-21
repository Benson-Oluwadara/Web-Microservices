using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.DTO;
using Mango.Services.CouponAPI.Repository.IRepository;
using Mango.Services.CouponAPI.Service.IService;

namespace Mango.Services.CouponAPI.Service.Service
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IMapper _mapper;

        public CouponService(ICouponRepository couponRepository, IMapper mapper)
        {
            _couponRepository = couponRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<CouponDTO>> GetAllCouponsAsync()
        {
            var coupons = await _couponRepository.GetAllCouponAsync();
            return _mapper.Map<IEnumerable<CouponDTO>>(coupons);
        }
        public async Task<CouponDTO> GetCouponByIdAsync(int id)
        {
            var coupon = await _couponRepository.GetCouponByIdAsync(id);
            return _mapper.Map<CouponDTO>(coupon);
        }
        public async Task<CouponDTO> CreateCouponAsync(CreateCouponDTO createCouponDto)
        {
            var couponEntity = _mapper.Map<Coupon>(createCouponDto);
            await _couponRepository.CreateCouponAsync(createCouponDto);
            return _mapper.Map<CouponDTO>(couponEntity);
        }
        public async Task<bool> UpdateCouponAsync(UpdateCouponDTO updateCouponDto)
        {
            return await _couponRepository.UpdateCouponAsync(updateCouponDto);
        }

        public async Task<bool> DeleteCouponAsync(int id)
        {
            return await _couponRepository.DeleteCouponAsync(id);
        }

        public async Task<bool> UpdateCouponAsync(int id, UpdateCouponDTO updateCouponDto)
        {
            return await _couponRepository.UpdateCouponAsync(id, updateCouponDto);
        }
    }
}
