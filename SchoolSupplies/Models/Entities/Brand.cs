using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolSupplies.Models.Entities
{
    public class Brand
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid BrandId { get; set; }
        [Required(ErrorMessage = "Brand name is required")]
        [StringLength(50, ErrorMessage = "Brand name cannot be longer than 50 characters")]
        public string BrandName { get; set; }
        [StringLength(200, ErrorMessage = "Brand description cannot be longer than 200 characters")]
        public string BrandDescription { get; set; }
        public virtual ICollection<Product>? Books { get; set; }

    }
}
