using System.ComponentModel.DataAnnotations;

namespace SchoolSupplies.Models
{
    public class ForgotPassword
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string UserEmail { get; set; }
    }
}
