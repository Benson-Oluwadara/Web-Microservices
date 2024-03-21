using Mango.Services.Shopping_CartAPI.Database.IDapperRepositorys;
using Mango.Services.Shopping_CartAPI.Models.DTO;
using Mango.Services.Shopping_CartAPI.Repository.IRepository;

namespace Mango.Services.Shopping_CartAPI.Repository.Repository
{
    public class CartRepository: ICartRepository
    {
        private readonly IDapperRepository _dapperRepository;
        public CartRepository(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }
        public async Task<CartDTO> GetCartByUserIdAsync(string userId)
        {
            var cart = await _dapperRepository.GetAsync<CartDTO>("SELECT * FROM CartHeaders WHERE UserId = @UserId", new { UserId = userId });

            if (cart != null)
            {
                cart.CartDetails = await _dapperRepository.GetAllAsync<CartDetailsDTO>("SELECT * FROM CartDetails WHERE CartHeaderId = @CartHeaderId", new { CartHeaderId = cart.CartHeader.CartHeaderId });
            }

            return cart;
        }
        public async Task<bool> UpsertCartAsync(CartDTO cartDto)
        {
            try
            {
                var cartHeaderFromDb = await GetCartByUserIdAsync(cartDto.CartHeader.UserId);

                if (cartHeaderFromDb == null)
                {
                    // Create header and details
                    await _dapperRepository.ExecuteAsync("INSERT INTO CartHeaders (UserId) VALUES (@UserId)", new { UserId = cartDto.CartHeader.UserId });

                    cartDto.CartDetails.First().CartHeaderId = cartDto.CartHeader.CartHeaderId;
                    await _dapperRepository.ExecuteAsync("INSERT INTO CartDetails (CartHeaderId, ProductId, Count) VALUES (@CartHeaderId, @ProductId, @Count)", cartDto.CartDetails.First());
                }
                else
                {
                    var cartDetailsFromDb = await _dapperRepository.GetAsync<CartDetailsDTO>("SELECT * FROM CartDetails WHERE ProductId = @ProductId AND CartHeaderId = @CartHeaderId",
                        new { ProductId = cartDto.CartDetails.First().ProductId, CartHeaderId = cartHeaderFromDb.CartHeader.CartHeaderId });

                    if (cartDetailsFromDb == null)
                    {
                        // Create cart details
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeader.CartHeaderId;
                        await _dapperRepository.ExecuteAsync("INSERT INTO CartDetails (CartHeaderId, ProductId, Count) VALUES (@CartHeaderId, @ProductId, @Count)", cartDto.CartDetails.First());
                    }
                    else
                    {
                        // Update count in cart details
                        cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                        await _dapperRepository.ExecuteAsync("UPDATE CartDetails SET Count = @Count WHERE CartDetailsId = @CartDetailsId", cartDto.CartDetails.First());
                    }
                }

                return true;
            }
            catch (AggregateException ex)
            {
                foreach (var innerException in ex.InnerExceptions)
                {
                    Console.WriteLine($"Inner Exception: {innerException.Message}");
                }
                return false;
            }
            catch
            {
                return false;
            }
          
        }

        public async Task<bool> RemoveCartAsync(int cartDetailsId)
        {
            try
            {
                var cartDetails = await _dapperRepository.GetAsync<CartDetailsDTO>("SELECT * FROM CartDetails WHERE CartDetailsId = @CartDetailsId", new { CartDetailsId = cartDetailsId });

                if (cartDetails != null)
                {
                    await _dapperRepository.ExecuteAsync("DELETE FROM CartDetails WHERE CartDetailsId = @CartDetailsId", new { CartDetailsId = cartDetailsId });

                    var totalCountofCartItem = await _dapperRepository.GetAsync<int>("SELECT COUNT(*) FROM CartDetails WHERE CartHeaderId = @CartHeaderId", new { CartHeaderId = cartDetails.CartHeaderId });

                    if (totalCountofCartItem == 1)
                    {
                        await _dapperRepository.ExecuteAsync("DELETE FROM CartHeaders WHERE CartHeaderId = @CartHeaderId", new { CartHeaderId = cartDetails.CartHeaderId });
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
