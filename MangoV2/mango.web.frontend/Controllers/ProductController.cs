using mango.web.frontend.Models.VM;
using mango.web.frontend.Models.WebDTO;
using mango.web.frontend.Models;
using mango.web.frontend.Services.Iservices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace mango.web.frontend.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _productService;
        public ProductController(ILogger<ProductController> logger,IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ProductIndex()
        {
            Console.WriteLine("Product Index Method!!!!!!!!!!!!!!!!!!!!!!!!!!\n!!!!!!!!!!!!!!!!!!!!!");

            List<ProductViewModel> list = new List<ProductViewModel>();
            var response = await _productService.GetAllProductAsync<WebAPIResponse>();
            _logger.LogInformation($"Response is: {JsonConvert.SerializeObject(response)}");
            Console.WriteLine($"Response Content: {JsonConvert.SerializeObject(response)}");

            if (response != null && response.IsSuccess)
            {
                // Deserialize the JSON array into a List<CouponViewModel>
                list = JsonConvert.DeserializeObject<List<ProductViewModel>>(response.Result.ToString()) ?? new List<ProductViewModel>();
                foreach (var product in list)
                {
                    Console.WriteLine($"ProductID: {product.ProductId}, Product Name: {product.Name}");
                }
            }
            return View(list);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(CreateProductDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.CreateProductAsync<WebAPIResponse>(model);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product Created Successfully";
                    return RedirectToAction(nameof(ProductIndex));
                }
                TempData["error"] = "Error Encountered";
            }

            // Check if the view name matches the actual view file name
            return View("CreateProduct", model);
        }



        public async Task<IActionResult> CreateProduct()
        {
            return View();
        }

      
        public async Task<IActionResult> UpdateProduct(int productId)
        {
            Console.WriteLine($"Update Product Method for ProductId: {productId} !!!!!!!!!!!!!!!!!!!!!!!!!!");

            var response = await _productService.GetProductByIdAsync<WebAPIResponse>(productId);
            _logger.LogInformation($"Response is: {JsonConvert.SerializeObject(response)}");

            if (response != null && response.IsSuccess)
            {
                var product = JsonConvert.DeserializeObject<ProductViewModel>(response.Result.ToString());

                // Assuming you have an UpdateProductDTO model to bind to the form
                var updateProductDto = new UpdateProductDTO
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Price = product.Price,
                    Description = product.Description,
                    CategoryName = product.CategoryName,
                    ImageUrl = product.ImageUrl
                    

                };

                return View(updateProductDto);
            }
            TempData["error"] = "Product not found.";
            return RedirectToAction(nameof(ProductIndex));
        }

        [HttpPost]
        //[Consumes("multipart/form-data")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProduct(int productId, [FromForm] UpdateProductDTO model)
        {
            Console.WriteLine("Update Product!!!!!!!!!!!!!!!!!!!!!!!!!!\n!!!!!!!!!!!!!!!!!!!!!");

            if (ModelState.IsValid)
            {
                // Handle image upload if a new image is provided
                

                // Set the ProductId in the model
                model.ProductId = productId;

                // Call the service to update the product
                var response = await _productService.UpdateProductAsync<WebAPIResponse>(model);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product Updated Successfully";
                    return RedirectToAction(nameof(ProductIndex));
                }

                TempData["error"] = "Error Encountered";
            }

            return View(model);
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> UpdateProduct(UpdateProductDTO model)
        //{
        //    Console.WriteLine("Update Product!!!!!!!!!!!!!!!!!!!!!!!!!!\n!!!!!!!!!!!!!!!!!!!!!");

        //    if (ModelState.IsValid)
        //    {
        //        var response = await _productService.UpdateProductAsync<WebAPIResponse>(model);
        //        if (response != null && response.IsSuccess)
        //        {
        //            TempData["success"] = "Product Updated Successfully";
        //            return RedirectToAction(nameof(ProductIndex));
        //        }

        //        TempData["error"] = "Error Encountered";
        //    }

        //    return View(model);
        //}

        

        


        [HttpGet]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            Console.WriteLine($"Delete Product Method for ProductId: {productId} !!!!!!!!!!!!!!!!!!!!!!!!!!");

            var response = await _productService.GetProductByIdAsync<WebAPIResponse>(productId);

            if (response != null && response.IsSuccess)
            {
                var product = JsonConvert.DeserializeObject<ProductViewModel>(response.Result.ToString());

                // Assuming you have a DeleteProductDTO model to bind to the form
                var deleteProductDto = new DeleteProductDTO
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Price = product.Price,
                    Description = product.Description,
                    CategoryName = product.CategoryName,
                    ImageUrl = product.ImageUrl
                    
                };

                return View(deleteProductDto);
            }

            TempData["error"] = "Product not found.";
            return RedirectToAction(nameof(ProductIndex));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(DeleteProductDTO model)
        {
            Console.WriteLine("Delete Product!!!!!!!!!!!!!!!!!!!!!!!!!!\n!!!!!!!!!!!!!!!!!!!!!");

            if (ModelState.IsValid)
            {
                var response = await _productService.DeleteProductAsync<WebAPIResponse>(model.ProductId);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product Deleted Successfully";
                    return RedirectToAction(nameof(ProductIndex));
                }

                TempData["error"] = "Error Encountered";
            }

            return View(model);
        }
    }
}
