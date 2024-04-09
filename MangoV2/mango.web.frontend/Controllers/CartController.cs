using mango.web.frontend.Models;
using mango.web.frontend.Models.WebDTO;
using mango.web.frontend.Services.Iservices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using Serilog;
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
            try
            {
                Log.Information("User {User} accessed the Cart Index page.", User.Identity.Name);
                return View(await LoadCartDTOBasedOnLoggedInUser());
            }
            catch(Exception ex)
            {
                Log.Error(ex, "An error occurred while retrieving cart details.");
                throw;
            }
            
        }
        public async Task<IActionResult> Checkout()
        {
            try
            {
                Log.Information("User {Username} accessed the Checkout page.", User.Identity.Name);

                // Load cart details based on logged-in user
                var cartDto = await LoadCartDTOBasedOnLoggedInUser();

                // Check if the cart details are valid
                if (cartDto == null)
                {
                    // Handle the case where cart details are not available
                    return RedirectToAction(nameof(CartIndex));
                }

                // Optionally, you can add additional logic here, such as calculating totals, applying discounts, etc.

                // Return the 'Checkout' view with the loaded cart details
                return View("Checkout", cartDto);
            }catch(Exception ex)
            {
                Log.Error(ex, "An error occurred while processing the checkout request.");
                throw;
            }
        }

        private async Task<CartDTO> LoadCartDTOBasedOnLoggedInUser()
        {
            try
            {
                var userid = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
                var response = await _cartService.GetCartByUserIdAsnyc<WebAPIResponse>(userid);

                if (response != null && response.IsSuccess)
                {
                    // Print all the response
                    Log.Information($"Response Content for LoadCart: {JsonConvert.SerializeObject(response)}");

                    // Deserialize the response to CartDTO
                    CartDTO cartDTO = JsonConvert.DeserializeObject<CartDTO>(Convert.ToString(response.Result));
                    // Log individual properties of the result
                    Log.Information("CartHeaderId: {CartHeaderId}, UserId: {UserId}, CouponCode: {CouponCode}", cartDTO.CartHeader.CartHeaderId, cartDTO.CartHeader.UserId, cartDTO.CartHeader.CouponCode);

                    // Check if CartDetails is not null before accessing its properties
                    //if (cartDTO.CartDetails != null)
                    //{
                    //    // Print the cart details
                    //    foreach (var item in cartDTO.CartDetails)
                    //    {
                    //        Console.WriteLine($"CartDetailsId: {item.CartDetailsId}, CartHeaderId: {item.CartHeaderId}, ProductId: {item.ProductId}, Count: {item.Count}");
                    //    }
                    //}
                    //else
                    //{
                    //    Console.WriteLine("CartDetails is null");
                    //}
                    //return new CartDTO();
                    // Print individual properties of the result
                    //Console.WriteLine($"CartHeaderId: {cartDTO.CartHeader.CartHeaderId}");
                    //Console.WriteLine($"UserId: {cartDTO.CartHeader.UserId}");
                    //Console.WriteLine($"CouponCode: {cartDTO.CartHeader.CouponCode}");

                    //foreach (var cartDetail in cartDTO.CartDetails)
                    //{
                    //    Console.WriteLine($"CartDetailsId: {cartDetail.CartDetailsId}, ProductId: {cartDetail.ProductId}, Count: {cartDetail.Count}");
                    //}

                    return cartDTO;
                }
                else
                {
                    Console.WriteLine($"Error retrieving cart. StatusCode: {response?.StatusCode}, IsSuccess: {response?.IsSuccess}, ErrorMessage: {response?.ErrorMessages}");
                    return null;
                }
            }
                        catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while loading cart details based on the logged-in user.");
                throw;
            }
            
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            try
            {
                var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
                var response = await _cartService.RemoveFromCartAsync<WebAPIResponse>(cartDetailsId);

                if (response != null && response.IsSuccess)
                {
                    Log.Information("Product with CartDetailsId {CartDetailsId} removed from the cart for user {Username}.", cartDetailsId, User.Identity.Name);
                    TempData["success"] = "Cart updated successfully";
                    return RedirectToAction(nameof(CartIndex));
                }

                // Log the failure to remove from cart
                Log.Error("Failed to remove product with CartDetailsId {CartDetailsId} from the cart for user {Username}.", cartDetailsId, User.Identity.Name);
                return View();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while removing product from cart for user {Username}.", User.Identity.Name);
                throw; // You may handle the exception according to your application's error handling strategy
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> ApplyCoupon(CartDTO cartDto)
        //{

        //    var  response = await _cartService.ApplyCouponAsync<WebAPIResponse>(cartDto);
        //    if (response != null & response.IsSuccess)
        //    {
        //        TempData["success"] = "Cart updated successfully";
        //        return RedirectToAction(nameof(CartIndex));
        //    }
        //    return View();
        //}

        ////[HttpPost]
        ////public async Task<IActionResult> EmailCart(CartDTO cartDto)
        ////{
        ////    CartDTO cartDTO= await LoadCartDTOBasedOnLoggedInUser();
        ////    cartDTO.CartHeader.Email=User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;

        ////    var response = await _cartService.EmailCart<WebAPIResponse>(cartDTO);
        ////    if (response != null & response.IsSuccess)
        ////    {
        ////        TempData["success"] = "Email will be processed and sent shortly";
        ////        return RedirectToAction(nameof(CartIndex));
        ////    }
        ////    return View();
        ////}
        //[HttpPost]
        //public async Task<IActionResult> EmailCart()
        //{
        //    // Load the CartDTO based on the logged-in user
        //    CartDTO cartDTO = await LoadCartDTOBasedOnLoggedInUser();

        //    // Check if the loaded CartDTO is null or if it has empty CartDetails
        //    if (cartDTO == null || cartDTO.CartDetails == null)
        //    {
        //        // Log or handle the case where the CartDTO or CartDetails is null or empty
        //        Console.WriteLine("Error: CartDTO or CartDetails is null or empty");
        //        return RedirectToAction(nameof(CartIndex));
        //    }

        //    // Update the email in CartHeader if available
        //    cartDTO.CartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;

        //    // Call the service to email the cart
        //    var response = await _cartService.EmailCart<WebAPIResponse>(cartDTO);

        //    // Check if the response is null
        //    if (response == null)
        //    {
        //        // Log or handle the case where the response is null
        //        Console.WriteLine("Error: Response from service is null");
        //        return RedirectToAction(nameof(CartIndex));
        //    }

        //    // Check if the response indicates success
        //    if (response.IsSuccess)
        //    {
        //        // Log or print out the success message
        //        Console.WriteLine("Email will be processed and sent shortly");
        //        TempData["success"] = "Email will be processed and sent shortly";
        //        return RedirectToAction(nameof(CartIndex));
        //    }
        //    else
        //    {
        //        // Log or handle the case where the service response indicates failure
        //        Console.WriteLine($"Error sending email. StatusCode: {response?.StatusCode}, IsSuccess: {response?.IsSuccess}, ErrorMessage: {response?.ErrorMessages}");
        //        return RedirectToAction(nameof(CartIndex));
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDTO cartDto)
        {
            try
            {
                var response = await _cartService.ApplyCouponAsync<WebAPIResponse>(cartDto);

                if (response != null && response.IsSuccess)
                {
                    Log.Information("Coupon applied successfully for user {Username}.", User.Identity.Name);
                    TempData["success"] = "Cart updated successfully";
                    return RedirectToAction(nameof(CartIndex));
                }

                // Log the failure to apply coupon
                Log.Error("Failed to apply coupon for user {Username}.", User.Identity.Name);
                return View();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while applying coupon for user {Username}.", User.Identity.Name);
                throw; // You may handle the exception according to your application's error handling strategy
            }
        }

        [HttpPost]
        public async Task<IActionResult> EmailCart()
        {
            try
            {
                var cartDTO = await LoadCartDTOBasedOnLoggedInUser();

                if (cartDTO == null || cartDTO.CartDetails == null)
                {
                    // Log or handle the case where the CartDTO or CartDetails is null or empty
                    Log.Error("Error: CartDTO or CartDetails is null or empty for user {Username}.", User.Identity.Name);
                    return RedirectToAction(nameof(CartIndex));
                }

                // Update the email in CartHeader if available
                cartDTO.CartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;

                var response = await _cartService.EmailCart<WebAPIResponse>(cartDTO);

                if (response == null)
                {
                    // Log or handle the case where the response is null
                    Log.Error("Error: Response from service is null when emailing cart for user {Username}.", User.Identity.Name);
                    return RedirectToAction(nameof(CartIndex));
                }

                if (response.IsSuccess)
                {
                    // Log or print out the success message
                    Log.Information("Email will be processed and sent shortly for user {Username}.", User.Identity.Name);
                    TempData["success"] = "Email will be processed and sent shortly";
                    return RedirectToAction(nameof(CartIndex));
                }
                else
                {
                    // Log or handle the case where the service response indicates failure
                    Log.Error("Error sending email for user {Username}. StatusCode: {StatusCode}, IsSuccess: {IsSuccess}, ErrorMessage: {ErrorMessages}", User.Identity.Name, response.StatusCode, response.IsSuccess, response.ErrorMessages);
                    return RedirectToAction(nameof(CartIndex));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while emailing cart for user {Username}.", User.Identity.Name);
                throw; // You may handle the exception according to your application's error handling strategy
            }
        }
    }
}
