using Microsoft.AspNetCore.Mvc;
using SchoolSupplies.Data;

namespace SchoolSupplies.Components
{
    public class RelatedProduct : ViewComponent
    {
        private readonly ApplicationDbContext _dbcontext;

        public RelatedProduct(ApplicationDbContext dbContext)
        {
            _dbcontext = dbContext;
        }
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
