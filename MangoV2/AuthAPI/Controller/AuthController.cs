using AuthAPI.Models.DTO;
using AuthAPI.Services.IService;
using mango.messagebus;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Microsoft.Extensions.Configuration;
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
        //private readonly ILogger<AuthController> _logger;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        private readonly string registerUser;
        public AuthController(IAuthService authService, IMessageBus messageBus, IConfiguration configuration)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
           // _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
            _configuration = configuration;
            registerUser = _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue");

        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto registrationRequest)
        {
            try
            {
                //_logger.LogInformation("Register endpoint called.");
                Log.Information("Register endpoint called {@RegistrationRequest}", registrationRequest);
                //Console.WriteLine("Register endpoint called {@RegistrationRequest}", registrationRequest);
                // Check if the username is unique before proceeding with registration
                bool isUnique = await _authService.IsUsernameUnique(registrationRequest.Email);
                if (!isUnique)
                {
                    //_logger.LogWarning("Username is already taken.");
                    Log.Warning("Username is already taken {@RegistrationRequest}", registrationRequest);

                    return BadRequest(new ResponseDto { IsSuccess = false, Message = "Username is already taken." });
                }

                // Proceed with the rest of the registration logic
                string result = await _authService.Register(registrationRequest);
                if (string.IsNullOrEmpty(result))
                {
                    //_logger.LogInformation("Registration successful!!!!!!!!!!!next.");
                    Log.Information("Registration successful for {@RegistrationRequest}", registrationRequest);
                    await _messageBus.PublicMessage(registrationRequest.Email,"registeruser");
                    return Ok(new ResponseDto { IsSuccess = true, Message = "Registration successful." });
                }
                else
                {
                    // _logger.LogWarning($"Registration failed. Reason: {result}");
                    Log.Warning("Registration failed for {@RegistrationRequest}. Reason: {Reason}", registrationRequest, result);

                    return BadRequest(new ResponseDto { IsSuccess = false, Message = result });
                }
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "An error occurred during registration.");
                Log.Error(ex, "An error occurred during registration {@RegistrationRequest}", registrationRequest);

                return StatusCode(500, new ResponseDto { IsSuccess = false, Message = "Internal server error." });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                Log.Information("Login endpoint called {@LoginRequest}", loginRequest);
                //Console.WriteLine("Login endpoint called {@LoginRequest}", loginRequest);

                LoginResponseDto result = await _authService.Login(loginRequest);
                if (result.User != null)
                {
                    Log.Information("Login successful for {@LoginRequest}", loginRequest);

                    return Ok(new ResponseDto { IsSuccess = true, Result = result, Message = "Login successful." });
                }
                else
                {
                    Log.Warning("Invalid username or password {@LoginRequest}", loginRequest);

                    return BadRequest(new ResponseDto { IsSuccess = false, Message = "Invalid username or password." });
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred during login {@LoginRequest}", loginRequest);

                return StatusCode(500, new ResponseDto { IsSuccess = false, Message = "Internal server error." });
            }
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] RoleAssignmentDto roleAssignment)
        {
            try
            {
                //_logger.LogInformation("AssignRole endpoint called.");
                Log.Information("AssignRole endpoint called {@RoleAssignment}", roleAssignment);

                bool success = await _authService.AssignRole(roleAssignment.Email, roleAssignment.Role);
                if (success)
                {
                    Log.Information("Role assigned successfully for {@RoleAssignment}", roleAssignment);

                    return Ok(new ResponseDto { IsSuccess = true, Message = "Role assigned successfully." });
                }
                else
                {
                    Log.Warning("Error assigning role {@RoleAssignment}", roleAssignment);

                    return BadRequest(new ResponseDto { IsSuccess = false, Message = "Error assigning role." });
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An error occurred during role assignment.");
                Log.Error(ex, "An error occurred during role assignment {@RoleAssignment}", roleAssignment);

                return StatusCode(500, new ResponseDto { IsSuccess = false, Message = "Internal server error." });
            }
        }
    }
}
