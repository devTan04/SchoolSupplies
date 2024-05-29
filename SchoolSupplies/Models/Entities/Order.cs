using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolSupplies.Models.Entities
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid OrderId { get; set; }

        [Required(ErrorMessage = "OrderDate is required")]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "OrderPhoneNumber is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string OrderPhoneNumber { get; set; }

        [Required(ErrorMessage = "OrderAddress is required")]
        [StringLength(500, ErrorMessage = "OrderAddress can't be longer than 500 characters")]
        public string OrderAddress { get; set; }

        [Required(ErrorMessage = "OrderFirstName is required")]
        [StringLength(100, ErrorMessage = "OrderFirstName can't be longer than 100 characters")]
        public string OrderFirstName { get; set; }

        [Required(ErrorMessage = "OrderLastName is required")]
        [StringLength(100, ErrorMessage = "OrderLastName can't be longer than 100 characters")]
        public string OrderLastName { get; set; }

        [Required(ErrorMessage = "OrderStatus is required")]
        [StringLength(50, ErrorMessage = "OrderStatus can't be longer than 50 characters")]
        public string OrderStatus { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "OrderTotal must be a positive value")]
        public double OrderTotal { get; set; }

        [Required(ErrorMessage = "UserId is required")]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
