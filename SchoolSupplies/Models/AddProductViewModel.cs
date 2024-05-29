using SchoolSupplies.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace SchoolSupplies.Models
{
    public class AddProductViewModel
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(50, ErrorMessage = "Product name cannot be longer than 50 characters")]
        public string ProductName { get; set; }
        [StringLength(200, ErrorMessage = "Product description cannot be longer than 200 characters")]
        public string ProductDescription { get; set; }
        [Required(ErrorMessage = "Please enter the product price")]
        [Range(0, double.MaxValue, ErrorMessage = "Product price must be a non-negative number")]
        public double ProductPrice { get; set; }
        [Range(0, 100, ErrorMessage = "Product discount must be between 0 and 100")]
        public int ProductDiscount { get; set; }
        public string ProductUrlImageMain { get; set; }
        [Required(ErrorMessage = "Please select a category")]
        public Guid CategoryId { get; set; }
        [Required(ErrorMessage = "Please select a brand")]
        public Guid BrandId { get; set; }
        public virtual Category? Category { get; set; }
        public virtual Brand? Brand { get; set; }
        public bool ProductHot { get; set; }
        public bool ProductLike { get; set; }
    }
}
