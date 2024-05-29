using System.ComponentModel.DataAnnotations;

namespace SchoolSupplies.Models
{
    public class FeedbackViewModel
    {

        public Guid UserId { get; set; }

        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [StringLength(1000, ErrorMessage = "Content cannot be longer than 1000 characters")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Datetime is required")]
        public DateTime Datetime { get; set; } = DateTime.UtcNow;

    }
}
