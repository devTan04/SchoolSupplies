using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSupplies.Data;

namespace SchoolSupplies.Components
{
    public class HotProducts : ViewComponent
    {
        private readonly ApplicationDbContext _dbcontext;
        public HotProducts(ApplicationDbContext dbContext)
        {
            _dbcontext = dbContext;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var hotProducts = await _dbcontext.Products
                .Where(p => p.ProductHot)
                .ToListAsync();

            return View(hotProducts);
        }
    }
}
