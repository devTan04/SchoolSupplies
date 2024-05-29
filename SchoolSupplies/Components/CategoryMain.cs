using Microsoft.AspNetCore.Mvc;
using SchoolSupplies.Data;

namespace SchoolSupplies.Components
{
    public class CategoryMain : ViewComponent
    {
        private readonly ApplicationDbContext _dbcontext;
        public CategoryMain(ApplicationDbContext dbContext)
        {
            _dbcontext = dbContext;
        }
        public IViewComponentResult Invoke()
        {
            return View(_dbcontext.Categories.ToList());
        }
    }
}
