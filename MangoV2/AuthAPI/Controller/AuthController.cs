using AuthAPI.Models.DTO;
using AuthAPI.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AuthAPI.Controller
{
    [Route("api/AuthAPI")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto registrationRequest)
        {
            try
            {
                _logger.LogInformation("Register endpoint called.");

                // Check if the username is unique before proceeding with registration
                bool isUnique = await _authService.IsUsernameUnique(registrationRequest.Email);
                if (!isUnique)
                {
                    _logger.LogWarning("Username is already taken.");
                    return BadRequest(new ResponseDto { IsSuccess = false, Message = "Username is already taken." });
                }

                // Proceed with the rest of the registration logic
                string result = await _authService.Register(registrationRequest);
                if (string.IsNullOrEmpty(result))
                {
                    _logger.LogInformation("Registration successful.");
                    return Ok(new ResponseDto { IsSuccess = true, Message = "Registration successful." });
                }
                else
                {
                    _logger.LogWarning($"Registration failed. Reason: {result}");
                    return BadRequest(new ResponseDto { IsSuccess = false, Message = result });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during registration.");
                return StatusCode(500, new ResponseDto { IsSuccess = false, Message = "Internal server error." });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                _logger.LogInformation("Login endpoint called.");

                LoginResponseDto result = await _authService.Login(loginRequest);
                if (result.User != null)
                {
                    _logger.LogInformation("Login successful.");
                    return Ok(new ResponseDto { IsSuccess = true, Result = result, Message = "Login successful." });
                }
                else
                {
                    _logger.LogWarning("Invalid username or password.");
                    return BadRequest(new ResponseDto { IsSuccess = false, Message = "Invalid username or password." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return StatusCode(500, new ResponseDto { IsSuccess = false, Message = "Internal server error." });
            }
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] RoleAssignmentDto roleAssignment)
        {
            try
            {
                _logger.LogInformation("AssignRole endpoint called.");

                bool success = await _authService.AssignRole(roleAssignment.Email, roleAssignment.Role);
                if (success)
                {
                    _logger.LogInformation("Role assigned successfully.");
                    return Ok(new ResponseDto { IsSuccess = true, Message = "Role assigned successfully." });
                }
                else
                {
                    _logger.LogWarning("Error assigning role.");
                    return BadRequest(new ResponseDto { IsSuccess = false, Message = "Error assigning role." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during role assignment.");
                return StatusCode(500, new ResponseDto { IsSuccess = false, Message = "Internal server error." });
            }
        }
    }
}
