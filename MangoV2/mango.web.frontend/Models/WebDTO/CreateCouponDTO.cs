namespace mango.web.frontend.Models.WebDTO
{
    public class CreateCouponDTO
    {
        public string CouponCode { get; set; }
        public double DiscountAmount { get; set; }
        public int MinAmount { get; set; }
    }
}
