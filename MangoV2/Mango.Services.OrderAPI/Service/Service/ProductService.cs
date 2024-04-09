using Mango.Services.OrderAPI.Models.DTO;
using Mango.Services.OrderAPI.Service.IService;
using Newtonsoft.Json;
using System.Net.Http;

namespace Mango.Services.OrderAPI.Service.Service
{
    public class ProductService: IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductService(IHttpClientFactory clientFactory)
        {
            _httpClientFactory = clientFactory;
        }
        public async Task<IEnumerable<ProductDTO>> GetProducts()
        {
            var client = _httpClientFactory.CreateClient("ProductAPI");
            var response = await client.GetAsync($"/api/products");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<ProductDTO>>(content);
            }
            return new List<ProductDTO>();
        }
    }
}
