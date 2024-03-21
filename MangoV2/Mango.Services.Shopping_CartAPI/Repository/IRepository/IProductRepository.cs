using System.Collections.Generic;
using System.Threading.Tasks;
using Mango.Services.Shopping_CartAPI.Models;
using Mango.Services.Shopping_CartAPI.Models.DTO;

namespace Mango.Services.Shopping_CartAPI.Repository.IRepository
{
    public interface IProductRepository
    {

        Task<IEnumerable<ProductDTO>> GetProductsAsync();

    }
}
