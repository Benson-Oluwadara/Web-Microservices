using AuthAPI.Models.DTO;
using AuthAPI.Models;

namespace AuthAPI.Repository.IRepository
{
    public interface IRepository_
    {
        Task<ApplicationUser> GetUserByUsernameAsync(string username);
        Task<int> AssignRoleAsync(string email, string roleName);
        Task<string> RegisterUserAsync(RegistrationRequestDto registrationRequest);
        Task<bool> ValidateUserCredentialsAsync(string username, string password);
        Task<IEnumerable<string>> GetRolesAsync(string userId);
    }
}
