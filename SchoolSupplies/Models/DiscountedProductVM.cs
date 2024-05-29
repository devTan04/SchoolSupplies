namespace SchoolSupplies.Models
{
    public class DiscountedProductVM
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductUrlImageMain { get; set; }
        public double ProductPrice { get; set; }
        public int ProductDiscount { get; set; }
        public string CategoryName { get; set; }
    }
}
