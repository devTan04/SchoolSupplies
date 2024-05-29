using SchoolSupplies.Models.Entities;

namespace SchoolSupplies.Models
{
    public class ProductDetailsVM
    {
        public List<Image> Images { get; set; }
        public Product Product { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public List<FeedbackVM> Feedbacks { get; set; }
    }

    public class FeedbackVM
    {
        public string UserName { get; set; }
        public int Rating { get; set; }
        public string Content { get; set; }
        public DateTime Datatime { get; set; }
    }
}
