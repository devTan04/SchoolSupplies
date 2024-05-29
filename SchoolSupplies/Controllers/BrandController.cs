using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSupplies.Data;
using SchoolSupplies.Models;
using SchoolSupplies.Models.Entities;

namespace SchoolSupplies.Controllers
{
    [RoleAuthorization("Staff, Shop Owner")]
    public class BrandController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly INotyfService _notify;

        public BrandController(ILogger<HomeController> logger, ApplicationDbContext dbContext, INotyfService notify)
        {
            _dbcontext = dbContext;
            _notify = notify;
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var brand = await _dbcontext.Brands.ToListAsync();
            return View(brand);
        }
        public IActionResult Create() 
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(AddBrandViewModel viewModel)
        {
            var brand = new Brand
            {
                BrandName = viewModel.BrandName,
                BrandDescription = viewModel.BrandDescription,
            };
            if (brand != null)
            {
                await _dbcontext.Brands.AddAsync(brand);
                await _dbcontext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Brand created successfully.";
            }
            return RedirectToAction("Create", "Brand");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var brand = await _dbcontext.Brands.FindAsync(id);
            return View(brand);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Brand viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            var brand = await _dbcontext.Brands.FindAsync(viewModel.BrandId);
            if (brand != null)
            {
                brand.BrandName = viewModel.BrandName;
                brand.BrandDescription = viewModel.BrandDescription;
                await _dbcontext.SaveChangesAsync();
            }
            TempData["SuccessMessage"] = "Brand updated successfully.";
            return RedirectToAction("List", "Brand");
            
        }
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var brand = await _dbcontext.Brands.AsNoTracking().FirstOrDefaultAsync(p => p.BrandId == id);
            if (brand == null)
            {
                return NotFound();
            }
            else
            {
                _dbcontext.Brands.Remove(brand);
                await _dbcontext.SaveChangesAsync();
                return RedirectToAction("List", "Brand");
            }
        }
    }
}
