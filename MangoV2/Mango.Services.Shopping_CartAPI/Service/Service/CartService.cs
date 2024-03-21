using AutoMapper;
using Mango.Services.Shopping_CartAPI.Models;
using Mango.Services.Shopping_CartAPI.Models.DTO;
using Mango.Services.Shopping_CartAPI.Repository.IRepository;
using Mango.Services.Shopping_CartAPI.Service.IService;

namespace Mango.Services.Shopping_CartAPI.Service.Service
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
                // Your existing logic
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
                // Your existing logic
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
    }
}
