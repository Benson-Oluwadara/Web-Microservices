using System.Collections.Generic;
using System.Threading.Tasks;
using Mango.Services.Shopping_CartAPI.Models.DTO;

namespace Mango.Services.Shopping_CartAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetProducts();
    }
}
