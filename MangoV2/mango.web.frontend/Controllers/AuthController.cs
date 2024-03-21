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

namespace mango.web.frontend.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService authService;
        private readonly ITokenProvider _tokenProvider;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthService authService, ITokenProvider tokenProvider, ILogger<AuthController> logger)
        {
            this.authService = authService;
            _tokenProvider = tokenProvider;
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto loginRequestDto = new();
            return View(loginRequestDto);
        }
        //[HttpPost]
        //public async Task<IActionResult> Login(LoginRequestDto obj)
        //{
        //    WebAPIResponse<LoginResponseDto> responseDto = await authService.LoginAsync<LoginResponseDto>(obj);

        //    if (responseDto != null && responseDto.IsSuccess)
        //    {
        //        LoginResponseDto loginResponseDto = responseDto.Result;

        //        if (loginResponseDto != null)
        //        {
        //            await SignInUser(loginResponseDto);
        //            _tokenProvider.SetToken(loginResponseDto.Token);
        //            return RedirectToAction("Index", "Home");
        //        }
        //        else
        //        {
        //            // Handle the case when loginResponseDto is null
        //            // Log a warning or take appropriate action
        //            return View(obj);
        //        }
        //    }
        //    else
        //    {
        //        TempData["error"] = responseDto.Message; // Check if 'Message' is a valid property in WebAPIResponse
        //        return View(obj);
        //    }
        //}
       

        //[HttpPost]
        //public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var response = await authService.LoginAsync<WebAPIResponse>(loginRequestDto);

        //        if (response.IsSuccess && response!=null)
        //        {
        //            // Handle successful login, e.g., set authentication cookie, redirect to dashboard
        //            // You may use _tokenProvider to generate and manage tokens
        //            LoginResponseDto loginResponseDto =
        //            JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(response.Result));

        //            await SignInUser(loginResponseDto);
        //            _tokenProvider.SetToken(loginResponseDto.Token);
        //            _logger.LogInformation($"Login successful. Token: {loginResponseDto.Token}");
        //            return RedirectToAction("Index", "Home");
        //        }
        //        else
        //        {
        //            _logger.LogWarning($"Login failed. Error: {response.ErrorMessages}");
        //            // Handle unsuccessful login, display error messages to the user
        //            foreach (var errorMessage in response.ErrorMessages)
        //            {
        //                ModelState.AddModelError(string.Empty, errorMessage);
        //            }
                    
        //        }
        //    }

        //    _logger.LogWarning($"Invalid login request. Username: {loginRequestDto.UserName}");
        //    return View(loginRequestDto);
        //}

        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=SD.RoleAdmin,Value=SD.RoleAdmin},
                new SelectListItem{Text=SD.RoleCustomer,Value=SD.RoleCustomer},
            };

            ViewBag.RoleList = roleList;
            return View();
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
                        _logger.LogInformation($"Login successful. Token: {loginResponseDto.Token}");
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        _logger.LogWarning($"Login failed. Error: {response.ErrorMessages}");
                        
                        // Handle unsuccessful login, display error messages to the user
                        //_logger.LogWarning("Login failed. Errors:");
                        foreach (var errorMessage in response.ErrorMessages)
                        {
                            ModelState.AddModelError(string.Empty, errorMessage);
                            _logger.LogWarning(errorMessage);
                        }

                    }
                }

                _logger.LogWarning($"Invalid login request. Username: {loginRequestDto.UserName}");
                return View(loginRequestDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An exception occurred during login: {ex.Message}");
                // Optionally, you can log the stack trace as well: _logger.LogError($"StackTrace: {ex.StackTrace}");
                ModelState.AddModelError(string.Empty, $"An unexpected error occurred during login.");
                return View(loginRequestDto);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto obj)
        {
            try
            {
                WebAPIResponse responseDto = await authService.RegisterAsync<WebAPIResponse>(obj);
                WebAPIResponse assignRole;

                if (responseDto != null && responseDto.IsSuccess)
                {
                    if (string.IsNullOrEmpty(obj.Role))
                    {
                        obj.Role = SD.RoleCustomer;
                    }
                    assignRole = await authService.AssignRoleAsync<WebAPIResponse>(obj.Email, obj.Role);
                    if (assignRole != null && assignRole.IsSuccess)
                    {
                        TempData["success"] = "Registration Successful";
                        _logger.LogInformation($"Registration successful. Email: {obj.Email}");
                        return RedirectToAction(nameof(Login));
                    }
                }
                else
                {
                    _logger.LogWarning($"Registration failed. Error: {responseDto.ErrorMessages}");
                    TempData["error"] = responseDto.ErrorMessages;
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
                _logger.LogError($"An exception occurred during registration: {ex.Message}");
                // Optionally, you can log the stack trace as well: _logger.LogError($"StackTrace: {ex.StackTrace}");
                TempData["error"] = $"An unexpected error occurred during registration.";
                return View(obj);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Sign out the user
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _tokenProvider.ClearToken();
            // Optionally, you can redirect the user to a specific page after logout
            return RedirectToAction("Index", "Home");
        }


        //[HttpPost]
        //public async Task<IActionResult> Register(RegistrationRequestDto obj)
        //{
        //    WebAPIResponse responseDto = await authService.RegisterAsync<WebAPIResponse>(obj);
        //    WebAPIResponse assingRole;

        //    if (responseDto != null && responseDto.IsSuccess)
        //    {
        //        if (string.IsNullOrEmpty(obj.Role))
        //        {
        //            obj.Role = SD.RoleCustomer;
        //        }
        //        assingRole = await authService.AssignRoleAsync<WebAPIResponse>(obj.Email,obj.Role );
        //        if (assingRole != null && assingRole.IsSuccess)
        //        {

        //            TempData["success"] = "Registration Successful";
        //            _logger.LogInformation($"Registration successful. Email: {obj.Email}");
        //            return RedirectToAction(nameof(Login));
        //        }
        //    }
        //    else
        //    {
        //        _logger.LogWarning($"Registration failed. Error: {responseDto.ErrorMessages}");
        //        TempData["error"] = responseDto.ErrorMessages;
        //    }

        //    var roleList = new List<SelectListItem>()
        //    {
        //        new SelectListItem{Text=SD.RoleAdmin,Value=SD.RoleAdmin},
        //        new SelectListItem{Text=SD.RoleCustomer,Value=SD.RoleCustomer},
        //    };

        //    ViewBag.RoleList = roleList;

        //    return View(obj);
        //}

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
