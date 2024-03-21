using System;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using mango.web.frontend.Models;
using mango.web.frontend.Models.WebDTO;
using mango.web.frontend.Services.Iservices;
using mango.web.frontend.Utility;

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
                    Data = new { Email = email, RoleName = roleName }, // Adjust the data structure based on your API
                    Url = SD.AuthAPIBase+"api/AuthAPI/assign-role" // Adjust the URL based on your controller endpoint
                }, "AuthAPI");

                return apiResponse;
            }
            catch (Exception ex)
            {
                var errorResponse = new WebAPIResponse
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message }

                };

                var errorResponseJson = JsonConvert.SerializeObject(errorResponse);
                return JsonConvert.DeserializeObject<T>(errorResponseJson);
            }
        }

        
        public async Task<T> LoginAsync<T>(LoginRequestDto loginRequestDto)
        {
            string check = SD.AuthAPIBase.TrimEnd('/') + "/api/AuthAPI/login";
            try
            {
                var apiResponse = await _baseService.SendAsync<T>(new WebAPIRequest()
                {
                    apiType = SD.ApiType.POST, // Adjust the API type according to your needs
                    Data = loginRequestDto, // Adjust the data structure based on your API
                    //Url =SD.AuthAPIBase+ "api/AuthAPI/login" // Adjust the URL based on your controller endpoint
                    Url = SD.AuthAPIBase.TrimEnd('/') + "/api/AuthAPI/login"

                }, "AuthAPI");

                Console.WriteLine("Url is:"+SD.AuthAPIBase + "api/AuthAPI/login");
                //get value of Url
                Console.WriteLine($"Url is:"+check);
                

                Console.WriteLine("API Response: " + JsonConvert.SerializeObject(apiResponse));


                return apiResponse;
            }
            catch (Exception ex)
            {
                var errorResponse = new WebAPIResponse
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message }

                };

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
                    Url =SD.AuthAPIBase+ "api/AuthAPI/Register" // Adjust the URL based on your controller endpoint
                }, "AuthAPI");

                return apiResponse;
            }
            catch (Exception ex)
            {
                var errorResponse = new WebAPIResponse
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message }
                };

                var errorResponseJson = JsonConvert.SerializeObject(errorResponse);
                return JsonConvert.DeserializeObject<T>(errorResponseJson);
            }
        }
    }
}
