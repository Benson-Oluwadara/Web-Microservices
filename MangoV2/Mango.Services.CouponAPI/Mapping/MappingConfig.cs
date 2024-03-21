using AutoMapper;
namespace Mango.Services.CouponAPI.Mapping
{
    public class MappingConfig: Profile
    {
        public MappingConfig()
        {
            CreateMap<Models.Coupon, Models.DTO.CouponDTO>().ReverseMap();
            CreateMap<Models.Coupon, Models.DTO.CreateCouponDTO>().ReverseMap();
            CreateMap<Models.Coupon, Models.DTO.UpdateCouponDTO>().ReverseMap();
        }
    }
}
