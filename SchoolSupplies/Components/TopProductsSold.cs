using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSupplies.Data;

namespace SchoolSupplies.Components
{
    public class TopProductsSold : ViewComponent
    {
        private readonly ApplicationDbContext _dbContext;

        public TopProductsSold(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var topProducts = await _dbContext.OrderDetails
                .GroupBy(od => od.ProductId)
                .Select(g => new { ProductId = g.Key, Quantity = g.Sum(od => od.Quantity) })
                .OrderByDescending(x => x.Quantity)
                .Take(10)
                .ToListAsync();

            var products = new List<dynamic>();
            foreach (var product in topProducts)
            {
                var productInfo = await _dbContext.Products.FindAsync(product.ProductId);
                products.Add(new { ProductName = productInfo.ProductName, Quantity = product.Quantity });
            }

            return View(products);
        }
    }
}
