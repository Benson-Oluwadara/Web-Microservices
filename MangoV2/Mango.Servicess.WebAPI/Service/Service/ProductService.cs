using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.DTO;
using Mango.Services.ProductAPI.Repository.IRepository;
using Mango.Services.ProductAPI.Service.IService;
using Microsoft.AspNetCore.Http;

namespace Mango.Services.ProductAPI.Service.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<ProductDTO>> GetAllProductAsync()
        {
            var products = await _productRepository.GetAllProductAsync();
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }
        public async Task<ProductDTO> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            return _mapper.Map<ProductDTO>(product);
        }
        public async Task<ProductDTO> CreateProductAsync(ProductCreateDTO createProductDto)
        {
            var productEntity = _mapper.Map<Product>(createProductDto);
            await _productRepository.CreateProductAsync(createProductDto);
            return _mapper.Map<ProductDTO>(productEntity);
        }
        public async Task<bool> UpdateProductAsync(ProductUpdateDTO updateProductDto)
        {  
            return await _productRepository.UpdateProductAsync(updateProductDto);
        }
        public async Task<bool> DeleteProductAsync(int id)
        {
            return await _productRepository.DeleteProductAsync(id);
        }
        public async Task<bool> UpdateProductAsync(int id, ProductUpdateDTO updateProductDto)
        {
            
            return await _productRepository.UpdateProductAsync(id, updateProductDto);
        }
    }
}
