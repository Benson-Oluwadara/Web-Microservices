using System.ComponentModel.DataAnnotations;

namespace mango.web.frontend.Models.WebDTO
{
    public class LoginRequestDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
