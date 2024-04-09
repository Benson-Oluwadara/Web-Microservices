using AutoMapper;
using mango.messagebus;
using Mango.Services.ShoppingCartAPI.Database.IDapperRepositorys;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTO;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Mango.Services.ShoppingCartAPI.Service.Service;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Net;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        private readonly IMapper _mapper;
        private readonly IDapperRepository _dapperRepository;
        private readonly ICartService _cartService;
        private readonly IMessageBus _messageBus;
        private IConfiguration _configuration;

        
        public CartAPIController(ICartService cartService,IProductService productService, IDapperRepository dapperRepository, ICouponService couponService, IMapper mapper, IMessageBus messageBus, IConfiguration configuration)/*IProductService productService, ICouponService couponService,*//* IMapper mapper IDapperRepository dapperRepository*/
        {
            _productService = productService;
            _couponService = couponService;
            //_mapper = mapper;
            // _dapperRepository = dapperRepository;
            _cartService = cartService;
            _dapperRepository = dapperRepository;
            _mapper = mapper;
            _messageBus = messageBus;
            _configuration = configuration;
            //_couponService = couponService;
        }
        //get cart by user id
        //[HttpGet("GetCart/{userId}")]
        //public async Task<IActionResult> GetCart(string userId)
        //{
        //    try
        //    {
        //        // Retrieve CartHeader using Dapper
        //        var cartHeader = await _dapperRepository.GetAsync<CartHeader>(
        //            "SELECT TOP 1 [CartHeaderId], [UserId], [CouponCode] " +
        //            "FROM [MangoDatabase].[dbo].[CartHeaders] " +
        //            "WHERE [UserId] = @UserId;",
        //            new { UserId = userId });

        //        if (cartHeader == null)
        //        {
        //            return NotFound(new APIResponse(HttpStatusCode.NotFound, false, null, new List<string> { "Cart not found." }));
        //        }

        //        Console.WriteLine($"CartHeaderId: {cartHeader.CartHeaderId}, UserId: {cartHeader.UserId}, CouponCode: {cartHeader.CouponCode}");

        //        // Retrieve CartDetails using Dapper
        //        var cartDetails = await _dapperRepository.GetAllAsync<CartDetailsDTO>(
        //            "SELECT * FROM CartDetails WHERE CartHeaderId = @CartHeaderId",
        //            new { CartHeaderId = cartHeader.CartHeaderId });

        //        // Fetch additional information about the product for each cart detail
        //        foreach (var item in cartDetails)
        //        {
        //            Console.WriteLine($"CartDetailsId: {item.CartDetailsId}, CartHeaderId: {item.CartHeaderId}, ProductId: {item.ProductId}, Count: {item.Count}");

        //            // Retrieve product details using Dapper
        //            var product = await _dapperRepository.GetAsync<ProductDTO>(
        //                "SELECT * FROM [MangoDatabase].[dbo].[Product] WHERE ProductId = @ProductId",
        //                new { ProductId = item.ProductId });

        //            if (product != null)
        //            {
        //                // Calculate total price for the product
        //                double totalPrice = item.Count * product.Price;

        //                Console.WriteLine($"Product Details: ProductId: {product.ProductId}, Name: {product.Name}, Price: {product.Price}, Description: {product.Description}, Category: {product.CategoryName}");
        //                Console.WriteLine($"Total Price for {item.Count} {product.Name}(s): {totalPrice}");

        //                // Assign product details to CartDetails
        //                item.Product = product;
        //            }
        //            else
        //            {
        //                Console.WriteLine($"Product not found for ProductId: {item.ProductId}");
        //            }
        //        }
        //        // Create CartDTO structure
        //        var cartDTO = new CartDTO
        //        {
        //            CartHeader = _mapper.Map<CartHeaderDTO>(cartHeader),
        //            CartDetails = _mapper.Map<IEnumerable<CartDetailsDTO>>(cartDetails)
        //        };

        //        return Ok(new APIResponse(HttpStatusCode.OK, true, cartDTO, null));


        //        //return Ok(new { CartHeader = cartHeader, CartDetails = cartDetails });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new APIResponse(HttpStatusCode.InternalServerError, false, null, new List<string> { ex.Message }));
        //    }
        //}



        //[HttpPost("AddToCart")]

        [HttpGet("GetCart/{userId}")]
        public async Task<IActionResult> GetCart(string userId)
        {
            try
            {
                // Retrieve CartHeader using Dapper
                var cartHeader = await _dapperRepository.GetAsync<CartHeader>(
                    "SELECT TOP 1 [CartHeaderId], [UserId], [CouponCode] " +
                    "FROM [dbo].[CartHeaders] " +
                    "WHERE [UserId] = @UserId;",
                    new { UserId = userId });

                if (cartHeader == null)
                {
                    Log.Warning("Cart not found for user with ID {UserId}.", userId);
                    return NotFound(new APIResponse(HttpStatusCode.NotFound, false, null, new List<string> { "Cart not found." }));
                }

                Log.Information("CartHeader retrieved successfully. CartHeaderId: {CartHeaderId}, UserId: {UserId}, CouponCode: {CouponCode}", cartHeader.CartHeaderId, cartHeader.UserId, cartHeader.CouponCode);

                // Retrieve CartDetails using Dapper
                var cartDetails = await _dapperRepository.GetAllAsync<CartDetailsDTO>(
                    "SELECT * FROM CartDetails WHERE CartHeaderId = @CartHeaderId",
                    new { CartHeaderId = cartHeader.CartHeaderId });

                // Fetch additional information about the product for each cart detail
                foreach (var item in cartDetails)
                {
                    Log.Information("Retrieving details for CartDetailsId: {CartDetailsId}, CartHeaderId: {CartHeaderId}, ProductId: {ProductId}, Count: {Count}", item.CartDetailsId, item.CartHeaderId, item.ProductId, item.Count);

                    // Retrieve product details using Dapper
                    var product = await _dapperRepository.GetAsync<ProductDTO>(
                        "SELECT * FROM [dbo].[Product] WHERE ProductId = @ProductId",
                        new { ProductId = item.ProductId });

                    if (product != null)
                    {
                        // Calculate total price for the product
                        double totalPrice = item.Count * product.Price;

                        Log.Information("Product Details: ProductId: {ProductId}, Name: {ProductName}, Price: {ProductPrice}, Description: {ProductDescription}, Category: {ProductCategory}", product.ProductId, product.Name, product.Price, product.Description, product.CategoryName);
                        Log.Information("Total Price for {Count} {ProductName}(s): {TotalPrice}", item.Count, product.Name, totalPrice);

                        // Assign product details to CartDetails
                        item.Product = product;
                    }
                    else
                    {
                        Log.Warning("Product not found for ProductId: {ProductId}", item.ProductId);
                    }
                }

                // Create CartDTO structure
                var cartDTO = new CartDTO
                {
                    CartHeader = _mapper.Map<CartHeaderDTO>(cartHeader),
                    CartDetails = _mapper.Map<IEnumerable<CartDetailsDTO>>(cartDetails)
                };

                return Ok(new APIResponse(HttpStatusCode.OK, true, cartDTO, null));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while retrieving the cart for user with ID {UserId}.", userId);
                return StatusCode(500, new APIResponse(HttpStatusCode.InternalServerError, false, null, new List<string> { "An error occurred while processing your request." }));
            }
        }

        //[HttpPost("UpsertCart")]
        //public async Task<IActionResult> CartUpsert(CartDTO cartDto)
        //{
        //    try
        //    {
        //        var response = await _cartService.CartUpsert(cartDto);

        //        if (response.IsSuccess)
        //        {
        //            // Log or print out successful cart details
        //            Console.WriteLine("Cart Upsert Successful!");
        //            Console.WriteLine("Cart Details:");
        //            //foreach (var cartDetail in cartDto.CartDetails)
        //            //{
        //            //    Console.WriteLine($"Product ID: {cartDetail.Product.ProductId}, Count: {cartDetail.Count}");
        //            //}

        //            return StatusCode(200, response);
        //        }
        //        else
        //        {
        //            return StatusCode(500, "Failed to upsert the cart.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Exception in UpsertCart: {ex}");

        //        // Log cart details if available
        //        if (cartDto != null)
        //        {
        //            Console.WriteLine("Cart Details:");
        //            foreach (var cartDetail in cartDto.CartDetails)
        //            {
        //                Console.WriteLine($"Product ID: {cartDetail.Product.ProductId}, Count: {cartDetail.Count}");
        //            }
        //        }

        //        var responseDto = new APIResponse(HttpStatusCode.InternalServerError, false, null, new List<string> { "An error occurred while applying the coupon. Check the inner exceptions for details." });
        //        return BadRequest(responseDto);
        //    }
        //}
        [HttpPost("UpsertCart")]
        public async Task<IActionResult> CartUpsert(CartDTO cartDto)
        {
            try
            {
                var response = await _cartService.CartUpsert(cartDto);

                if (response.IsSuccess)
                {
                    // Log or print out successful cart details
                    Log.Information("Cart Upsert Successful!");
                    Log.Information("Cart Details:");
                    foreach (var cartDetail in cartDto.CartDetails)
                    {
                        Log.Information("Product ID: {ProductId}, Count: {Count}", cartDetail.Product.ProductId, cartDetail.Count);
                    }

                    return StatusCode(200, response);
                }
                else
                {
                    Log.Error("Failed to upsert the cart.");
                    return StatusCode(500, "Failed to upsert the cart.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception in UpsertCart");

                // Log cart details if available
                if (cartDto != null)
                {
                    Log.Information("Cart Details:");
                    foreach (var cartDetail in cartDto.CartDetails)
                    {
                        Log.Information("Product ID: {ProductId}, Count: {Count}", cartDetail.Product.ProductId, cartDetail.Count);
                    }
                }

                var responseDto = new APIResponse(HttpStatusCode.InternalServerError, false, null, new List<string> { "An error occurred while applying the coupon. Check the inner exceptions for details." });
                return BadRequest(responseDto);
            }
        }


        //[HttpPost("ApplyCoupon")]
        //public async Task<IActionResult> ApplyCoupon([FromBody] CartDTO cartDto)
        //{
        //    try
        //    {
        //        var response = await _cartService.ApplyCoupon(cartDto);
        //        return StatusCode(200, response);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Exception in ApplyCoupon: {ex}");

        //        var responseDto = new APIResponse(HttpStatusCode.InternalServerError, false, null, new List<string> { "An error occurred while applying the coupon. Check the inner exceptions for details." });
        //        return BadRequest(responseDto);
        //    }
        //}

        [HttpPost("ApplyCoupon")]
        public async Task<IActionResult> ApplyCoupon([FromBody] CartDTO cartDto)
        {
            try
            {
                // Apply coupon logic
                var response = await _cartService.ApplyCoupon(cartDto);

                // Log informational message
                Log.Information("Coupon applied successfully.");

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                // Log error message
                Log.Error(ex, "Exception in ApplyCoupon");

                var responseDto = new APIResponse(HttpStatusCode.InternalServerError, false, null, new List<string> { "An error occurred while applying the coupon. Check the inner exceptions for details." });
                return BadRequest(responseDto);
            }
        }


        //[HttpPost("EmailCartRequest")]
        //public async Task<IActionResult> EmailCartRequest([FromBody] CartDTO cartDto)
        //{
        //    try
        //    {
        //        await _messageBus.PublicMessage(cartDto, _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue"));
        //        return StatusCode(200);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Exception in Email Request: {ex}");

        //        var responseDto = new APIResponse(HttpStatusCode.InternalServerError, false, null, new List<string> { "An error occurred while applying the coupon. Check the inner exceptions for details." });
        //        return BadRequest(responseDto);
        //    }
        //}

        [HttpPost("EmailCartRequest")]
        public async Task<IActionResult> EmailCartRequest([FromBody] CartDTO cartDto)
        {
            try
            {
                // Send cart via email logic
                await _messageBus.PublicMessage(cartDto, _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue"));

                // Log informational message
                Log.Information("Cart sent via email successfully.");

                return StatusCode(200);
            }
            catch (Exception ex)
            {
                // Log error message
                Log.Error(ex, "Exception in Email Request");

                var responseDto = new APIResponse(HttpStatusCode.InternalServerError, false, null, new List<string> { "An error occurred while sending the cart via email. Check the inner exceptions for details." });
                return BadRequest(responseDto);
            }
        }

        //[HttpDelete("RemoveCart/{cartDetailsId}")]
        //public async Task<IActionResult> RemoveCart(int cartDetailsId)
        //{
        //    var response = await _cartService.RemoveCart(cartDetailsId);
        //    return Ok(response);
        //}

        [HttpDelete("RemoveCart/{cartDetailsId}")]
        public async Task<IActionResult> RemoveCart(int cartDetailsId)
        {
            try
            {
                // Remove cart item logic
                var response = await _cartService.RemoveCart(cartDetailsId);

                // Log informational message
                Log.Information("Cart item removed successfully.");

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log error message
                Log.Error(ex, "Exception in RemoveCart");

                var responseDto = new APIResponse(HttpStatusCode.InternalServerError, false, null, new List<string> { "An error occurred while removing the cart item. Check the inner exceptions for details." });
                return BadRequest(responseDto);
            }
        }
    }
}
