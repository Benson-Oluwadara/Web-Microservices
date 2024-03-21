using AutoMapper;
using Mango.Services.Shopping_CartAPI.Database.IDapperRepositorys;
using Mango.Services.Shopping_CartAPI.Models;
using Mango.Services.Shopping_CartAPI.Models.DTO;
using Mango.Services.Shopping_CartAPI.Service.IService;
using Mango.Services.Shopping_CartAPI.Service.Service;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Mango.Services.Shopping_CartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        private readonly IMapper _mapper;
        private readonly IDapperRepository _dapperRepository;
        private readonly ICartService _cartService;
        public CartAPIController(ICartService cartService,IProductService productService, ICouponService couponService, IMapper mapper, IDapperRepository dapperRepository)
        {
            _productService = productService;
            _couponService = couponService;
            _mapper = mapper;
            _dapperRepository = dapperRepository;
            _cartService = cartService;
        }
        //get cart by user id
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(string userId)
        {
            try
            {
                var cart = await _dapperRepository.GetAsync<CartDTO>("SELECT TOP 1 * FROM CartHeaders WHERE UserId = @UserId", new { UserId = userId }).ConfigureAwait(false);

                if (cart == null)
                {
                    return NotFound(new APIResponse(HttpStatusCode.NotFound, false, null, new List<string> { "Cart not found." }));
                }

                var cartDetails = await _dapperRepository.GetAllAsync<CartDetailsDTO>("SELECT * FROM CartDetails WHERE CartHeaderId = @CartHeaderId", new { CartHeaderId = cart.CartHeader.CartHeaderId });

                var productDtos = await _productService.GetProducts();

                foreach (var item in cartDetails)
                {
                    item.Product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
                    cart.CartHeader.CartTotal += (item.Count * item.Product.Price);
                }

                if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                {
                    var coupon = await _couponService.GetCoupon(cart.CartHeader.CouponCode);
                    if (coupon != null && cart.CartHeader.CartTotal > coupon.MinAmount)
                    {
                        cart.CartHeader.CartTotal -= coupon.DiscountAmount;
                        cart.CartHeader.Discount = coupon.DiscountAmount;
                    }
                }

                var responseDto = new APIResponse(HttpStatusCode.OK, true, cart);
                return Ok(responseDto);
            }
            catch (AggregateException ex)
            {
                foreach (var innerException in ex.InnerExceptions)
                {
                    Console.WriteLine($"Inner Exception: {innerException.Message}");
                }
                //get the return type of the method
                return BadRequest(new APIResponse(HttpStatusCode.InternalServerError, false, null, new List<string> { "An error occurred. Check the inner exceptions for details." }));
                //return new APIResponse(HttpStatusCode.InternalServerError, false, null, new List<string> { "An error occurred. Check the inner exceptions for details." });
            }
            catch (Exception ex)
            {
                var responseDto = new APIResponse(HttpStatusCode.InternalServerError, false, null, new List<string> { ex.Message });
                return BadRequest(responseDto);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CartUpsert(CartDTO cartDto)
        {
            var response = await _cartService.CartUpsert(cartDto);
            return Ok(response);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveCart([FromBody] int cartDetailsId)
        {
            var response = await _cartService.RemoveCart(cartDetailsId);
            return Ok(response);
        }
    }
}
