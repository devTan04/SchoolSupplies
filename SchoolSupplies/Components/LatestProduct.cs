using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSupplies.Data;

namespace SchoolSupplies.Components
{
    public class LatestProduct : ViewComponent
    {
        private readonly ApplicationDbContext _dbcontext;
        public LatestProduct(ApplicationDbContext dbContext)
        {
            _dbcontext = dbContext;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var topOrderedProducts = await _dbcontext.Products
                .Include(p => p.OrderDetails)
                .OrderByDescending(p => p.OrderDetails.Count)
                .Take(10) // Số lượng sản phẩm muốn hiển thị
                .ToListAsync();

            return View(topOrderedProducts);
        }
    }
}
