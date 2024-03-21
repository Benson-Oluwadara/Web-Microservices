using System.ComponentModel.DataAnnotations;
namespace Mango.Services.CouponAPI.Models.DTO
{
    public class CreateCouponDTO
    {

        [Required]
        public string CouponCode { get; set; }
        [Required]
        public double DiscountAmount { get; set; }
        [Required]
        public int MinAmount { get; set; }
    }
}
