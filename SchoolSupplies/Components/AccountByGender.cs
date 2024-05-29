using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSupplies.Data;

namespace SchoolSupplies.Components
{
    public class AccountByGender : ViewComponent
    {
        private readonly ApplicationDbContext _dbcontext;
        public AccountByGender(ApplicationDbContext dbContext)
        {
            _dbcontext = dbContext;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var maleCount = await _dbcontext.Users.CountAsync(u => u.Gender == "Male");
            var femaleCount = await _dbcontext.Users.CountAsync(u => u.Gender == "Female");

            var genderData = new
            {
                Male = maleCount,
                Female = femaleCount
            };

            return View(genderData);
        }
    }
}
