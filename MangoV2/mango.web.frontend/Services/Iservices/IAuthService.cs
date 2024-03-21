using mango.web.frontend.Models;
using mango.web.frontend.Models.WebDTO;

namespace mango.web.frontend.Services.Iservices
{
    public interface IAuthService
    {
        
        Task<T> RegisterAsync<T>(RegistrationRequestDto registrationRequestDto);
        Task<T> LoginAsync<T>(LoginRequestDto loginRequestDto);
        //Task<T> AssignRoleAsync<T>(RegistrationRequestDto registrationRequestDto);
        Task<T> AssignRoleAsync<T>(string email, string roleName);


    }
}
