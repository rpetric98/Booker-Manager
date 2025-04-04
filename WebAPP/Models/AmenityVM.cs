using System.ComponentModel.DataAnnotations;

namespace WebAPP.Models
{
    public class AmenityVM
    {
        [Display(Name = "Amenity")]
        [Required(ErrorMessage = "Amenity is required")]
        public int AmenityId { get; set; }

        [Display(Name = "Amenity")]
        [Required(ErrorMessage = "Amenity is required")]
        public string? AmenityName { get; set; }
    }
}
