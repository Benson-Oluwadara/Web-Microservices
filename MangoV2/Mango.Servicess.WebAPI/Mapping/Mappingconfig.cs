using System.Reflection;
using AutoMapper;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.DTO;

namespace Mango.Servicess.WebAPI.Mapping
{
    public class Mappingconfig:Profile
    {
        public Mappingconfig()
        {
            
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<ProductCreateDTO, Product>().ReverseMap();
            CreateMap<ProductUpdateDTO, Product>().ReverseMap();
            

        }
    }
}
