using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSupplies.Data;
using SchoolSupplies.Models;

namespace SchoolSupplies.Components
{
    public class SaveOff : ViewComponent
    {
        private readonly ApplicationDbContext _dbcontext;

        public SaveOff(ApplicationDbContext dbContext)
        {
            _dbcontext = dbContext;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var discountedProducts = await _dbcontext.Products
                .Where(p => p.ProductDiscount > 0)
                .Select(p => new DiscountedProductVM
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductUrlImageMain = p.ProductUrlImageMain,
                    ProductPrice = p.ProductPrice,
                    ProductDiscount = p.ProductDiscount,
                    CategoryName = p.Category.CategoryName
                }).ToListAsync();

            return View(discountedProducts);
        }

    }
}
