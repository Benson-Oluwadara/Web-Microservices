using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Mango.Services.CouponAPI.Database.IDapperRepositorys;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.DTO;
using Mango.Services.CouponAPI.Repository.IRepository;

namespace Mango.Services.CouponAPI.Repository.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly IDapperRepository _dapperRepository;

        public CouponRepository(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        public async Task<int> CreateCouponAsync(CreateCouponDTO createCouponDto)
        {
            var couponEntity = MapToCouponEntity(createCouponDto);
            //var sql = "INSERT INTO Coupons (CouponCode, DiscountAmount, MinAmount) VALUES (@CouponCode, @DiscountAmount, @MinAmount)";
            var sql=@"INSERT INTO Coupons (CouponCode, DiscountAmount, MinAmount, LastUpdateDate) 
                              VALUES (@CouponCode, @DiscountAmount, @MinAmount, GETDATE());
                              SELECT CAST(SCOPE_IDENTITY() as int)";
            return await _dapperRepository.ExecuteAsync(sql, couponEntity);
        }

        public async Task<bool> DeleteCouponAsync(int couponId)
        {
            var sql = "DELETE FROM Coupons WHERE CouponId = @CouponId";
            var rowsAffected = await _dapperRepository.ExecuteAsync(sql, new { CouponId = couponId });
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Coupon>> GetAllCouponAsync()
        {
            var sql = "SELECT * FROM Coupons";
            return await _dapperRepository.GetAllAsync<Coupon>(sql);
        }

        public async Task<Coupon> GetCouponByIdAsync(int couponId)
        {
            var sql = "SELECT * FROM Coupons WHERE CouponId = @CouponId";
            return await _dapperRepository.GetAsync<Coupon>(sql, new { CouponId = couponId });
        }

        public async Task<bool> UpdateCouponAsync(UpdateCouponDTO updateCouponDto)
        {
            var existingCoupon = await GetCouponByIdAsync(updateCouponDto.CouponId);

            if (existingCoupon == null)
            {
                return false; // Handle the case where the coupon with the given ID is not found
            }

            MapToCouponEntity(updateCouponDto, existingCoupon);
            var sql = @"UPDATE Coupons SET CouponCode = @CouponCode, DiscountAmount = @DiscountAmount, MinAmount = @MinAmount,         
                      LastUpdateDate = GETDATE()
                      WHERE CouponId = @CouponId";
            var rowsAffected = await _dapperRepository.ExecuteAsync(sql, existingCoupon);

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateCouponAsync(int couponId, UpdateCouponDTO updateCouponDto)
        {
            var existingCoupon = await GetCouponByIdAsync(couponId);

            if (existingCoupon == null)
            {
                return false; // Handle the case where the coupon with the given ID is not found
            }

            MapToCouponEntity(updateCouponDto, existingCoupon);
            var sql = @"UPDATE Coupons SET CouponCode = @CouponCode, DiscountAmount = @DiscountAmount, MinAmount = @MinAmount,         
                      LastUpdateDate = GETDATE()
                      WHERE CouponId = @CouponId";
            var rowsAffected = await _dapperRepository.ExecuteAsync(sql, existingCoupon);

            return rowsAffected > 0;
        }

        private static Coupon MapToCouponEntity(CreateCouponDTO createCouponDto)
        {
            return new Coupon
            {
                CouponCode = createCouponDto.CouponCode,
                DiscountAmount = createCouponDto.DiscountAmount,
                MinAmount = createCouponDto.MinAmount
            };
        }

        private static void MapToCouponEntity(UpdateCouponDTO updateCouponDto, Coupon existingCoupon)
        {
            existingCoupon.CouponCode = updateCouponDto.CouponCode;
            existingCoupon.DiscountAmount = updateCouponDto.DiscountAmount;
            existingCoupon.MinAmount = updateCouponDto.MinAmount;
        }
    }
}
