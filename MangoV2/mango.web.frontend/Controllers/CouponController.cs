using mango.web.frontend.Models;
using mango.web.frontend.Models.VM;
using mango.web.frontend.Models.WebDTO;
using mango.web.frontend.Services.Iservices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace mango.web.frontend.Controllers
{
    public class CouponController : Controller
    {
        private readonly ILogger<CouponController> _logger;
        private readonly ICouponService _couponService;

        //constructor for the controller
        public CouponController(ILogger<CouponController> logger, ICouponService couponService)
        {
            _logger = logger;
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
            Console.WriteLine("Coupon Index Method!!!!!!!!!!!!!!!!!!!!!!!!!!\n!!!!!!!!!!!!!!!!!!!!!");

            List<CouponViewModel> list = new List<CouponViewModel>();
            var response = await _couponService.GetAllCouponAsync<WebAPIResponse>();
            _logger.LogInformation($"Response is: {JsonConvert.SerializeObject(response)}");
            Console.WriteLine($"Response Content: {JsonConvert.SerializeObject(response)}");

            if (response != null && response.IsSuccess)
            {
                // Deserialize the JSON array into a List<CouponViewModel>
                list = JsonConvert.DeserializeObject<List<CouponViewModel>>(response.Result.ToString()) ?? new List<CouponViewModel>();

                foreach (var coupon in list)
                {
                    Console.WriteLine($"Coupon Code: {coupon.CouponCode}, Min Amount: {coupon.MinAmount}");
                }
            }
            return View(list);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCoupon(CreateCouponDTO model)
        {
            // ... (your existing code)

            if (ModelState.IsValid)
            {
                var response = await _couponService.CreateCouponAsync<WebAPIResponse>(model);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Coupon Created Successfully";
                    return RedirectToAction(nameof(CouponIndex));
                }
                TempData["error"] = "Error Encountered";
            }

            // Check if the view name matches the actual view file name
            return View("CreateCoupon", model);
        }



        public async Task<IActionResult> CreateCoupon()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCoupon(int couponId)
        {
            Console.WriteLine($"Update Coupon Method for CouponId: {couponId} !!!!!!!!!!!!!!!!!!!!!!!!!!");

            var response = await _couponService.GetCouponByIdAsync<WebAPIResponse>(couponId);
            _logger.LogInformation($"Response is: {JsonConvert.SerializeObject(response)}");

            if (response != null && response.IsSuccess)
            {
                var coupon = JsonConvert.DeserializeObject<CouponViewModel>(response.Result.ToString());
                Console.WriteLine($"Coupon Code: {coupon.CouponCode}, Min Amount: {coupon.MinAmount}");

                // Assuming you have an UpdateCouponDTO model to bind to the form
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCoupon(UpdateCouponDTO model)
        {
            Console.WriteLine("Update Coupon!!!!!!!!!!!!!!!!!!!!!!!!!!\n!!!!!!!!!!!!!!!!!!!!!");

            if (ModelState.IsValid)
            {
                var response = await _couponService.UpdateCouponAsync<WebAPIResponse>(model);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Coupon Updated Successfully";
                    return RedirectToAction(nameof(CouponIndex));
                }

                TempData["error"] = "Error Encountered";
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCoupon(int couponId)
        {
            Console.WriteLine($"Delete Coupon Method for CouponId: {couponId} !!!!!!!!!!!!!!!!!!!!!!!!!!");

            var response = await _couponService.GetCouponByIdAsync<WebAPIResponse>(couponId);

            if (response != null && response.IsSuccess)
            {
                var coupon = JsonConvert.DeserializeObject<CouponViewModel>(response.Result.ToString());

                // Assuming you have a DeleteCouponDTO model to bind to the form
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCoupon(DeleteCouponDTO model)
        {
            Console.WriteLine("Delete Coupon!!!!!!!!!!!!!!!!!!!!!!!!!!\n!!!!!!!!!!!!!!!!!!!!!");

            if (ModelState.IsValid)
            {
                var response = await _couponService.DeleteCouponAsync<WebAPIResponse>(model.CouponId);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Coupon Deleted Successfully";
                    return RedirectToAction(nameof(CouponIndex));
                }

                TempData["error"] = "Error Encountered";
            }

            return View(model);
        }

    }
}
