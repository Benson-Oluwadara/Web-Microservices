using Mango.Services.EmailAPI.Message;
using Mango.Services.EmailAPI.Models.DTO;

namespace Mango.Services.EmailAPI.Services.IServices
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDTO cartdto);
        Task RegisterUserEmailAndLog(string email);
        Task LogOrderPlaced(RewardsMessage rewardsDTO);
    }

}

