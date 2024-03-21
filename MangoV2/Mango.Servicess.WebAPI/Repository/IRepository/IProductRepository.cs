using System.Collections.Generic;
using System.Threading.Tasks;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.DTO;

namespace Mango.Services.ProductAPI.Repository.IRepository
{
    public interface IProductRepository
    {
        Task<int> CreateProductAsync(ProductCreateDTO createProductDto);
        Task<bool> DeleteProductAsync(int productId);
        Task<IEnumerable<Product>> GetAllProductAsync();
        Task<Product> GetProductByIdAsync(int productId);
        Task<bool> UpdateProductAsync(ProductUpdateDTO updateProductDto);
        Task<bool> UpdateProductAsync(int productId,ProductUpdateDTO updateProductDto);


    }
}
