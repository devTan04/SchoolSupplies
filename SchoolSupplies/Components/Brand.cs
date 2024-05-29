using Microsoft.AspNetCore.Mvc;
using SchoolSupplies.Data;

namespace SchoolSupplies.Components
{
    public class Brand:ViewComponent
    {
        private readonly ApplicationDbContext _dbcontext;

        public Brand(ApplicationDbContext dbContext)
        {
            _dbcontext = dbContext;
        }
        public IViewComponentResult Invoke() 
        {
            return View(_dbcontext.Brands.ToList()); 
        }
    }
}
