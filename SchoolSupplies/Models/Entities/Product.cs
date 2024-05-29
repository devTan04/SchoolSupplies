using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolSupplies.Models.Entities
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ProductId { get; set; }
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(50, ErrorMessage = "Product name cannot be longer than 50 characters")]
        public string ProductName { get; set; }
        public string ProductUrlImageMain { get; set; }

        [StringLength(200, ErrorMessage = "Product description cannot be longer than 200 characters")]
        public string ProductDescription { get; set; }
        [Required(ErrorMessage = "Please enter the product price")]
        [Range(0, double.MaxValue, ErrorMessage = "Product price must be a non-negative number")]
        public double ProductPrice { get; set; }
        [Range(0, 100, ErrorMessage = "Product discount must be between 0 and 100")]
        public int ProductDiscount { get; set; }
        public bool ProductHot { get; set; }
        public bool ProductLike { get; set; }
        public Guid CategoryId { get; set; }
        public Guid BrandId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }
        [ForeignKey("BrandId")]
        public virtual Brand? Brand { get; set; }
        public virtual ICollection<Image> Images { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
