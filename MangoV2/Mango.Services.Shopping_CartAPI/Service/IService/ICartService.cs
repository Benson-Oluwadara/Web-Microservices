using Mango.Services.Shopping_CartAPI.Models;
using Mango.Services.Shopping_CartAPI.Models.DTO;

namespace Mango.Services.Shopping_CartAPI.Service.IService
{
    public interface ICartService
    {
        //public Task<CartDTO> GetCart(string userId);
        public  Task<APIResponse> CartUpsert(CartDTO cartDto);
        public Task<APIResponse> RemoveCart(int cartDetailsId);

    }
}
