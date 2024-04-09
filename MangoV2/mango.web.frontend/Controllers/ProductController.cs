using mango.web.frontend.Models.VM;
using mango.web.frontend.Models.WebDTO;
using mango.web.frontend.Models;
using mango.web.frontend.Services.Iservices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
namespace mango.web.frontend.Controllers
{
    public class ProductController : Controller
    {

        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {

            _productService = productService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ProductIndex()
        {
            try
            {
                Log.Information("Product Index Method called.");
                List<ProductViewModel> list = new List<ProductViewModel>();
                var response = await _productService.GetAllProductAsync<WebAPIResponse>();
                Log.Information("Response from GetAllProductAsync: {@Response}", response);

                if (response != null && response.IsSuccess)
                {
                    // Deserialize the JSON array into a List<CouponViewModel>
                    list = JsonConvert.DeserializeObject<List<ProductViewModel>>(response.Result.ToString()) ?? new List<ProductViewModel>();
                    return View(list);
                }
                else
                {
                    Log.Warning("Failed to retrieve products: {@Response}", response);
                    TempData["error"] = "Failed to retrieve products";
                    return View(new List<ProductViewModel>());

                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while accessing the Product Index page.");
                TempData["error"] = "An unexpected error occurred.";
                return View(new List<ProductViewModel>());
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(CreateProductDTO model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _productService.CreateProductAsync<WebAPIResponse>(model);
                    Log.Information("Response from CreateProductAsync: {@Response}", response);

                    if (response != null && response.IsSuccess)
                    {
                        TempData["success"] = "Product Created Successfully";
                        return RedirectToAction(nameof(ProductIndex));
                    }
                    else
                    {
                        Log.Warning("Failed to create product: {@Response}", response);
                        TempData["error"] = "Failed to create product";
                    }
                }
                else
                {
                    Log.Warning("Invalid model state when creating product.");
                }

                return View("CreateProduct", model);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred in CreateProduct action.");
                TempData["error"] = "An unexpected error occurred.";
                return View("CreateProduct", model);
            }
        }

        public async Task<IActionResult> CreateProduct()
        {
            return View();
        }

        public async Task<IActionResult> UpdateProduct(int productId)
        {
            try
            {
                Log.Information($"Update Product Method called for ProductId: {productId}");

                var response = await _productService.GetProductByIdAsync<WebAPIResponse>(productId);
                Log.Information("Response from GetProductByIdAsync: {@Response}", response);

                if (response != null && response.IsSuccess)
                {
                    var product = JsonConvert.DeserializeObject<ProductViewModel>(response.Result.ToString());

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
                else
                {
                    Log.Warning("Product not found for ProductId: {ProductId}", productId);
                    TempData["error"] = "Product not found.";
                    return RedirectToAction(nameof(ProductIndex));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred in UpdateProduct action for ProductId: {ProductId}", productId);
                TempData["error"] = "An unexpected error occurred.";
                return RedirectToAction(nameof(ProductIndex));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProduct(int productId, [FromForm] UpdateProductDTO model)
        {
            try
            {
                Log.Information($"Update Product method called for ProductId: {productId}");

                if (ModelState.IsValid)
                {
                    model.ProductId = productId;
                    var response = await _productService.UpdateProductAsync<WebAPIResponse>(model);
                    Log.Information("Response from UpdateProductAsync: {@Response}", response);

                    if (response != null && response.IsSuccess)
                    {
                        TempData["success"] = "Product Updated Successfully";
                        return RedirectToAction(nameof(ProductIndex));
                    }
                    else
                    {
                        Log.Warning("Failed to update product for ProductId: {ProductId}", productId);
                        TempData["error"] = "Failed to update product.";
                    }
                }
                else
                {
                    Log.Warning("Invalid model state when updating product for ProductId: {ProductId}", productId);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred in UpdateProduct action for ProductId: {ProductId}", productId);
                TempData["error"] = "An unexpected error occurred.";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            try
            {
                Log.Information($"Delete Product Method called for ProductId: {productId}");

                var response = await _productService.GetProductByIdAsync<WebAPIResponse>(productId);

                if (response != null && response.IsSuccess)
                {
                    var product = JsonConvert.DeserializeObject<ProductViewModel>(response.Result.ToString());

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
                else
                {
                    Log.Warning("Product not found for ProductId: {ProductId}", productId);
                    TempData["error"] = "Product not found.";
                    return RedirectToAction(nameof(ProductIndex));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred in DeleteProduct action for ProductId: {ProductId}", productId);
                TempData["error"] = "An unexpected error occurred.";
                return RedirectToAction(nameof(ProductIndex));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(DeleteProductDTO model)
        {
            try
            {
                Log.Information("Delete Product method called");

                if (ModelState.IsValid)
                {
                    var response = await _productService.DeleteProductAsync<WebAPIResponse>(model.ProductId);

                    if (response != null && response.IsSuccess)
                    {
                        TempData["success"] = "Product Deleted Successfully";
                        return RedirectToAction(nameof(ProductIndex));
                    }
                    else
                    {
                        Log.Warning("Failed to delete product for ProductId: {ProductId}", model.ProductId);
                        TempData["error"] = "Failed to delete product.";
                    }
                }
                else
                {
                    Log.Warning("Invalid model state when deleting product");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred in DeleteProduct action for ProductId: {ProductId}", model.ProductId);
                TempData["error"] = "An unexpected error occurred.";
                return View(model);
            }
        }
    }
    }
