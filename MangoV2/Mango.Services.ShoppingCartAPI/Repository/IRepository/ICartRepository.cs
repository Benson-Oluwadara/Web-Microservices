using Mango.Services.ShoppingCartAPI.Models.DTO;

namespace Mango.Services.ShoppingCartAPI.Repository.IRepository
{
    public interface ICartRepository
    {
        Task<CartDTO> GetCartByUserIdAsync(string userId);
        Task<bool> UpsertCartAsync(CartDTO cartDto);
        Task<bool> RemoveCartAsync(int cartDetailsId);
        Task<double> CalculateCouponDiscountAsync(string couponCode, double cartTotal);
        Task<bool> CreateOrUpdateCartDetailsAsync(CartDTO cartDto, int cartHeaderId);

    }
}
