using AutoMapper;
using mango.web.frontend.Models.WebDTO;

namespace mango.web.frontend.Mapping
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<CartHeader, CartHeaderDTO>().ReverseMap();
            CreateMap<CartDetails, CartDetailsDTO>().ReverseMap();
            CreateMap<Product, ProductDTO>().ReverseMap();
        }
    }
}


