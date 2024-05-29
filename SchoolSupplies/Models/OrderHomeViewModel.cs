namespace SchoolSupplies.Models
{
    public class OrderHomeViewModel
    {
        public Guid OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public string FullName { get; set; }
        public string OrderPhoneNumber { get; set; }
        public string OrderAddress { get; set; }
        public double OrderTotal { get; set; }
    }
}
