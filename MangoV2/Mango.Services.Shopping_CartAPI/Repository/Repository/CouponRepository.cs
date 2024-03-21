using Mango.Services.Shopping_CartAPI.Database.IDapperRepositorys;
using Mango.Services.Shopping_CartAPI.Models.DTO;
using Mango.Services.Shopping_CartAPI.Repository.IRepository;

namespace Mango.Services.Shopping_CartAPI.Repository.Repository
{
    public class CouponRepository:ICouponRepository
    {
        private readonly IDapperRepository _dapperRepository;
        public CouponRepository(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        } 
        public async Task<CouponDTO> GetCouponAsync(string couponCode)
        {
            var sql = "SELECT * FROM Coupons WHERE CouponCode = @CouponCode";
            var parameters = new { CouponCode = couponCode };
            return await _dapperRepository.GetAsync<CouponDTO>(sql, parameters);
        }



    }
}
