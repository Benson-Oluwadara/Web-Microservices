using AutoMapper;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTO;
using Mango.Services.ShoppingCartAPI.Repository.IRepository;
using Mango.Services.ShoppingCartAPI.Service.IService;
using System.Net;

namespace Mango.Services.ShoppingCartAPI.Service.Service
{
    public class CartService: ICartService
    {
        private readonly ICartRepository _cartRepository;
        //private readonly IProductService _productService;
        //private readonly ICouponService _couponService;
        //private readonly IMapper _mapper;

        public CartService(ICartRepository cartRepository/*, IProductService productService, ICouponService couponService, IMapper mapper*/)
        {
            _cartRepository = cartRepository;
            //_productService = productService;
            //_couponService = couponService;
            //_mapper = mapper;
        }

        public async Task<APIResponse> CartUpsert(CartDTO cartDto)
        {
            try
            {
                var result = await _cartRepository.UpsertCartAsync(cartDto);
                return new APIResponse { IsSuccess = result, Result = cartDto };
            }
            catch (AggregateException ex)
            {
                foreach (var innerException in ex.InnerExceptions)
                {
                    Console.WriteLine($"Inner Exception: {innerException.Message}");
                }
                return new APIResponse { IsSuccess = false, ErrorMessages = new List<string> { ex.Message } };
            }
            catch (Exception ex)
            {
                return new APIResponse { IsSuccess = false, ErrorMessages = new List<string> { ex.Message } };
            }
        }


        //public Task<CartDTO> GetCart(string userId)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<APIResponse> RemoveCart(int cartDetailsId)
        {
            try
            {
                var result = await _cartRepository.RemoveCartAsync(cartDetailsId);
                return new APIResponse { IsSuccess = result, Result = true };
            }
            catch (AggregateException ex)
            {
                foreach (var innerException in ex.InnerExceptions)
                {
                    Console.WriteLine($"Inner Exception: {innerException.Message}");
                }
                return new APIResponse { IsSuccess = false, ErrorMessages = new List<string> { ex.Message } };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during service registration: {ex}");
                return new APIResponse { IsSuccess = false, ErrorMessages = new List<string> { ex.Message } };
            }
        }


        public async Task<CartDTO> ApplyCoupon(CartDTO cartDto)
        {
            //try
            //{
            //    var cartFromRepo = await _cartRepository.GetCartByUserIdAsync(cartDto.CartHeader.UserId);

            //    if (cartFromRepo == null)
            //    {
            //        return new APIResponse(HttpStatusCode.NotFound, false, null, new List<string> { "Cart not found." });
            //    }

            //    cartFromRepo.CartHeader.CouponCode = cartDto.CartHeader.CouponCode;
            //    var result = await _cartRepository.UpsertCartAsync(cartFromRepo);

            //    if (result)
            //    {
            //        return new APIResponse(HttpStatusCode.OK, true, true);
            //    }

            //    return new APIResponse(HttpStatusCode.InternalServerError, false, null, new List<string> { "An error occurred while applying the coupon. Check the inner exceptions for details." });
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Exception in ApplyCoupon: {ex}");

            //    return new APIResponse(HttpStatusCode.InternalServerError, false, null, new List<string> { "An error occurred while applying the coupon. Check the inner exceptions for details." });
            //}


            if (cartDto == null)
            {
                // Handle invalid input, maybe return BadRequest or throw an exception
                throw new ArgumentNullException(nameof(cartDto), "CartDTO cannot be null");
            }
            // Check if the cart has a valid coupon code
            if (!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
            {
                // Assuming you have a coupon service to validate and apply coupons
                var discountAmount = await _cartRepository.CalculateCouponDiscountAsync(cartDto.CartHeader.CouponCode, cartDto.CartHeader.CartTotal);

                if (discountAmount > 0)
                {
                    // Apply the coupon discount to the cart total
                    cartDto.CartHeader.Discount = discountAmount;
                    // Update the cart total after applying the discount
                    cartDto.CartHeader.CartTotal -= cartDto.CartHeader.Discount;

                    // Perform any other necessary updates or validations

                    return cartDto; // Return the updated cart
                }
                else
                {
                    // Handle the case where the coupon is not valid
                    // You might want to return a specific response or throw an exception
                    throw new InvalidOperationException("Invalid coupon code");
                }
            }

            // If no coupon code is provided, return the original cart
            return cartDto;
        }
        private double CalculateCouponDiscount(double cartTotal)
        {
            // Add your logic to calculate the discount based on the cart total
            // This can include percentage-based discounts, fixed amount discounts, etc.

            // For example, applying a 10% discount
            return cartTotal * 0.1;
        }
    }
}
