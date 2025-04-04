using System.ComponentModel.DataAnnotations;

namespace WebAPP.Models
{
    public class UserDetailVM
    {
        public int UserDetailId { get; set; }
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, ErrorMessage = "Username cannot be more than 50 characters")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, MinimumLength =8, ErrorMessage = "Password should be at least 8 characters")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone is required")]
        [Phone(ErrorMessage = "Invalid Phone Number")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, ErrorMessage = "First Name cannot be more than 50 characters")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, ErrorMessage = "Last Name cannot be more than 50 characters")]
        public string LastName { get; set; }

    }
}
