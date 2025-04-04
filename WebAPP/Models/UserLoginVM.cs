using System.ComponentModel.DataAnnotations;

namespace WebAPP.Models
{
    public class UserLoginVM
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(50,MinimumLength = 8, ErrorMessage = "Password should be at least 8 charqacters")]
        public string Password { get; set; }
        public string Url { get; set; }
    }
}
