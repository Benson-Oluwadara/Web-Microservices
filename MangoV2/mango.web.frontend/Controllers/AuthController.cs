using mango.web.frontend.Models;
using mango.web.frontend.Models.WebDTO;
using mango.web.frontend.Services.Iservices;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using mango.web.frontend.Utility;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Serilog;

namespace mango.web.frontend.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService authService;
        private readonly ITokenProvider _tokenProvider;
        //private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthService authService, ITokenProvider tokenProvider/*, ILogger<AuthController> logger*/)
        {
            this.authService = authService;
            _tokenProvider = tokenProvider;
            //_logger = logger;
        }
        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto loginRequestDto = new();
            Log.Information("{User} accessed the Login page.", User.Identity.Name);
            return View(loginRequestDto);
        }
        
        [HttpGet]
        public IActionResult Register()
        {
            Log.Information("{User} accessed the Register page.", User.Identity.Name);

            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=SD.RoleAdmin,Value=SD.RoleAdmin},
                new SelectListItem{Text=SD.RoleCustomer,Value=SD.RoleCustomer},
            };

            ViewBag.RoleList = roleList;
            

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto obj)
        {
            //Console.WriteLine("Register Get Second Method");
            try
            {
                Log.Information("{User} submitted registration form.", User.Identity.Name);

                WebAPIResponse responseDto = await authService.RegisterAsync<WebAPIResponse>(obj);
                WebAPIResponse assignRole;
                //get the urk 
                //string check = SD.AuthAPIBase.TrimEnd('/') + "/api/AuthAPI/Register";
                //Console.WriteLine("Url is:" + check);

                if (responseDto != null && responseDto.IsSuccess)
                {
                    //print responseDTO valus
                    //Console.WriteLine(" inner method Response DTO: " + JsonConvert.SerializeObject(responseDto));
                    if (string.IsNullOrEmpty(obj.Role))
                    {
                        obj.Role = SD.RoleCustomer;
                        //print obj.Role
                        //Console.WriteLine("Role is:" + obj.Role);
                    }
                    assignRole = await authService.AssignRoleAsync<WebAPIResponse>(obj.Email, obj.Role);
                    //print assignRole
                    //Console.WriteLine("Assign Role: " + JsonConvert.SerializeObject(assignRole));
                    if (assignRole != null && assignRole.IsSuccess)
                    {
                        TempData["success"] = "Registration Successful";
                        Log.Information($"Registration successful. Email: {obj.Email}");
                        //Console.WriteLine($"Registration successful. Email: {obj.Email}");
                        return RedirectToAction("Login");
                    }
                }
                else
                {
                    Log.Warning($"Registration failed. Errors: {string.Join(", ", responseDto.ErrorMessages)}");
                    TempData["error"] = string.Join(", ", responseDto.ErrorMessages);

                }

                var roleList = new List<SelectListItem>()
        {
            new SelectListItem{Text=SD.RoleAdmin,Value=SD.RoleAdmin},
            new SelectListItem{Text=SD.RoleCustomer,Value=SD.RoleCustomer},
        };

                ViewBag.RoleList = roleList;

                return View(obj);
            }
            catch (Exception ex)
            {
                Log.Error($"An exception occurred during registration: {ex.Message}");
                // Optionally, you can log the stack trace as well: _logger.LogError($"StackTrace: {ex.StackTrace}");
                TempData["error"] = $"An unexpected error occurred during registration.";
                return View(obj);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    var response = await authService.LoginAsync<WebAPIResponse>(loginRequestDto);

                    if (response.IsSuccess && response != null)
                    {
                        // Handle successful login, e.g., set authentication cookie, redirect to dashboard
                        // You may use _tokenProvider to generate and manage tokens
                        LoginResponseDto loginResponseDto =
                            JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(response.Result));

                        await SignInUser(loginResponseDto);
                        _tokenProvider.SetToken(loginResponseDto.Token);
                        Log.Information("User {Username} logged in successfully. Token: {Token}", loginRequestDto.UserName, loginResponseDto.Token);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        Log.Warning("User {Username} login failed. Error: {ErrorMessages}", loginRequestDto.UserName, string.Join(", ", response.ErrorMessages));

                        // Handle unsuccessful login, display error messages to the user
                        //_logger.LogWarning("Login failed. Errors:");
                        foreach (var errorMessage in response.ErrorMessages)
                        {
                            ModelState.AddModelError(string.Empty, errorMessage);
                            Log.Warning(errorMessage);
                        }

                    }
                }

                Log.Warning($"Invalid login request. Username: {loginRequestDto.UserName}");
                return View(loginRequestDto);
            }
            catch (Exception ex)
            {
                Log.Error($"An exception occurred during login: {ex.Message}");
                // Optionally, you can log the stack trace as well: _logger.LogError($"StackTrace: {ex.StackTrace}");
                ModelState.AddModelError(string.Empty, $"An unexpected error occurred during login.");
                return View(loginRequestDto);
            }
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Sign out the user
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                _tokenProvider.ClearToken();
                Log.Information("User {Username} logged out.", User.Identity.Name);
                // Optionally, you can redirect the user to a specific page after logout
                return RedirectToAction("Index", "Home");
            }
            catch(Exception ex)
            {
                Log.Error(ex, "An exception occurred during logout: {ErrorMessage}", ex.Message);
                return RedirectToAction("Index", "Home");
            }
        }


        

        private async Task SignInUser(LoginResponseDto model)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(model.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));


            identity.AddClaim(new Claim(ClaimTypes.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(ClaimTypes.Role,
                jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));



            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

    }
}
