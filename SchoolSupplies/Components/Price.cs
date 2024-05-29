using Microsoft.AspNetCore.Mvc;
using SchoolSupplies.Data;

namespace SchoolSupplies.Components
{
    public class Price : ViewComponent
    {
        private readonly ApplicationDbContext _dbcontext;

        public Price(ApplicationDbContext dbContext)
        {
            _dbcontext = dbContext;
        }
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
