using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace WebAPP.Models
{
    public class PropertyVM
    {
        public int PropertyId { get; set; }

        [Display(Name = "Property Type")]
        [Required(ErrorMessage = "Property Type is required")]
        public int? PropertyTypeId { get; set; }

        [Display(Name = "Property Type")]
        public string? PropertyTypeName { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string? City { get; set; }

        [Required(ErrorMessage = "State is required")]
        public string? ZipCode { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public string? Country { get; set; }

        [Required(ErrorMessage = "Price per night is required")]
        public int? PricePerNight { get; set; }

        [Required(ErrorMessage = "Number of guests is required")]
        public int? MaxGuests { get; set; }
        [ValidateNever]
        public List<int>? AmenityIds { get; set; }
        [ValidateNever]
        public List<AmenityVM>? Amenities { get; set; }
        [ValidateNever]
        public int Page { get; set; } = 1;
        [ValidateNever]
        public int Size { get; set; } = 10;
        [ValidateNever]
        public int FromPager { get; set; }
        [ValidateNever]
        public int ToPager { get; set; }
        [ValidateNever]
        public int LastPage { get; set; }
    }
}
