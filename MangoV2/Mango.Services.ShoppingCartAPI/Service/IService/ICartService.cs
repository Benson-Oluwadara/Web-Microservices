using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTO;

namespace Mango.Services.ShoppingCartAPI.Service.IService
{
    public interface ICartService
    {
        //public Task<CartDTO> GetCart(string userId);
        public  Task<APIResponse> CartUpsert(CartDTO cartDto);
        public Task<APIResponse> RemoveCart(int cartDetailsId);

        // New method for applying a coupon
        Task<CartDTO> ApplyCoupon(CartDTO cartDto);
    }
}
