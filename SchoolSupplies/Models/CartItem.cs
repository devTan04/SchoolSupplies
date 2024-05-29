namespace SchoolSupplies.Models
{
    public class CartItem
    {
        public Guid ProductId { get; set; }
        public string ProductImage { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double Total => Quantity * Price;
    }
}
