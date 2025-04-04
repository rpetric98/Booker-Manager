using System.ComponentModel.DataAnnotations;

namespace WebAPP.Models
{
    public class UserMessageVM
    {
        public int UserMessageId { get; set; }
        [Display(Name = "Message")]
        [Required(ErrorMessage = "Message is required")]
        public string? Message { get; set; }
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; set; }
    }
}
