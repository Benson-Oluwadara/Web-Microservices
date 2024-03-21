using Mango.Services.ShoppingCartAPI.Database.IDapperRepositorys;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTO;
using Mango.Services.ShoppingCartAPI.Repository.IRepository;

namespace Mango.Services.ShoppingCartAPI.Repository.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly IDapperRepository _dapperRepository;
        public CartRepository(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }
        //Task<double> ICartRepository.CalculateCouponDiscountAsync(string couponCode, double cartTotal)
        //{
        //    string CouponQuery = @"SELECT TOP (1) [CouponId]
        //                              ,[CouponCode]
        //                              ,[DiscountAmount]
        //                              ,[MinAmount]
        //                              ,[LastUpdateDate]
        //                           FROM [MangoDatabase].[dbo].[Coupons]
        //                           WHERE CouponCode = @CouponCode";
        //    var coupon = _dapperRepository.GetAsync<CouponDTO>(CouponQuery, new { CouponCode = couponCode });
        //    if (coupon != null)
        //    {
        //        // Check if the cart total meets the minimum amount requirement
        //        if (cartTotal >= coupon.Result.MinAmount)
        //        {
        //            // Apply the discount
        //            return coupon.Result.DiscountAmount;
        //        }
        //    }

        //    // No discount applied
        //    return 0;
        //}

        public async Task<double> CalculateCouponDiscountAsync(string couponCode, double cartTotal)
        {
            string couponQuery = @"SELECT TOP (1) [CouponId]
                                ,[CouponCode]
                                ,[DiscountAmount]
                                ,[MinAmount]
                                ,[LastUpdateDate]
                            FROM [MangoDatabase].[dbo].[Coupons]
                            WHERE CouponCode = @CouponCode";

            var coupon = await _dapperRepository.GetAsync<CouponDTO>(couponQuery, new { CouponCode = couponCode });

            if (coupon != null)
            {
                // Check if the cart total meets the minimum amount requirement
                if (cartTotal >= coupon.MinAmount)
                {
                    // Apply the discount
                    return coupon.DiscountAmount;
                }
            }

            // No discount applied
            return 0;
        }

        //private async Task<double> CalculateCouponDiscountAsync(string couponCode, double cartTotal)
        //{

        //}
        public async Task<CartDTO> GetCartByUserIdAsync(string userId)
        {
            try
            {
                // Troubleshooting Step 1: Trim whitespaces
                var trimmedUserId = userId.Trim();

                // Troubleshooting Step 2: Explicitly specify columns
                var cartHeader = await _dapperRepository.GetAsync<CartHeaderDTO>(
    "SELECT CartHeaderId, UserId, CouponCode FROM CartHeaders WHERE UserId = @UserId",
    new { UserId = trimmedUserId });

                if (cartHeader != null)
                {
                    // CartHeader found, proceed to retrieve CartDetails
                    var cartDetails = await _dapperRepository.GetAllAsync<CartDetailsDTO>(
                        "SELECT CartDetailsId, CartHeaderId, ProductId, Count FROM CartDetails WHERE CartHeaderId = @CartHeaderId",
                        new { CartHeaderId = cartHeader.CartHeaderId });

                    // Create a CartDTO with retrieved CartHeader and CartDetails
                    var cartDTO = new CartDTO
                    {
                        CartHeader = cartHeader,
                        CartDetails = cartDetails
                    };

                    return cartDTO;
                }
                else
                {
                    // Handle the case where CartHeader is not found
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"Exception in GetCartByUserIdAsync: {ex.Message}");
                throw; // Re-throw the exception to propagate it to the caller
            }
        }





        public async Task<bool> RemoveCartAsync(int cartDetailsId)
        {
            try
            {
                // Retrieve cart details by ID
                var cartDetailsFromDb = await _dapperRepository.GetAsync<CartDetailsDTO>(
                    "SELECT * FROM CartDetails WHERE CartDetailsId = @CartDetailsId",
                    new { CartDetailsId = cartDetailsId });

                if (cartDetailsFromDb != null)
                {
                    // Remove the cart details
                    await _dapperRepository.ExecuteAsync(
                        "DELETE FROM CartDetails WHERE CartDetailsId = @CartDetailsId",
                        new { CartDetailsId = cartDetailsId });

                    // Optional: Remove the associated cart header if no other details are linked
                    var cartHeaderId = cartDetailsFromDb.CartHeaderId;
                    var remainingDetailsCount = await _dapperRepository.ExecuteScalarAsync<int>(
                        "SELECT COUNT(*) FROM CartDetails WHERE CartHeaderId = @CartHeaderId",
                        new { CartHeaderId = cartHeaderId });

                    if (remainingDetailsCount == 0)
                    {
                        await _dapperRepository.ExecuteAsync(
                            "DELETE FROM CartHeaders WHERE CartHeaderId = @CartHeaderId",
                            new { CartHeaderId = cartHeaderId });
                    }

                    return true;
                }

                // If cart details with the specified ID do not exist
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in RemoveCartAsync: {ex.Message}");
                // Log the exception here
                return false;
            }
        }

        //public async Task<bool> UpsertCartAsync(CartDTO cartDto)
        //{
        //    try
        //    {
        //        var cartHeaderFromDb = await GetCartByUserIdAsync(cartDto.CartHeader.UserId);

        //        if (cartHeaderFromDb == null)
        //        {
        //            // Create header and details
        //            await _dapperRepository.ExecuteAsync("INSERT INTO CartHeaders (UserId) VALUES (@UserId)", new { UserId = cartDto.CartHeader.UserId });

        //            // Use cartDto.CartHeader.CartHeaderId directly
        //            cartDto.CartDetails.First().CartHeaderId = cartDto.CartHeader.CartHeaderId;
        //            await _dapperRepository.ExecuteAsync("INSERT INTO CartDetails (CartHeaderId, ProductId, Count) VALUES (@CartHeaderId, @ProductId, @Count)", cartDto.CartDetails.First());
        //        }

        //        else
        //        {
        //            var cartDetailsFromDb = await _dapperRepository.GetAsync<CartDetailsDTO>("SELECT * FROM CartDetails WHERE ProductId = @ProductId AND CartHeaderId = @CartHeaderId",
        //                new { ProductId = cartDto.CartDetails.First().ProductId, CartHeaderId = cartHeaderFromDb.CartHeader.CartHeaderId });

        //            if (cartDetailsFromDb == null)
        //            {
        //                // Create cart details
        //                cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeader.CartHeaderId;
        //                await _dapperRepository.ExecuteAsync("INSERT INTO CartDetails (CartHeaderId, ProductId, Count) VALUES (@CartHeaderId, @ProductId, @Count)", cartDto.CartDetails.First());
        //            }
        //            else
        //            {
        //                // Update count in cart details
        //                cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
        //                cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
        //                await _dapperRepository.ExecuteAsync("UPDATE CartDetails SET Count = @Count WHERE CartDetailsId = @CartDetailsId", cartDto.CartDetails.First());
        //            }
        //        }

        //        return true;
        //    }
        //    catch (AggregateException ex)
        //    {
        //        foreach (var innerException in ex.InnerExceptions)
        //        {
        //            Console.WriteLine($"Inner Exception: {innerException.Message}");
        //        }
        //        // Log the exception here or rethrow it if necessary
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Exception in UpsertCartAsync: {ex.Message}");
        //        // Log the exception here
        //        return false;
        //    }
        //}

        public async Task<bool> UpsertCartAsync(CartDTO cartDto)
        {
            try
            {
                Console.WriteLine("Cart header id is:" + cartDto.CartHeader.UserId);
                var cartHeaderFromDb = await GetCartByUserIdAsync(cartDto.CartHeader.UserId);

                Console.WriteLine("Cart Header from DB:");
                Console.WriteLine($"User ID: {cartHeaderFromDb?.CartHeader?.UserId}, CartHeaderId: {cartHeaderFromDb?.CartHeader?.CartHeaderId}");

                if (cartHeaderFromDb == null)
                {
                    // Create header
                    await _dapperRepository.ExecuteAsync("INSERT INTO CartHeaders (UserId) VALUES (@UserId)", new { UserId = cartDto.CartHeader.UserId });

                    // Retrieve the newly created cart header
                    var createdCartHeader = await GetCartByUserIdAsync(cartDto.CartHeader.UserId);

                    if (createdCartHeader != null)
                    {
                        // Create or update cart details
                        await CreateOrUpdateCartDetailsAsync(cartDto, createdCartHeader.CartHeader.CartHeaderId);
                    }
                    else
                    {
                        // Handle the case where creating the cart header was unsuccessful
                        return false;
                    }
                }
                else
                {
                    // Cart header exists, create or update cart details
                    await CreateOrUpdateCartDetailsAsync(cartDto, cartHeaderFromDb.CartHeader.CartHeaderId);
                }

                return true;
            }
            catch (AggregateException ex)
            {
                foreach (var innerException in ex.InnerExceptions)
                {
                    Console.WriteLine($"Inner Exception: {innerException.Message}");
                }
                // Log the exception here or rethrow it if necessary
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in UpsertCartAsync: {ex.Message}");
                // Log the exception here
                return false;
            }
        }

        public async Task<bool> CreateOrUpdateCartDetailsAsync(CartDTO cartDto, int cartHeaderId)
        {
            try
            {
                // Check if the cart details list is not null and contains at least one element
                if (cartDto.CartDetails != null && cartDto.CartDetails.Any())
                {
                    var cartDetailsFromDb = await _dapperRepository.GetAsync<CartDetailsDTO>(
                        "SELECT * FROM CartDetails WHERE ProductId = @ProductId AND CartHeaderId = @CartHeaderId",
                        new { ProductId = cartDto.CartDetails.First().ProductId, CartHeaderId = cartHeaderId });

                    if (cartDetailsFromDb == null)
                    {
                        // Create cart details
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderId;
                        await _dapperRepository.ExecuteAsync("INSERT INTO CartDetails (CartHeaderId, ProductId, Count) VALUES (@CartHeaderId, @ProductId, @Count)", cartDto.CartDetails.First());
                    }
                    else
                    {
                        // Update count in cart details
                        cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                        await _dapperRepository.ExecuteAsync("UPDATE CartDetails SET Count = @Count WHERE CartDetailsId = @CartDetailsId", cartDto.CartDetails.First());
                    }

                    return true;
                }
                else
                {
                    // Handle the case where cartDto.CartDetails is null or empty
                    Console.WriteLine("Cart details list is null or empty.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in CreateOrUpdateCartDetailsAsync: {ex.Message}");
                // Log the exception here
                return false;
            }
        }

    }


}

