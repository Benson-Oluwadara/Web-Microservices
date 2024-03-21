using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Mango.Services.Shopping_CartAPI.Models;
using Mango.Services.Shopping_CartAPI.Models.DTO;
using Mango.Services.Shopping_CartAPI.Repository.IRepository;
using Mango.Services.Shopping_CartAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Mango.Services.Shopping_CartAPI.Service.Service
{
    public class ProductService : IProductService
    {
       
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<ProductDTO>> GetProducts()
        {
            var client = _httpClientFactory.CreateClient("ProductAPI");
            var response = await client.GetAsync("/api/products");
            var content = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<IEnumerable<ProductDTO>>(content);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error calling the ProductAPI");
            }
            if (response.IsSuccessStatusCode)
            {
                return resp;    
                
            }

            return new List<ProductDTO>();

        }
    }
}
