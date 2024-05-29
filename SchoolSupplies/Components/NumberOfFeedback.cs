using Microsoft.AspNetCore.Mvc;
using SchoolSupplies.Data;

namespace SchoolSupplies.Components
{
    public class NumberOfFeedback : ViewComponent
    {
        private readonly ApplicationDbContext _dbcontext;
        public NumberOfFeedback(ApplicationDbContext dbContext)
        {
            _dbcontext = dbContext;
        }
        public IViewComponentResult Invoke()
        {
            var orderCount = _dbcontext.Feedbacks.Count();
            return View(orderCount);
        }
    }
}
