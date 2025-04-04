using System.ComponentModel.DataAnnotations;

namespace WebAPP.Models
{
    public class PropertyAmenityVM
    {
        public int PropertyAmenityId { get; set; }

        [Display(Name = "Property")]
        [Required(ErrorMessage = "Property is required")]
        public int? PropertyId { get; set; }

        [Display(Name = "Property")]
        public string? PropertyName { get; set; }

        [Display(Name = "Amenity")]
        [Required(ErrorMessage = "Amenity is required")]
        public int? AmenityId { get; set; }
        
        [Display(Name = "Amenity")]
        public string? AmenityName { get; set; }
    }
}
