using System.ComponentModel.DataAnnotations;

namespace SchoolSupplies.Models
{
    public class AddCategoryViewModel
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(50, ErrorMessage = "Category name cannot be longer than 50 characters")]
        public string CategoryName { get; set; }
        [Required(ErrorMessage = "Category image is required")]
        public string CategoryImage { get; set; }
        [StringLength(200, ErrorMessage = "Category description cannot be longer than 200 characters")]
        public string CategoryDescription { get; set; }

    }
}
