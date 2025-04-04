using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace WebAPP.Models
{
    public class ReservationVM
    {
        public int ReservationID { get; set; }
        [Display(Name = "Property")]
        [Required(ErrorMessage = "Property is required")]
        public int PropertyID { get; set; }
        [Display(Name = "Property")]
        public string PropertyName { get; set; }
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }
        [Display(Name = "Check In")]
        [Required(ErrorMessage = "Check In is required")]
        public DateTime? CheckIn { get; set; }
        [Display(Name = "Check Out")]
        [Required(ErrorMessage = "Check Out is required")]
        public DateTime? CheckOut { get; set; }
        [Display(Name = "Total Price")]
        public int? TotalPrice { get; set; }
        [Display(Name = "Number of Days")]
        public int NumberOfDays { get; set; }
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
