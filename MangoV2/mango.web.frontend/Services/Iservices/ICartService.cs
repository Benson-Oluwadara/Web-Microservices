using mango.web.frontend.Models.WebDTO;

namespace mango.web.frontend.Services.Iservices
{
    public interface ICartService
    {
        Task<T> GetCartByUserIdAsnyc<T>(string userId);
        Task<T> UpsertCartAsync<T>(CartDTO cartDto);
        Task<T> RemoveFromCartAsync<T>(int cartDetailsId);
        Task<T> ApplyCouponAsync<T>(CartDTO cartDto);
        Task<T> EmailCart<T>(CartDTO cartDto);
    }
}
