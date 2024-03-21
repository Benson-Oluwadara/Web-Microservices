using mango.web.frontend.Models;
using mango.web.frontend.Models.WebDTO;
using mango.web.frontend.Services.Iservices;
using mango.web.frontend.Utility;
using static System.Net.WebRequestMethods;

namespace mango.web.frontend.Services.services
{
    public class ProductService : BaseService, IProductService
    {
        private readonly IHttpClientFactory _clientFactory;
        public ProductService(IHttpClientFactory clientFactory) : base(clientFactory)
        {
            _clientFactory = clientFactory;
        }
        public Task<T> CreateProductAsync<T>(CreateProductDTO createproductDTO)
        {
            return this.SendAsync<T>(new WebAPIRequest()
            {
                apiType = SD.ApiType.POST,
                Data = createproductDTO,
                Url = SD.ProductAPIBase + "/api/products"
            }, "ProductAPI");
        }

        

        public Task<T> DeleteProductAsync<T>(int productId)
        {
            
            return this.SendAsync<T>(new WebAPIRequest()
            {
                apiType = SD.ApiType.DELETE,
                Url = "https://localhost:6003" + "/api/products/" + productId
            }, "ProductAPI");
        }

      
        public Task<T> GetAllProductAsync<T>()
        {
            Console.WriteLine("URL is: " + SD.ProductAPIBase + "/api/products");
            return this.SendAsync<T>(new WebAPIRequest()
            {
                apiType = SD.ApiType.GET,
                Url = SD.ProductAPIBase + "/api/products"
            }, "ProductAPI");

            
        }
       
        public Task<T> GetProductByIdAsync<T>(int productId)
        {
            return this.SendAsync<T>(new WebAPIRequest()
            {
                apiType = SD.ApiType.GET,
                Url = SD.ProductAPIBase + "/api/products/" + productId
            }, "ProductAPI");
        }
        public Task<T> UpdateProductAsync<T>(UpdateProductDTO updateproductDTO)
        {
            return this.SendAsync<T>(new WebAPIRequest()
            {
                apiType = SD.ApiType.PUT,
                Data = updateproductDTO,
                Url = SD.ProductAPIBase + "/api/products/" + updateproductDTO.ProductId
            }, "ProductAPI");
        }

        
    }
}
 
