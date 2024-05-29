using System.ComponentModel.DataAnnotations;

namespace SchoolSupplies.Models
{
    public class AddBrandViewModel
    {
        [Required(ErrorMessage = "Brand name is required")]
        [StringLength(50, ErrorMessage = "Brand name cannot be longer than 50 characters")]
        public string BrandName { get; set; }
        [StringLength(200, ErrorMessage = "Brand description cannot be longer than 200 characters")]
        public string BrandDescription { get; set; }
    }
}
