namespace SchoolSupplies.Models
{
    public class SubImageViewModel
    {
        public Guid ProductId { get; set; }
        public List<IFormFile> SubImages { get; set; }
    }
}
