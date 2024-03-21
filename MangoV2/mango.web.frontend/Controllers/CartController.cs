using mango.web.frontend.Models;
using mango.web.frontend.Models.WebDTO;
using mango.web.frontend.Services.Iservices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace mango.web.frontend.Controllers
{
    public class CartController : Controller
    {
        private ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDTOBasedOnLoggedInUser());
        }

        private async Task<CartDTO> LoadCartDTOBasedOnLoggedInUser()
        {
            var userid = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            var response = await _cartService.GetCartByUserIdAsnyc<WebAPIResponse>(userid);

            if (response != null && response.IsSuccess)
            {
                // Print all the response
                Console.WriteLine($"Response Content for LoadCart: {JsonConvert.SerializeObject(response)}");

                // Deserialize the response to CartDTO
                CartDTO cartDTO = JsonConvert.DeserializeObject<CartDTO>(Convert.ToString(response.Result));

                // Check if CartDetails is not null before accessing its properties
                if (cartDTO.CartDetails != null)
                {
                    // Print the cart details
                    foreach (var item in cartDTO.CartDetails)
                    {
                        Console.WriteLine($"CartDetailsId: {item.CartDetailsId}, CartHeaderId: {item.CartHeaderId}, ProductId: {item.ProductId}, Count: {item.Count}");
                    }
                }
                else
                {
                    Console.WriteLine("CartDetails is null");
                }

                // Print individual properties of the result
                Console.WriteLine($"CartHeaderId: {cartDTO.CartHeader.CartHeaderId}");
                Console.WriteLine($"UserId: {cartDTO.CartHeader.UserId}");
                Console.WriteLine($"CouponCode: {cartDTO.CartHeader.CouponCode}");

                foreach (var cartDetail in cartDTO.CartDetails)
                {
                    Console.WriteLine($"CartDetailsId: {cartDetail.CartDetailsId}, ProductId: {cartDetail.ProductId}, Count: {cartDetail.Count}");
                }

                return cartDTO;
            }
            else
            {
                Console.WriteLine($"Error retrieving cart. StatusCode: {response?.StatusCode}, IsSuccess: {response?.IsSuccess}, ErrorMessage: {response?.ErrorMessages}");
            }

            return new CartDTO();
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            var  response = await _cartService.RemoveFromCartAsync<WebAPIResponse>(cartDetailsId);
            
            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDTO cartDto)
        {

            var  response = await _cartService.ApplyCouponAsync<WebAPIResponse>(cartDto);
            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> EmailCart(CartDTO cartDto)
        //{
        //    CartDTO cartDTO= await LoadCartDTOBasedOnLoggedInUser();
        //    cartDTO.CartHeader.Email=User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;

        //    var response = await _cartService.EmailCart<WebAPIResponse>(cartDTO);
        //    if (response != null & response.IsSuccess)
        //    {
        //        TempData["success"] = "Email will be processed and sent shortly";
        //        return RedirectToAction(nameof(CartIndex));
        //    }
        //    return View();
        //}
        [HttpPost]
        public async Task<IActionResult> EmailCart()
        {
            // Load the CartDTO based on the logged-in user
            CartDTO cartDTO = await LoadCartDTOBasedOnLoggedInUser();

            // Check if the loaded CartDTO is null or if it has empty CartDetails
            if (cartDTO == null || cartDTO.CartDetails == null)
            {
                // Log or handle the case where the CartDTO or CartDetails is null or empty
                Console.WriteLine("Error: CartDTO or CartDetails is null or empty");
                return RedirectToAction(nameof(CartIndex));
            }

            // Update the email in CartHeader if available
            cartDTO.CartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;

            // Call the service to email the cart
            var response = await _cartService.EmailCart<WebAPIResponse>(cartDTO);

            // Check if the response is null
            if (response == null)
            {
                // Log or handle the case where the response is null
                Console.WriteLine("Error: Response from service is null");
                return RedirectToAction(nameof(CartIndex));
            }

            // Check if the response indicates success
            if (response.IsSuccess)
            {
                // Log or print out the success message
                Console.WriteLine("Email will be processed and sent shortly");
                TempData["success"] = "Email will be processed and sent shortly";
                return RedirectToAction(nameof(CartIndex));
            }
            else
            {
                // Log or handle the case where the service response indicates failure
                Console.WriteLine($"Error sending email. StatusCode: {response?.StatusCode}, IsSuccess: {response?.IsSuccess}, ErrorMessage: {response?.ErrorMessages}");
                return RedirectToAction(nameof(CartIndex));
            }
        }


    }
}
