using AutoMapper;
using Mango.Services.ShoppingCartAPI.Models.DTO;
namespace Mango.Services.ShoppingCartAPI.Models.DTO
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<CartHeader, CartHeaderDTO>().ReverseMap();
            CreateMap<CartDetails, CartDetailsDTO>().ReverseMap();
            CreateMap<Product, ProductDTO>().ReverseMap();
        }
    }
}
