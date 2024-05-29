using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSupplies.Data;
using SchoolSupplies.Models;

namespace SchoolSupplies.Controllers
{
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        public int pageSite = 9;
        public ShopController(ApplicationDbContext dbContext)
        {
            _dbcontext = dbContext;
        }
        public async Task<IActionResult> Search(string? query)
        {
            var products = _dbcontext.Products.AsQueryable();
            if (query != null)
            {
                products = products.Where(p => p.ProductName.Contains(query));
            }
            var result = products.Select(p => new ProductVM
            {
                Id = p.ProductId,
                ProductName = p.ProductName,
                ProductPrice = p.ProductPrice,
                ProductImage = p.ProductUrlImageMain,
            });
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid? id, int? priceMin, int? priceMax)
        {
            // Debugging lines
            Console.WriteLine($"Received PriceMin: {priceMin}, PriceMax: {priceMax}");

            var categories = await _dbcontext.Categories.ToListAsync();
            ViewBag.Categories = categories;

            var products = _dbcontext.Products.AsQueryable();

            if (id.HasValue)
            {
                products = products.Where(p => p.CategoryId == id.Value);
            }

            if (priceMin.HasValue && priceMax.HasValue)
            {
                Console.WriteLine($"Filtering products within price range: {priceMin.Value} - {priceMax.Value}");
                products = products.Where(p => p.ProductPrice >= priceMin.Value && p.ProductPrice <= priceMax.Value);
            }
            else
            {
                Console.WriteLine("PriceMin or PriceMax is null");
            }

            var result = await products.Select(p => new ProductVM
            {
                Id = p.ProductId,
                ProductName = p.ProductName,
                ProductPrice = p.ProductPrice,
                ProductImage = p.ProductUrlImageMain,
            }).ToListAsync();

            return View(result);
        }

    }
}
