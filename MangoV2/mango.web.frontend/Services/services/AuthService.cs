using System;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using mango.web.frontend.Models;
using mango.web.frontend.Models.WebDTO;
using mango.web.frontend.Services.Iservices;
using mango.web.frontend.Utility;
using Serilog;
using Newtonsoft.Json;

namespace Mango.web.frontend.Services.services
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;

        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<T> AssignRoleAsync<T>(string email, string roleName)
        {
            try
            {
                var apiResponse = await _baseService.SendAsync<T>(new WebAPIRequest()
                {
                    apiType = SD.ApiType.POST, // Adjust the API type according to your needs
                    Data = new { Email = email, Role = roleName }, // Adjust the data structure based on your API
                    Url = SD.AuthAPIBase+"/api/AuthAPI/assign-role" // Adjust the URL based on your controller endpoint
                }, "AuthAPI");
                //log in this method
                Log.Information($"Role assigned to user: {email}");
                return apiResponse;
            }
            catch (Exception ex)
            {
                var errorResponse = new WebAPIResponse
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message }

                };
                //log in this method
                Log.Error($"Error assigning role to user: {email}");
                var errorResponseJson = JsonConvert.SerializeObject(errorResponse);
                return JsonConvert.DeserializeObject<T>(errorResponseJson);
            }
        }

        
        public async Task<T> LoginAsync<T>(LoginRequestDto loginRequestDto)
        {
            
            try
            {
                var apiResponse = await _baseService.SendAsync<T>(new WebAPIRequest()
                {
                    apiType = SD.ApiType.POST, // Adjust the API type according to your needs
                    Data = loginRequestDto, // Adjust the data structure based on your API
                    //Url =SD.AuthAPIBase+ "api/AuthAPI/login" // Adjust the URL based on your controller endpoint
                    Url = SD.AuthAPIBase.TrimEnd('/') + "/api/AuthAPI/login"

                }, "AuthAPI");
                //log in this method
                Log.Information($"User logged in: {loginRequestDto.UserName}");
                

                return apiResponse;
            }
            catch (Exception ex)
            {
                var errorResponse = new WebAPIResponse
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message }

                };
                //log in this method
                Log.Error($"Error logging in user: {loginRequestDto.UserName}");
                var errorResponseJson = JsonConvert.SerializeObject(errorResponse);
                return JsonConvert.DeserializeObject<T>(errorResponseJson);
            }
        }

        public async Task<T> RegisterAsync<T>(RegistrationRequestDto registrationRequestDto)
        {
            try
            {
                var apiResponse = await _baseService.SendAsync<T>(new WebAPIRequest()
                {
                    apiType = SD.ApiType.POST, // Adjust the API type according to your needs
                    Data = registrationRequestDto, // Adjust the data structure based on your API
                    //Url =SD.AuthAPIBase+ "api/AuthAPI/Register" // Adjust the URL based on your controller endpoint
                     Url = SD.AuthAPIBase + "/api/AuthAPI/Register"

                }, "AuthAPI");
            //https://localhost:6002/api/AuthAPI/Register
            //log in this method
            Log.Information($"User registered: {registrationRequestDto.Email}");
                return apiResponse;
            }
            catch (Exception ex)
            {
                var errorResponse = new WebAPIResponse
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message }
                };
                //log in this method
                Log.Error($"Error registering user: {registrationRequestDto.Email}");
                var errorResponseJson = JsonConvert.SerializeObject(errorResponse);
                return JsonConvert.DeserializeObject<T>(errorResponseJson);
            }
        }
    }
}
