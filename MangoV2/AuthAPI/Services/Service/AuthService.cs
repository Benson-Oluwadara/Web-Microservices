using AuthAPI.Data;
using AuthAPI.Models.DTO;
using AuthAPI.Models;
using AuthAPI.Services.IService;
using Microsoft.AspNetCore.Identity;
using AuthAPI.Repository.IRepository;

namespace AuthAPI.Services.Service
{
    public class AuthService : IAuthService
    {
        private readonly IRepository_ _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(IRepository_ userRepository, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _jwtTokenGenerator = jwtTokenGenerator ?? throw new ArgumentNullException(nameof(jwtTokenGenerator));
        }
        public async Task<bool> IsUsernameUnique(string username)
        {
            var existingUser = await _userRepository.GetUserByUsernameAsync(username);
            return existingUser == null;
        }
        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            if (!await IsUsernameUnique(registrationRequestDto.Email))
            {
                return "Username is already taken.";
            }

            // Proceed with the registration logic
            return await _userRepository.RegisterUserAsync(registrationRequestDto);
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = await _userRepository.GetUserByUsernameAsync(loginRequestDto.UserName);

            if (user == null || !await _userRepository.ValidateUserCredentialsAsync(user.UserName, loginRequestDto.Password))
            {
                return new LoginResponseDto { User = null, Token = "" };
            }

            var roles = await _userRepository.GetRolesAsync(user.Id);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            var userDTO = new UserDto
            {
                Email = user.Email,
                ID = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };

            return new LoginResponseDto { User = userDTO, Token = token };
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var result = await _userRepository.AssignRoleAsync(email, roleName);
            return result > 0;
        }

        // ... other methods ...
    }

}