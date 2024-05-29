using Microsoft.AspNetCore.Mvc;
using SchoolSupplies.Data;

namespace SchoolSupplies.Components
{
    public class CategoryImage : ViewComponent
    {
        private readonly ApplicationDbContext _dbcontext;
        public CategoryImage(ApplicationDbContext dbContext)
        {
            _dbcontext = dbContext;
        }
        public IViewComponentResult Invoke()
        {
            return View(_dbcontext.Categories.ToList());
        }
    }

}
