namespace Mango.Services.ProductAPI.Models.DTO
{
    public class ProductCreateDTO
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string ImageUrl { get; set; }
        //public IFormFile Image { get; set; }
    }
}
