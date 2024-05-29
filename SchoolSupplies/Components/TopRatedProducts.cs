using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSupplies.Data;

namespace SchoolSupplies.Components
{
    public class TopRatedProducts : ViewComponent
    {
        private readonly ApplicationDbContext _dbcontext;
        public TopRatedProducts(ApplicationDbContext dbContext)
        {
            _dbcontext = dbContext;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var ratedProducts = await _dbcontext.Products
                .Where(p => p.Feedbacks.Any(f => f.Rating >= 3 && f.Rating <= 5))
                .Include(p => p.Feedbacks)
                .ToListAsync();

            return View(ratedProducts);
        }
    }
}
