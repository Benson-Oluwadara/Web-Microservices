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
using Serilog;
using System.Data.Common;

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
            try
            {
                Log.Information("Fetching user by username: {@Username}", username);

                return await _dbConnection.QueryFirstOrDefaultAsync<ApplicationUser>(
                "SELECT * FROM ApplicationUsers WHERE UserName = @Username",
                new { Username = username });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while fetching user by username: {@Username}", username);
                throw;
            }
        }
            

        //public async Task<int> AssignRoleAsync(string email, string roleName)
        //{
        //    var user = await _dbConnection.QueryFirstOrDefaultAsync<ApplicationUser>(
        //        "SELECT * FROM ApplicationUsers WHERE Email = @Email",
        //        new { Email = email });

        //    if (user != null)
        //    {
        //        // Check if the role exists; if not, insert it
        //        var roleId = await _dbConnection.QueryFirstOrDefaultAsync<int>(
        //            "SELECT Id FROM IdentityRoles WHERE Name = @RoleName",
        //            new { RoleName = roleName });

        //        if (roleId == 0)
        //        {
        //            var newRole = new IdentityRole
        //            {
        //                Id = Guid.NewGuid().ToString(),
        //                Name = roleName,
        //                NormalizedName = roleName.ToUpper(),
        //                ConcurrencyStamp = null
        //            };

        //            roleId = await _dbConnection.InsertAsync(newRole);
        //        }

        //        // Assign the role to the user
        //        return await _dbConnection.ExecuteAsync(
        //            "INSERT INTO IdentityUserRole (UserId, RoleId) VALUES (@UserId, @RoleId)",
        //            new { UserId = user.Id, RoleId = roleId });
        //    }

        //    return 0;
        //}

   
    public async Task<int> AssignRoleAsync(string email, string roleName)
        {
            try
            {
                Log.Information("AssignRoleAsync method called with email: {Email} and roleName: {RoleName}", email, roleName);

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

                        Log.Information("Role {RoleName} inserted into database with ID: {RoleId}", roleName, roleId);
                    }

                    // Assign the role to the user
                    var result = await _dbConnection.ExecuteAsync(
                        "INSERT INTO IdentityUserRole (UserId, RoleId) VALUES (@UserId, @RoleId)",
                        new { UserId = user.Id, RoleId = roleId });

                    Log.Information("Role {RoleName} assigned to user with email: {Email}", roleName, email);

                    return result;
                }
                else
                {
                    Log.Warning("User with email: {Email} not found", email);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred in AssignRoleAsync method with email: {Email} and roleName: {RoleName}", email, roleName);
                throw;
            }
        }

        //public async Task<string> RegisterUserAsync(RegistrationRequestDto registrationRequest)
        //    {
        //        var hashedPassword = PasswordHashUtility.HashPassword(registrationRequest.Password);

        //        var user = new ApplicationUser
        //        {
        //            UserName = registrationRequest.Email,
        //            Email = registrationRequest.Email,
        //            PasswordHash = hashedPassword,
        //            Name = registrationRequest.Name,
        //            PhoneNumber = registrationRequest.PhoneNumber,

        //            EmailConfirmed = false,
        //            SecurityStamp = Guid.NewGuid().ToString(),
        //            PhoneNumberConfirmed = false,
        //            TwoFactorEnabled = false,
        //            LockoutEnd = null,
        //            LockoutEnabled = true,
        //            AccessFailedCount = 0,
        //            NormalizedUserName = registrationRequest.Email.ToUpper(), // Set NormalizedUserName
        //            NormalizedEmail = registrationRequest.Email.ToUpper() // Set NormalizedEmail
        //        };

        //        var result = await _dbConnection.InsertAsync(user);

        //        return result > 0 ? "" : "Error Encountered";
        //    }
        public async Task<string> RegisterUserAsync(RegistrationRequestDto registrationRequest)
        {
            try
            {
                Log.Information("RegisterUserAsync method called with registration request: {@RegistrationRequest}", registrationRequest);

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
                    NormalizedUserName = registrationRequest.Email.ToUpper(),
                    NormalizedEmail = registrationRequest.Email.ToUpper()
                };

                var result = await _dbConnection.InsertAsync(user);

                if (result > 0)
                {
                    Log.Information("User registration successful for email: {Email}", registrationRequest.Email);
                    return "";
                }
                else
                {
                    Log.Warning("User registration failed for email: {Email}", registrationRequest.Email);
                    return "Error Encountered";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred in RegisterUserAsync method with registration request: {@RegistrationRequest}", registrationRequest);
                throw;
            }
        }

        //public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
        //{
        //    var user = await _dbConnection.QueryFirstOrDefaultAsync<ApplicationUser>(
        //        "SELECT * FROM ApplicationUsers WHERE UserName = @Username",
        //        new { Username = username });

        //    return user != null && PasswordHashUtility.VerifyPassword(user.PasswordHash, password);
        //}

        //public async Task<IEnumerable<string>> GetRolesAsync(string userId)
        //{
        //    return await _dbConnection.QueryAsync<string>(
        //        "SELECT r.Name FROM IdentityRoles r " +
        //        "JOIN IdentityUserRole ur ON r.Id = ur.RoleId " +
        //        "WHERE ur.UserId = @UserId",
        //        new { UserId = userId });
        //}

        public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
        {
            try
            {
                Log.Information("Validating user credentials for username: {Username}", username);

                var user = await _dbConnection.QueryFirstOrDefaultAsync<ApplicationUser>(
                    "SELECT * FROM ApplicationUsers WHERE UserName = @Username",
                    new { Username = username });

                if (user != null && PasswordHashUtility.VerifyPassword(user.PasswordHash, password))
                {
                    Log.Information("User credentials validation successful for username: {Username}", username);
                    return true;
                }
                else
                {
                    Log.Warning("Invalid credentials provided for username: {Username}", username);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while validating user credentials for username: {Username}", username);
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetRolesAsync(string userId)
        {
            try
            {
                Log.Information("Fetching roles for user with ID: {UserId}", userId);

                return await _dbConnection.QueryAsync<string>(
                    "SELECT r.Name FROM IdentityRoles r " +
                    "JOIN IdentityUserRole ur ON r.Id = ur.RoleId " +
                    "WHERE ur.UserId = @UserId",
                    new { UserId = userId });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching roles for user with ID: {UserId}", userId);
                throw;
            }
        }

    }
}
