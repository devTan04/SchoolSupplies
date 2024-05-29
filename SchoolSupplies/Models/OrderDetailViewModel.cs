namespace SchoolSupplies.Models
{
    public class OrderDetailViewModel
    {
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public string FullName { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductUrlImageMain { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public double Total { get; set; }
    }
}
