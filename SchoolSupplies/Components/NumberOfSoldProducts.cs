using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSupplies.Data;

namespace SchoolSupplies.Components
{
    public class NumberOfSoldProducts : ViewComponent
    {
        private readonly ApplicationDbContext _dbcontext;
        public NumberOfSoldProducts(ApplicationDbContext dbContext)
        {
            _dbcontext = dbContext;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var totalQuantity = await _dbcontext.OrderDetails.SumAsync(od => od.Quantity);
            return View(totalQuantity);
        }
    }
}
