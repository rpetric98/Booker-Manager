using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dtos
{
    public class PropertyDto
    {
        public int PropertyId { get; set; }
        [Required(ErrorMessage = "Please enter something")]
        public string? PropertyTypeName { get; set; }

        [Required(ErrorMessage = "Please enter something")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Please enter something")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Please enter something")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Please enter something")]
        public string? City { get; set; }

        [Required(ErrorMessage = "Please enter something")]
        public string? ZipCode { get; set; }

        [Required(ErrorMessage = "Please enter something")]
        public string? Country { get; set; }

        [Required(ErrorMessage = "Please enter something")]
        public int? PricePerNight { get; set; }

        [Required(ErrorMessage = "Please enter something")]
        public int? MaxGuests { get; set; }
    }
}
