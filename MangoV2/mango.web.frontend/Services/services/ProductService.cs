using mango.web.frontend.Models;
using mango.web.frontend.Models.WebDTO;
using mango.web.frontend.Services.Iservices;
using mango.web.frontend.Utility;
using static System.Net.WebRequestMethods;
using Serilog;
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
            //Log the method function
            Log.Information("CreateProductAsync method called");
            //Log in this method from which client the request is coming
            Log.Information($"Request from ProductAPI to {SD.ProductAPIBase + "/api/products"}");
            return this.SendAsync<T>(new WebAPIRequest()
            {
                apiType = SD.ApiType.POST,
                Data = createproductDTO,
                Url = SD.ProductAPIBase + "/api/products"
            }, "ProductAPI");
        }

        

        public Task<T> DeleteProductAsync<T>(int productId)
        {
            //Log the method function
            Log.Information("DeleteProductAsync method called");
            //Log in this method from which client the request is coming
            Log.Information($"Request from ProductAPI to {SD.ProductAPIBase + "/api/products/" + productId}");
            return this.SendAsync<T>(new WebAPIRequest()
            {
                apiType = SD.ApiType.DELETE,
                Url = SD.ProductAPIBase + "/api/products/" + productId
            }, "ProductAPI");
        }

      
        public Task<T> GetAllProductAsync<T>()
        {
            //Log the method function
            Log.Information("GetAllProductAsync method called");
            //Log in this method from which client the request is coming
            Log.Information($"Request from ProductAPI to {SD.ProductAPIBase + "/api/products"}");
            return this.SendAsync<T>(new WebAPIRequest()
            {
                apiType = SD.ApiType.GET,
                Url = SD.ProductAPIBase + "/api/products"
            }, "ProductAPI");

            
        }
       
        public Task<T> GetProductByIdAsync<T>(int productId)
        {
            //Log the method function
            Log.Information("GetProductByIdAsync method called");
                //Log in this method from which client the request is coming
                Log.Information($"Request from ProductAPI to {SD.ProductAPIBase + "/api/products/" + productId}");
            return this.SendAsync<T>(new WebAPIRequest()
            {
                apiType = SD.ApiType.GET,
                Url = SD.ProductAPIBase + "/api/products/" + productId
            }, "ProductAPI");
        }
        public Task<T> UpdateProductAsync<T>(UpdateProductDTO updateproductDTO)
        {
            //Log the method function
            Log.Information("UpdateProductAsync method called");
            //Log in this method from which client the request is coming
            Log.Information($"Request from ProductAPI to {SD.ProductAPIBase + "/api/products/" + updateproductDTO.ProductId}");
            return this.SendAsync<T>(new WebAPIRequest()
            {
                apiType = SD.ApiType.PUT,
                Data = updateproductDTO,
                Url = SD.ProductAPIBase + "/api/products/" + updateproductDTO.ProductId
            }, "ProductAPI");
        }

        
    }
}
 
