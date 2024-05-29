using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolSupplies.Data;
using SchoolSupplies.Models;
using SchoolSupplies.Models.Entities;

namespace SchoolSupplies.Controllers
{
    [RoleAuthorization("Staff, Shop Owner")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly IWebHostEnvironment _webHost;

        public CategoryController(ApplicationDbContext dbcontext, IWebHostEnvironment webHost)
        {
            _dbcontext = dbcontext;
            _webHost = webHost;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var category = await _dbcontext.Categories.ToListAsync();
            return View(category);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(AddCategoryViewModel viewModel, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("CategoryImage", "Category image is required");
                return View(viewModel);
            }

            try
            {
                if (!IsImage(file))
                {
                    ModelState.AddModelError("CategoryImage", "Please select a valid image file.");
                    ViewData["BrandId"] = new SelectList(_dbcontext.Brands.ToList(), "BrandId", "BrandName");
                    ViewData["CategoryId"] = new SelectList(_dbcontext.Categories.ToList(), "CategoryId", "CategoryName");
                    return View(viewModel);
                }
                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "categories");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string fileSavePath = Path.Combine(uploadsFolder, fileName);
                using (FileStream stream = new FileStream(fileSavePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                viewModel.CategoryImage = fileName;

                var category = new Category
                {
                    CategoryName = viewModel.CategoryName,
                    CategoryImage = viewModel.CategoryImage,
                    CategoryDescription = viewModel.CategoryDescription,
                };

                if (category != null)
                {
                    await _dbcontext.Categories.AddAsync(category);
                    await _dbcontext.SaveChangesAsync();
                }
                TempData["SuccessMessage"] = "Category created successfully.";
                return RedirectToAction("Create", "Category");
            }
            catch (Exception ex)
            {
                // Log exception and handle it appropriately
                ModelState.AddModelError("CategoryName", "An error occurred while processing your request.");
                return View(viewModel);
            }
        }
        private bool IsImage(IFormFile file)
        {
            // Kiểm tra loại tệp có phải là hình ảnh hay không
            if (file.ContentType.Contains("image"))
            {
                return true;
            }

            string[] allowedFileTypes = { ".jpg", ".jpeg", ".png", ".gif" };
            string fileExtension = Path.GetExtension(file.FileName).ToLower();
            return allowedFileTypes.Contains(fileExtension);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id, string img)
        {
            var category = await _dbcontext.Categories.FindAsync(id);
            ViewBag.ImageUrl = "/categories/" + img;
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category viewModel, IFormFile file)
        {
            var category = await _dbcontext.Categories.FindAsync(viewModel.CategoryId);

            // Kiểm tra xem người dùng đã chọn tệp hay chưa
            if (file != null)
            {
                // Kiểm tra xem tệp được chọn có phải là hình ảnh hay không
                if (!IsImage(file))
                {
                    ModelState.AddModelError("CategoryImage", "Please select a valid image file.");
                    return View(viewModel);
                }

                // Xử lý tệp được chọn
                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "categories");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); // Tạo tên duy nhất cho tệp
                string fileSavePath = Path.Combine(uploadsFolder, fileName); // Đường dẫn lưu trữ tệp
                using (FileStream stream = new FileStream(fileSavePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream); // Lưu tệp vào đường dẫn
                }
                viewModel.CategoryImage = fileName;
            }
            else
            {
                // Người dùng không chọn tệp mới, giữ nguyên ảnh cũ
                viewModel.CategoryImage = category.CategoryImage;
            }

            // Cập nhật thông tin category
            category.CategoryName = viewModel.CategoryName;
            category.CategoryImage = viewModel.CategoryImage;
            category.CategoryDescription = viewModel.CategoryDescription;

            // Lưu thay đổi vào cơ sở dữ liệu
            await _dbcontext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Category updated successfully.";
            return RedirectToAction("List", "Category");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var category = await _dbcontext.Categories.AsNoTracking().FirstOrDefaultAsync(p => p.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }
            else
            {
                _dbcontext.Categories.Remove(category);
                await _dbcontext.SaveChangesAsync();
                return RedirectToAction("List", "Category");
            }           
        }
    }
}
