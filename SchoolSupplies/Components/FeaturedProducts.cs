using Microsoft.AspNetCore.Mvc;
using SchoolSupplies.Data;
using SchoolSupplies.Models;

namespace SchoolSupplies.Components
{
    public class FeaturedProducts : ViewComponent
    {
        private readonly ApplicationDbContext _dbcontext;

        public FeaturedProducts(ApplicationDbContext dbContext)
        {
            _dbcontext = dbContext;
        }
        public IViewComponentResult Invoke()
        {
            var products = _dbcontext.Products.Where(p => p.ProductHot).ToList();

            var viewModel = new FeaturedProductsViewModel
            {
                Products = products
            };

            return View("~/Views/Shared/Components/FeaturedProducts/Default.cshtml", viewModel);
        }

    }
}
