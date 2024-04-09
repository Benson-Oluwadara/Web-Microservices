using AutoMapper;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.DTO;
using Mango.Services.CouponAPI.Repository.IRepository;
using Mango.Services.CouponAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
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

        //[HttpGet]

        //public async Task<IActionResult> GetAllCoupons()
        //{
        //    var coupons = await _couponService.GetAllCouponsAsync();
        //    return Ok(new APIResponse(HttpStatusCode.OK, true, coupons));
        //}
        [HttpGet]
        public async Task<IActionResult> GetAllCoupons()
        {
            try
            {

                var coupons = await _couponService.GetAllCouponsAsync();
                Log.Information("Returning all coupons");
                return Ok(new APIResponse(HttpStatusCode.OK, true, coupons));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while getting all coupons");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while getting all coupons");
            }
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetCouponById(int id)
        //{
        //    var coupon = await _couponService.GetCouponByIdAsync(id);

        //    if (coupon == null)
        //    {
        //        return NotFound(new APIResponse(HttpStatusCode.NotFound, false, null, new List<string> { "Coupon not found." }));
        //    }

        //    return Ok(new APIResponse(HttpStatusCode.OK, true, coupon));
        //}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCouponById(int id)
        {
            try
            {
                var coupon = await _couponService.GetCouponByIdAsync(id);

                if (coupon == null)
                {
                    Log.Warning("Coupon with id {Id} not found", id);
                    return NotFound(new APIResponse(HttpStatusCode.NotFound, false, null, new List<string> { "Coupon not found." }));
                }
                Log.Information("Returning coupon with id {Id}", id);
                return Ok(new APIResponse(HttpStatusCode.OK, true, coupon));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while getting the coupon with id {Id}", id);
                return StatusCode((int)HttpStatusCode.InternalServerError, $"An error occurred while getting the coupon with id {id}");
            }
        }
        //[HttpPost]
        //public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponDTO createCouponDto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(new APIResponse(HttpStatusCode.BadRequest, false, null, GetModelStateErrorMessages()));
        //    }

        //    var createdCoupon = await _couponService.CreateCouponAsync(createCouponDto);

        //    return CreatedAtAction(nameof(GetCouponById), new { id = createdCoupon.CouponId }, new APIResponse(HttpStatusCode.Created, true, createdCoupon));
        //}

        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateCoupon(int id, [FromBody] UpdateCouponDTO updateCouponDto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(new APIResponse(HttpStatusCode.BadRequest, false, null, GetModelStateErrorMessages()));
        //    }

        //    var isUpdated = await _couponService.UpdateCouponAsync(id, updateCouponDto);

        //    if (!isUpdated)
        //    {
        //        return NotFound(new APIResponse(HttpStatusCode.NotFound, false, null, new List<string> { "Coupon not found." }));
        //    }

        //    return Ok(new APIResponse(HttpStatusCode.OK, true, null));
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteCoupon(int id)
        //{
        //    var isDeleted = await _couponService.DeleteCouponAsync(id);

        //    if (!isDeleted)
        //    {
        //        return NotFound(new APIResponse(HttpStatusCode.NotFound, false, null, new List<string> { "Coupon not found." }));
        //    }

        //    return Ok(new APIResponse(HttpStatusCode.OK, true, null));
        //}

        [HttpPost]
        public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponDTO createCouponDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new APIResponse(HttpStatusCode.BadRequest, false, null, GetModelStateErrorMessages()));
                }

                var createdCoupon = await _couponService.CreateCouponAsync(createCouponDto);
                Log.Information("Coupon created with id {Id}", createdCoupon.CouponId);
                return CreatedAtAction(nameof(GetCouponById), new { id = createdCoupon.CouponId }, new APIResponse(HttpStatusCode.Created, true, createdCoupon));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while creating the coupon");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while creating the coupon");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCoupon(int id, [FromBody] UpdateCouponDTO updateCouponDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new APIResponse(HttpStatusCode.BadRequest, false, null, GetModelStateErrorMessages()));
                }

                var isUpdated = await _couponService.UpdateCouponAsync(id, updateCouponDto);

                if (!isUpdated)
                {
                    Log.Warning("Coupon with id {Id} not found", id);
                    return NotFound(new APIResponse(HttpStatusCode.NotFound, false, null, new List<string> { "Coupon not found." }));
                }
                Log.Information("Coupon with id {Id} updated", id);
                return Ok(new APIResponse(HttpStatusCode.OK, true, null));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while updating the coupon with id {Id}", id);
                return StatusCode((int)HttpStatusCode.InternalServerError, $"An error occurred while updating the coupon with id {id}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            try
            {
                var isDeleted = await _couponService.DeleteCouponAsync(id);

                if (!isDeleted)
                {
                    Log.Warning("Coupon with id {Id} not found", id);
                    return NotFound(new APIResponse(HttpStatusCode.NotFound, false, null, new List<string> { "Coupon not found." }));
                }
                Log.Information("Coupon with id {Id} deleted", id);
                return Ok(new APIResponse(HttpStatusCode.OK, true, null));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while deleting the coupon with id {Id}", id);
                return StatusCode((int)HttpStatusCode.InternalServerError, $"An error occurred while deleting the coupon with id {id}");
            }
        }

        private List<string> GetModelStateErrorMessages()
        {
           Log.Warning("Model state is invalid");
            return ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
        }
    }


}
