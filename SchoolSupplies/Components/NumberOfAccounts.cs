using Microsoft.AspNetCore.Mvc;
using SchoolSupplies.Data;

namespace SchoolSupplies.Components
{
    public class NumberOfAccounts : ViewComponent
    {
        private readonly ApplicationDbContext _dbcontext;
        public NumberOfAccounts(ApplicationDbContext dbContext)
        {
            _dbcontext = dbContext;
        }
        public IViewComponentResult Invoke()
        {
            var orderCount = _dbcontext.Users.Count();
            // Truyền số lượng đơn hàng vào view
            return View(orderCount);
        }
    }
}
