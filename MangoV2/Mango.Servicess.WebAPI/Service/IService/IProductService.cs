using System.Collections.Generic;
using System.Threading.Tasks;
using Mango.Services.ProductAPI.Models.DTO;

namespace Mango.Services.ProductAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllProductAsync();
        Task<ProductDTO> GetProductByIdAsync(int id);
        Task<ProductDTO> CreateProductAsync(ProductCreateDTO createProductDto);
        //Task<int> CreateProductAsync(CreateProductDTO createProductDto);
        Task<bool> UpdateProductAsync(int id, ProductUpdateDTO updateProductDto);
        Task<bool> DeleteProductAsync(int id);
    }
}
