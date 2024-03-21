using AutoMapper;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.DTO;
using Mango.Services.CouponAPI.Repository.IRepository;
using Mango.Services.CouponAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Mango.Services.CouponAPI.Controllers
{
    // CouponController.cs
    [ApiController]
    [Route("api/coupons")]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpGet]
        
        public async Task<IActionResult> GetAllCoupons()
        {
            var coupons = await _couponService.GetAllCouponsAsync();
            return Ok(new APIResponse(HttpStatusCode.OK, true, coupons));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCouponById(int id)
        {
            var coupon = await _couponService.GetCouponByIdAsync(id);

            if (coupon == null)
            {
                return NotFound(new APIResponse(HttpStatusCode.NotFound, false, null, new List<string> { "Coupon not found." }));
            }

            return Ok(new APIResponse(HttpStatusCode.OK, true, coupon));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponDTO createCouponDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new APIResponse(HttpStatusCode.BadRequest, false, null, GetModelStateErrorMessages()));
            }

            var createdCoupon = await _couponService.CreateCouponAsync(createCouponDto);

            return CreatedAtAction(nameof(GetCouponById), new { id = createdCoupon.CouponId }, new APIResponse(HttpStatusCode.Created, true, createdCoupon));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCoupon(int id, [FromBody] UpdateCouponDTO updateCouponDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new APIResponse(HttpStatusCode.BadRequest, false, null, GetModelStateErrorMessages()));
            }

            var isUpdated = await _couponService.UpdateCouponAsync(id, updateCouponDto);

            if (!isUpdated)
            {
                return NotFound(new APIResponse(HttpStatusCode.NotFound, false, null, new List<string> { "Coupon not found." }));
            }

            return Ok(new APIResponse(HttpStatusCode.OK, true, null));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            var isDeleted = await _couponService.DeleteCouponAsync(id);

            if (!isDeleted)
            {
                return NotFound(new APIResponse(HttpStatusCode.NotFound, false, null, new List<string> { "Coupon not found." }));
            }

            return Ok(new APIResponse(HttpStatusCode.OK, true, null));
        }

        private List<string> GetModelStateErrorMessages()
        {
            return ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
        }
    }


}
