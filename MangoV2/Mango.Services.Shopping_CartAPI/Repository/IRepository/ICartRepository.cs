using Mango.Services.Shopping_CartAPI.Models.DTO;

namespace Mango.Services.Shopping_CartAPI.Repository.IRepository
{
    public interface ICartRepository
    {
        Task<CartDTO> GetCartByUserIdAsync(string userId);
        Task<bool> UpsertCartAsync(CartDTO cartDto);
        Task<bool> RemoveCartAsync(int cartDetailsId);
    }
}
