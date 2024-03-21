using System.Reflection;
using AutoMapper;
using Mango.Services.Shopping_CartAPI.Models;
using Mango.Services.Shopping_CartAPI.Models.DTO;

namespace Mango.Services.Shopping_CartAPI.Mapping
{
    public class Mappingconfig:Profile
    {
        public Mappingconfig()
        {
            
            CreateMap<CartHeader,CartHeaderDTO>().ReverseMap();
            CreateMap<CartDetails, CartDetailsDTO>().ReverseMap();
        }
    }
}
