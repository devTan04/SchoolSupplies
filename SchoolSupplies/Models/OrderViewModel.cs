using System.ComponentModel.DataAnnotations;

namespace SchoolSupplies.Models
{
    public class OrderViewModel
    {
        public List<CartItem> CartItems { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(100, ErrorMessage = "First Name can't be longer than 100 characters")]
        public string OrderFirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(100, ErrorMessage = "Last Name can't be longer than 100 characters")]
        public string OrderLastName { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string OrderPhoneNumber { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(500, ErrorMessage = "Address can't be longer than 500 characters")]
        public string OrderAddress { get; set; }
    }
}
