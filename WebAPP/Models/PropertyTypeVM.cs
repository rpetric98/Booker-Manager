using System.ComponentModel.DataAnnotations;

namespace WebAPP.Models
{
    public class PropertyTypeVM
    {
        [Display(Name = "Property type")]
        [Required(ErrorMessage = "Property type is required")]
        public int PropertyTypeId { get; set; }
        [Display(Name ="Property type")]
        [Required(ErrorMessage = "Property type is required")]
        public string? PropertyTypeName { get;set; }
    }
}
