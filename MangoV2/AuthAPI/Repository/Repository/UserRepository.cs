// File: UserRepository.cs
using AuthAPI.Models;
using AuthAPI.Models.DTO;
using AuthAPI.Repository.IRepository;
using AuthAPI.Utility;
using AuthAPI.Utility.PasswordUtility;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Identity;

namespace AuthAPI.Repository.Repository
{
    public class UserRepository : IRepository_
    {
        private readonly IDbConnection _dbConnection;

        public UserRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<ApplicationUser> GetUserByUsernameAsync(string username)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<ApplicationUser>(
                "SELECT * FROM ApplicationUsers WHERE UserName = @Username",
                new { Username = username });
        }

        public async Task<int> AssignRoleAsync(string email, string roleName)
        {
            var user = await _dbConnection.QueryFirstOrDefaultAsync<ApplicationUser>(
                "SELECT * FROM ApplicationUsers WHERE Email = @Email",
                new { Email = email });

            if (user != null)
            {
                // Check if the role exists; if not, insert it
                var roleId = await _dbConnection.QueryFirstOrDefaultAsync<int>(
                    "SELECT Id FROM IdentityRoles WHERE Name = @RoleName",
                    new { RoleName = roleName });

                if (roleId == 0)
                {
                    var newRole = new IdentityRole
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = roleName,
                        NormalizedName = roleName.ToUpper(),
                        ConcurrencyStamp = null
                    };

                    roleId = await _dbConnection.InsertAsync(newRole);
                }

                // Assign the role to the user
                return await _dbConnection.ExecuteAsync(
                    "INSERT INTO IdentityUserRole (UserId, RoleId) VALUES (@UserId, @RoleId)",
                    new { UserId = user.Id, RoleId = roleId });
            }

            return 0;
        }


        public async Task<string> RegisterUserAsync(RegistrationRequestDto registrationRequest)
        {
            var hashedPassword = PasswordHashUtility.HashPassword(registrationRequest.Password);

            var user = new ApplicationUser
            {
                UserName = registrationRequest.Email,
                Email = registrationRequest.Email,
                PasswordHash = hashedPassword,
                Name = registrationRequest.Name,
                PhoneNumber = registrationRequest.PhoneNumber,

                EmailConfirmed = false,
                SecurityStamp = Guid.NewGuid().ToString(),
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnd = null,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                NormalizedUserName = registrationRequest.Email.ToUpper(), // Set NormalizedUserName
                NormalizedEmail = registrationRequest.Email.ToUpper() // Set NormalizedEmail
            };

            var result = await _dbConnection.InsertAsync(user);

            return result > 0 ? "" : "Error Encountered";
        }

        public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
        {
            var user = await _dbConnection.QueryFirstOrDefaultAsync<ApplicationUser>(
                "SELECT * FROM ApplicationUsers WHERE UserName = @Username",
                new { Username = username });

            return user != null && PasswordHashUtility.VerifyPassword(user.PasswordHash, password);
        }

        public async Task<IEnumerable<string>> GetRolesAsync(string userId)
        {
            return await _dbConnection.QueryAsync<string>(
                "SELECT r.Name FROM IdentityRoles r " +
                "JOIN IdentityUserRole ur ON r.Id = ur.RoleId " +
                "WHERE ur.UserId = @UserId",
                new { UserId = userId });
        }
    }
}
