using AuthAPI.Models;
using AuthAPI.Services.IService;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthAPI.Services.Service
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            ValidateJwtOptions();
        }

        public string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles)
        {
            // Access configuration values directly
            var secret = _configuration["ApiSettings:JwtOptions:Secret"];
            var audience = _configuration["ApiSettings:JwtOptions:Audience"];
            var issuer = _configuration["ApiSettings:JwtOptions:SecretIssuer"];
            Console.WriteLine($"Secret: {secret}, Audience: {audience}, Issuer: {issuer}");

            if (string.IsNullOrEmpty(secret))
            {
                throw new ArgumentException("JWT secret key is required.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(secret);

            var claimList = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, applicationUser.Email),
                new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Id),
                new Claim(JwtRegisteredClaimNames.Name, applicationUser.UserName)
            };

            claimList.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = audience,
                Issuer = issuer,
                Subject = new ClaimsIdentity(claimList),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private void ValidateJwtOptions()
        {
            var secret = _configuration["ApiSettings:JwtOptions:Secret"];
            var audience = _configuration["ApiSettings:JwtOptions:Audience"];
            var issuer = _configuration["ApiSettings:JwtOptions:SecretIssuer"];

            Console.WriteLine($"Validating JWT Options - Secret: {secret}, Audience: {audience}, Issuer: {issuer}");

            if (string.IsNullOrEmpty(secret))
            {
                throw new ArgumentException("JWT secret key is required.");
            }

            // Add more validation as needed
        }
    }
}
