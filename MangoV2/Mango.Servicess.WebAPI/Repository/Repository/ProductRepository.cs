using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Mango.Services.ProductAPI.Database.IDapperRepositorys;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.DTO;
using Mango.Services.ProductAPI.Repository.IRepository;

namespace Mango.Services.ProductAPI.Repository.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDapperRepository _dapperRepository;

        public ProductRepository (IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }
        
        public async Task<int> CreateProductAsync(ProductCreateDTO createProductDto)
        {
            var productEntity = MapToCouponEntity(createProductDto);
            var sql= @"INSERT INTO Product (Name, Price,Description,CategoryName,ImageUrl,
            LastUpdateDate) 
                              VALUES (@Name, @Price, @Description,@CategoryName,@ImageUrl,GETDATE());
                              SELECT CAST(SCOPE_IDENTITY() as int)";
            return await _dapperRepository.ExecuteAsync(sql, productEntity);
        }
        public  async Task<bool> DeleteProductAsync(int productId)
        {
            var sql = "DELETE FROM Product WHERE ProductId = @ProductId";
            var rowsAffected = await _dapperRepository.ExecuteAsync(sql, new { ProductId = productId });
            return rowsAffected > 0;
        }
        public async Task<IEnumerable<Product>> GetAllProductAsync()
        {
            var sql = "SELECT * FROM Product";
            return await _dapperRepository.GetAllAsync<Product>(sql);
        }      
        public async Task<Product> GetProductByIdAsync(int productId)
        {
            var sql = "SELECT * FROM Product WHERE ProductId = @ProductId";
            return await _dapperRepository.GetAsync<Product>(sql, new { ProductId = productId });
        }
        public async Task<bool> UpdateProductAsync(ProductUpdateDTO updateProductDto)
        {
            var existingProduct = await GetProductByIdAsync(updateProductDto.ProductId);

            if (existingProduct == null)
            {
                return false; // Handle the case where the coupon with the given ID is not found
            }

            MapToCouponEntity(updateProductDto, existingProduct);
            var sql = @"UPDATE Product SET  Name = @Name, Price = @Price,
                        Description = @Description, CategoryName = @CategoryName, ImageUrl = @ImageUrl, ImageLocalPath = @ImageLocalPath,
                      LastUpdateDate = GETDATE()
                      WHERE ProductId = @ProductId";

             var rowsAffected = await _dapperRepository.ExecuteAsync(sql, existingProduct);

            return rowsAffected > 0;
        }
        public async Task<bool> UpdateProductAsync(int productId, ProductUpdateDTO updateProductDto)
        {
            var existingProduct = await GetProductByIdAsync(productId);

            if (existingProduct == null)
            {
                return false; // Handle the case where the coupon with the given ID is not found
            }

            MapToCouponEntity(updateProductDto, existingProduct);
            var sql = @"UPDATE Product SET  Name = @Name, Price = @Price,
                        Description = @Description, CategoryName = @CategoryName, ImageUrl = @ImageUrl
                      ,LastUpdateDate = GETDATE()
                      WHERE ProductId = @ProductId";
            var rowsAffected = await _dapperRepository.ExecuteAsync(sql, existingProduct);

            return rowsAffected > 0;
        }
        private static Product MapToCouponEntity(ProductCreateDTO createProductDto)
        {
            return new Product
            {
                Name = createProductDto.Name,
                Price = createProductDto.Price,
                Description = createProductDto.Description,
                CategoryName = createProductDto.CategoryName,
                ImageUrl = createProductDto.ImageUrl,

            };
        }
        private static void MapToCouponEntity(ProductUpdateDTO updateProductDto, Product existingCoupon)
        {

            existingCoupon.Name = updateProductDto.Name;
            existingCoupon.Price = updateProductDto.Price;
            existingCoupon.Description = updateProductDto.Description;
            existingCoupon.CategoryName = updateProductDto.CategoryName;
            existingCoupon.ImageUrl = updateProductDto.ImageUrl;
            

        }
  
    }
}
