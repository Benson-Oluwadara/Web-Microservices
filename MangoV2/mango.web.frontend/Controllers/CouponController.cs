using mango.web.frontend.Models;
using mango.web.frontend.Models.VM;
using mango.web.frontend.Models.WebDTO;
using mango.web.frontend.Services.Iservices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
namespace mango.web.frontend.Controllers
{
    public class CouponController : Controller
    {

        private readonly ICouponService _couponService;

        //constructor for the controller
        public CouponController(ICouponService couponService)
        {

            _couponService = couponService;
        }

        // GET
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> CouponIndex()
        {
            try
            {
                Log.Information("Accessing Coupon Index page.");

                List<CouponViewModel> list = new List<CouponViewModel>();
                var response = await _couponService.GetAllCouponAsync<WebAPIResponse>();
                Log.Information("Response is: {@Response}", response);
                //Console.WriteLine($"Response Content: {JsonConvert.SerializeObject(response)}");

                if (response != null && response.IsSuccess)
                {
                    list = JsonConvert.DeserializeObject<List<CouponViewModel>>(response.Result.ToString()) ?? new List<CouponViewModel>();

                    foreach (var coupon in list)
                    {
                        Log.Information("Coupon Code: {CouponCode}, Min Amount: {MinAmount}", coupon.CouponCode, coupon.MinAmount);
                    }
                }

                return View(list);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while accessing the Coupon Index page.");
                throw; // You may handle the exception according to your application's error handling strategy
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCoupon(CreateCouponDTO model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _couponService.CreateCouponAsync<WebAPIResponse>(model);
                    if (response != null && response.IsSuccess)
                    {
                        Log.Information("Coupon Created Successfully");
                        TempData["success"] = "Coupon Created Successfully";
                        return RedirectToAction(nameof(CouponIndex));
                    }
                    TempData["error"] = "Error Encountered";
                }

                return View("CreateCoupon", model);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while creating a coupon.");
                throw; // You may handle the exception according to your application's error handling strategy
            }
        }



        public async Task<IActionResult> CreateCoupon()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCoupon(int couponId)
        {
            try
            {
                Log.Information("Update Coupon Method for CouponId: {CouponId}", couponId);

                var response = await _couponService.GetCouponByIdAsync<WebAPIResponse>(couponId);
                Log.Information("Response is: {@Response}", response);

                if (response != null && response.IsSuccess)
                {
                    var coupon = JsonConvert.DeserializeObject<CouponViewModel>(response.Result.ToString());
                    Log.Information("Coupon Code: {CouponCode}, Min Amount: {MinAmount}", coupon.CouponCode, coupon.MinAmount);

                    var updateCouponDto = new UpdateCouponDTO
                    {
                        CouponId = coupon.CouponId,
                        CouponCode = coupon.CouponCode,
                        DiscountAmount = coupon.DiscountAmount,
                        MinAmount = coupon.MinAmount
                    };

                    return View(updateCouponDto);
                }

                TempData["error"] = "Coupon not found.";
                return RedirectToAction(nameof(CouponIndex));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while updating the coupon for CouponId: {CouponId}", couponId);
                throw; // You may handle the exception according to your application's error handling strategy
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCoupon(UpdateCouponDTO model)
        {
            try
            {
                Log.Information("Update Coupon");

                if (ModelState.IsValid)
                {
                    var response = await _couponService.UpdateCouponAsync<WebAPIResponse>(model);
                    if (response != null && response.IsSuccess)
                    {
                        Log.Information("Coupon Updated Successfully");
                        TempData["success"] = "Coupon Updated Successfully";
                        return RedirectToAction(nameof(CouponIndex));
                    }

                    TempData["error"] = "Error Encountered";
                }

                return View(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while updating the coupon.");
                throw; // You may handle the exception according to your application's error handling strategy
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCoupon(int couponId)
        {
            try
            {
                Log.Information("Delete Coupon Method for CouponId: {CouponId}", couponId);

                var response = await _couponService.GetCouponByIdAsync<WebAPIResponse>(couponId);

                if (response != null && response.IsSuccess)
                {
                    var coupon = JsonConvert.DeserializeObject<CouponViewModel>(response.Result.ToString());
                    var deleteCouponDto = new DeleteCouponDTO
                    {
                        CouponId = coupon.CouponId,
                        CouponCode = coupon.CouponCode,
                        DiscountAmount = coupon.DiscountAmount,
                        MinAmount = coupon.MinAmount
                    };

                    return View(deleteCouponDto);
                }

                TempData["error"] = "Coupon not found.";
                return RedirectToAction(nameof(CouponIndex));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while deleting the coupon for CouponId: {CouponId}", couponId);
                throw; // You may handle the exception according to your application's error handling strategy
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCoupon(DeleteCouponDTO model)
        {
            try
            {
                Log.Information("Delete Coupon");

                if (ModelState.IsValid)
                {
                    var response = await _couponService.DeleteCouponAsync<WebAPIResponse>(model.CouponId);
                    if (response != null && response.IsSuccess)
                    {
                        Log.Information("Coupon Deleted Successfully");
                        TempData["success"] = "Coupon Deleted Successfully";
                        return RedirectToAction(nameof(CouponIndex));
                    }

                    TempData["error"] = "Error Encountered";
                }

                return View(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while deleting the coupon.");
                throw; // You may handle the exception according to your application's error handling strategy
            }
        }
    }
    }
