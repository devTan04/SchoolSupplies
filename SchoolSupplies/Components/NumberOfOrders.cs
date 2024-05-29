using Microsoft.AspNetCore.Mvc;
using SchoolSupplies.Data;

namespace SchoolSupplies.Components
{
    public class NumberOfOrders : ViewComponent
    {
        private readonly ApplicationDbContext _dbcontext;
        public NumberOfOrders(ApplicationDbContext dbContext)
        {
            _dbcontext = dbContext;
        }
        public IViewComponentResult Invoke()
        {
            // Đếm số lượng đơn hàng
            var orderCount = _dbcontext.Orders.Count();
            // Truyền số lượng đơn hàng vào view
            return View(orderCount);
        }
    }
}
