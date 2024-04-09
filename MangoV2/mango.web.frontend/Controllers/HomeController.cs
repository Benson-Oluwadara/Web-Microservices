using mango.web.frontend.Models;
using mango.web.frontend.Models.VM;
using mango.web.frontend.Models.WebDTO;
using mango.web.frontend.Services.Iservices;
using Microsoft.AspNetCore.Mvc;
using IdentityModel;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;
using System.Diagnostics;
using Serilog;

namespace mango.web.frontend.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        public HomeController( IProductService productservice, ICartService cartService)
        {
            
            _productService = productservice;
            _cartService = cartService;

        }

        public async Task<IActionResult> Index()
        {
            try
            {
                List<ProductViewModel> list = new List<ProductViewModel>();
                var response = await _productService.GetAllProductAsync<WebAPIResponse>();
                Log.Information($"Response is: {JsonConvert.SerializeObject(response)}");
                //Console.WriteLine($"Response Content: {JsonConvert.SerializeObject(response)}");

                if (response != null && response.IsSuccess)
                {
                    // Deserialize the JSON array into a List<CouponViewModel>
                    list = JsonConvert.DeserializeObject<List<ProductViewModel>>(response.Result.ToString()) ?? new List<ProductViewModel>();
                    
                }
                return View(list);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while accessing the Home page.");
                throw; // You may handle the exception according to your application's error handling strategy
            }
        }
        //post
        //[HttpPost]
        //[ActionName("Details")]
        //public async Task<IActionResult> Details(ProductDTO productdto)
        //{
        //    CartDTO cartDTO = new CartDTO()
        //    {
        //        CartHeader = new CartHeaderDTO()
        //        {
        //            UserId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value
        //        }
        //    };
        //    CartDetailsDTO cartDetails = new CartDetailsDTO()
        //    {
        //        Count= productdto.Count,
        //        ProductId = productdto.ProductId,
        //    };
        //    List<CartDetailsDTO> cartDetailsDtos = new() { cartDetails };
        //    cartDTO.CartDetails = cartDetailsDtos;
        //    var response = await _cartService.UpsertCartAsync<WebAPIResponse>(cartDTO);
        //    if (response != null && response.IsSuccess)
        //    {
        //        TempData["success"] = "Item has been added to the Shopping Cart";
        //        return RedirectToAction(nameof(Index));
        //    }
        //    else
        //    {
        //        TempData["error"] = response?.Result;
        //    }

        //    return View(productdto);
        //}
        //public async Task<IActionResult> Details(int productID)
        //{
        //    ProductDTO product = new ProductDTO();
        //    var response = await _productService.GetProductByIdAsync<WebAPIResponse>(productID);
        //    _logger.LogInformation($"Response is: {JsonConvert.SerializeObject(response)}");
        //    Console.WriteLine($"Response Content: {JsonConvert.SerializeObject(response)}");

        //    if (response != null && response.IsSuccess)
        //    {
        //        // Deserialize the JSON object into a single ProductViewModel
        //        product = JsonConvert.DeserializeObject<ProductDTO>(response.Result.ToString());

        //        // Uncomment the following line if you need to inspect the deserialized data
        //        //Console.WriteLine($"ProductID: {product.ProductId}, Product Name: {product.Name}");
        //    }

        //    return View(product);
        //}


        [HttpPost]
        [ActionName("Details")]
        public async Task<IActionResult> Details(ProductDTO productDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Handle invalid model state (e.g., return to the view with validation errors)
                    return View(productDto);
                }

                CartDTO cartDto = new CartDTO()
                {
                    CartHeader = new CartHeaderDTO()
                    {
                        //UserId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value
                        UserId= User.Claims.Where(u => u.Type == JwtClaimTypes.Subject)?.FirstOrDefault()?.Value
                    }

                };
                
                CartDetailsDTO cartDetails = new CartDetailsDTO()
                {
                    Count = productDto.Count,
                    ProductId = productDto.ProductId,
                };
                List<CartDetailsDTO> cartDetailsDtos = new() { cartDetails };
                cartDto.CartDetails = cartDetailsDtos;

                Log.Information($"Constructed CartDTO: {JsonConvert.SerializeObject(cartDto)}");
                Log.Information($"ProductDTO Details: {JsonConvert.SerializeObject(productDto)}");

                var response = await _cartService.UpsertCartAsync<WebAPIResponse>(cartDto);
                Log.Information($"UpsertCart Response: {JsonConvert.SerializeObject(response)}");

                if (response != null && response.IsSuccess)
                {
                    Log.Information("Item has been added to the Shopping Cart");
                    TempData["success"] = "Item has been added to the Shopping Cart";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = response?.Result;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while processing cart request.");
                TempData["error"] = "An unexpected error occurred.";
            }

            return View(productDto);
        }

        public async Task<IActionResult> Details(int productId)
        {
            try
            {
                ProductDTO productDto = new ProductDTO();
                var response = await _productService.GetProductByIdAsync<WebAPIResponse>(productId);
                Log.Information($"Response is: {JsonConvert.SerializeObject(response)}");
                //Console.WriteLine($"Response Content: {JsonConvert.SerializeObject(response)}");

                if (response != null && response.IsSuccess)
                {
                    // Deserialize the JSON object into a single ProductViewModel
                    productDto = JsonConvert.DeserializeObject<ProductDTO>(response.Result.ToString());

                    // Uncomment the following line if you need to inspect the deserialized data
                    //Console.WriteLine($"ProductID: {productDto.ProductId}, Product Name: {productDto.Name}");
                }

                return View(productDto);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while processing product details request.");
                TempData["error"] = "An unexpected error occurred.";
                return View(new ProductDTO()); // or handle the error accordingly
            }
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}