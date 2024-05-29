using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolSupplies.Data;
using SchoolSupplies.Models;
using SchoolSupplies.Models.Entities;

namespace SchoolSupplies.Controllers
{
    [RoleAuthorization("Staff, Shop Owner")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly IWebHostEnvironment _webHost;

        public ProductController(ApplicationDbContext dbcontext, IWebHostEnvironment webHost)
        {
            _dbcontext = dbcontext;
            _webHost = webHost;
        }


        public async Task<IActionResult> Display()
        {
            var products = await _dbcontext.Products.ToListAsync();
            return View(products);
        }
        public async Task<IActionResult> ToggleProductHot(Guid productId)
        {
            var product = await _dbcontext.Products.FindAsync(productId);
            if (product != null)
            {
                product.ProductHot = !product.ProductHot;
                await _dbcontext.SaveChangesAsync();
            }
            return RedirectToAction("Display");
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var products = await _dbcontext.Products.Include(p => p.Category).Include(p => p.Brand).ToListAsync();
            return View(products);
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
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_dbcontext.Brands.ToList(), "BrandId", "BrandName");
            ViewData["CategoryId"] = new SelectList(_dbcontext.Categories.ToList(), "CategoryId", "CategoryName");
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(AddProductViewModel viewModel, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("ProductUrlImageMain", "Category image is required");
                ViewData["BrandId"] = new SelectList(_dbcontext.Brands.ToList(), "BrandId", "BrandName");
                ViewData["CategoryId"] = new SelectList(_dbcontext.Categories.ToList(), "CategoryId", "CategoryName");
                return View(viewModel);
            }
            if (viewModel.BrandId == null || viewModel.CategoryId == null)
            {
                // Xử lý trường hợp khi BrandId hoặc CategoryId không có giá trị
                TempData["ErrorMessage"] = "Please select Brand and Category.";
                return View(viewModel);
            }
            // Kiểm tra xem BrandId và CategoryId đã được chọn chưa
            if (viewModel.BrandId == Guid.Empty)
            {

                ModelState.AddModelError("BrandId", "Please select a brand");
            }

            if (viewModel.CategoryId == Guid.Empty)
            {
                ModelState.AddModelError("CategoryId", "Please select a category");
            }
            // Thiết lập giá trị mặc định cho ProductHot và ProductLike
            viewModel.ProductHot = false;
            viewModel.ProductLike = false;

            // Kiểm tra tệp tải lên trước khi xác minh ModelState
            if (!IsImage(file))
            {
                ModelState.AddModelError("ProductUrlImageMain", "Please select a valid image file.");
                ViewData["BrandId"] = new SelectList(_dbcontext.Brands.ToList(), "BrandId", "BrandName");
                ViewData["CategoryId"] = new SelectList(_dbcontext.Categories.ToList(), "CategoryId", "CategoryName");
                return View(viewModel);
            }

            string uploadsFolder = Path.Combine(_webHost.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); // Create a unique name for the file
            string fileSavePath = Path.Combine(uploadsFolder, fileName); // Path to the storage directory
            using (FileStream stream = new FileStream(fileSavePath, FileMode.Create))
            {
                await file.CopyToAsync(stream); // Save the file to the path
            }
            viewModel.ProductUrlImageMain = fileName;

            var product = new Product
            {
                ProductName = viewModel.ProductName,
                ProductUrlImageMain = viewModel.ProductUrlImageMain,
                ProductDescription = viewModel.ProductDescription,
                ProductPrice = viewModel.ProductPrice,
                ProductDiscount = viewModel.ProductDiscount,
                ProductLike = viewModel.ProductLike,
                ProductHot = viewModel.ProductHot,
                CategoryId = viewModel.CategoryId,
                BrandId = viewModel.BrandId
            };

            await _dbcontext.Products.AddAsync(product);
            await _dbcontext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Product created successfully.";
            return RedirectToAction("Create", "Product");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id, string img)
        {
            ViewData["BrandId"] = new SelectList(_dbcontext.Brands.ToList(), "BrandId", "BrandName");
            ViewData["CategoryId"] = new SelectList(_dbcontext.Categories.ToList(), "CategoryId", "CategoryName");
            var product = await _dbcontext.Products.FindAsync(id);
            ViewBag.ImageUrl = "/uploads/" + img;
            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Product viewModel, IFormFile file)
        {
            var product = await _dbcontext.Products.FindAsync(viewModel.ProductId);

            if (product == null)
            {
                return NotFound(); // Trả về lỗi 404 nếu không tìm thấy sản phẩm
            }

            // Nếu người dùng chọn một file mới
            if (file != null && file.Length > 0)
            {
                if (file == null || file.Length == 0)
                {
                    ModelState.AddModelError("ProductUrlImageMain", "Category image is required");
                    ViewData["BrandId"] = new SelectList(_dbcontext.Brands.ToList(), "BrandId", "BrandName");
                    ViewData["CategoryId"] = new SelectList(_dbcontext.Categories.ToList(), "CategoryId", "CategoryName");
                    return View(viewModel);
                }
                // Thiết lập giá trị mặc định cho ProductHot và ProductLike
/*                viewModel.ProductHot = false;
                viewModel.ProductLike = false;*/

                // Kiểm tra tệp tải lên trước khi xác minh ModelState
                if (!IsImage(file))
                {
                    ModelState.AddModelError("ProductUrlImageMain", "Please select a valid image file.");
                    ViewData["BrandId"] = new SelectList(_dbcontext.Brands.ToList(), "BrandId", "BrandName");
                    ViewData["CategoryId"] = new SelectList(_dbcontext.Categories.ToList(), "CategoryId", "CategoryName");
                    return View(viewModel);
                }
                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); // Tạo tên mới cho file
                string fileSavePath = Path.Combine(uploadsFolder, fileName);// Đường dẫn lưu trữ file
                using (FileStream stream = new FileStream(fileSavePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream); // Lưu file vào đường dẫn
                }
                viewModel.ProductUrlImageMain = fileName;
            }
            else
            {
                // Người dùng không chọn file mới, giữ nguyên hình ảnh cũ
                viewModel.ProductUrlImageMain = product.ProductUrlImageMain;
            }

            // Cập nhật thông tin sản phẩm với thông tin mới từ viewModel
            product.ProductName = viewModel.ProductName;
            product.ProductUrlImageMain = viewModel.ProductUrlImageMain;
            product.ProductDescription = viewModel.ProductDescription;
            product.ProductPrice = viewModel.ProductPrice;
            product.ProductDiscount = viewModel.ProductDiscount;
            product.ProductLike = false; // Không thay đổi giá trị ProductLike
            product.ProductHot = false; // Không thay đổi giá trị ProductHot
            product.CategoryId = viewModel.CategoryId;
            product.BrandId = viewModel.BrandId;

            if (viewModel.BrandId != null)
            {
                product.BrandId = viewModel.BrandId;
            }

            if (viewModel.CategoryId != null)
            {
                product.CategoryId = viewModel.CategoryId;
            }

            await _dbcontext.SaveChangesAsync();

            return RedirectToAction("List", "Product");
        }
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id, string img)
        {
            var product = await _dbcontext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            else
            {
                if (product.ProductUrlImageMain == null)
                {
                    _dbcontext.Products.Remove(product);
                    await _dbcontext.SaveChangesAsync();
                    return RedirectToAction("List", "Product");
                }
                else
                {
                    img = Path.Combine("wwwroot", "uploads", img);
                    FileInfo infor = new FileInfo(img);
                    if(infor != null){
                        System.IO.File.Delete(img);
                        infor.Delete();
                    }
                    _dbcontext.Products.Remove(product);
                    await _dbcontext.SaveChangesAsync();
                    return RedirectToAction("List", "Product");
                }   
            }
        }
    }     
}
