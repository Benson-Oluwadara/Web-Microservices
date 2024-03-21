using mango.web.frontend.Models.WebDTO;

namespace mango.web.frontend.Services.Iservices
{
    public interface IProductService
    {
        Task<T> GetAllProductAsync<T>();
        Task<T> GetProductByIdAsync<T>(int productId);
        Task<T> CreateProductAsync<T>(CreateProductDTO createproductDTO);
        Task<T> UpdateProductAsync<T>(UpdateProductDTO updateproductDTO);
        Task<T> DeleteProductAsync<T>(int productId);
    }
    
}
