using Microsoft.AspNetCore.Mvc;
using SchoolSupplies.Data;

namespace SchoolSupplies.Components
{
    public class MonthlyRevenue : ViewComponent
    {
        private readonly ApplicationDbContext _dbContext;

        public MonthlyRevenue(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Lấy dữ liệu doanh thu của 12 tháng trong năm
            var monthlyRevenue = new double[12];
            var currentYear = DateTime.UtcNow.Year;

            for (int i = 1; i <= 12; i++)
            {
                var startDate = new DateTime(currentYear, i, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                monthlyRevenue[i - 1] = _dbContext.Orders
                    .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                    .Sum(o => o.OrderTotal);
            }

            // Truyền dữ liệu vào view
            return View(monthlyRevenue);
        }
    }
}
