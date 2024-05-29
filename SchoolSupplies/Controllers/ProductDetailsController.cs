using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSupplies.Data;
using SchoolSupplies.Models;

namespace SchoolSupplies.Controllers
{
    [RoleAuthorization("Guest, Staff, Shop Owner")]
    public class ProductDetailsController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly IWebHostEnvironment _webHost;

        public ProductDetailsController(ApplicationDbContext dbcontext, IWebHostEnvironment webHost)
        {
            _dbcontext = dbcontext;
            _webHost = webHost;
        }
        public async Task<IActionResult> Details(Guid id)
        {
            var productDetails = await _dbcontext.Products
                .Include(p => p.Images)
                .Include(p => p.Feedbacks)
                    .ThenInclude(f => f.User)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (productDetails == null)
            {
                return NotFound();
            }

            var viewModel = new ProductDetailsVM
            {
                Product = productDetails,
                Images = productDetails.Images.ToList(),
                Feedbacks = productDetails.Feedbacks.Select(f => new FeedbackVM
                {
                    UserName = f.User.UserEmail, // Assuming User has an Email property
                    Rating = f.Rating,
                    Content = f.Content,
                    Datatime = f.Datatime
                }).ToList()
            };

            return View(viewModel);
        }


    }
}
