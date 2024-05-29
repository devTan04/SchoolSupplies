using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSupplies.Data;
using SchoolSupplies.Models;
using SchoolSupplies.Models.Entities;

namespace SchoolSupplies.Controllers
{
    public class AddSubImagesController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<AddSubImagesController> _logger;

        public AddSubImagesController(ApplicationDbContext dbcontext, IWebHostEnvironment webHostEnvironment, ILogger<AddSubImagesController> logger)
        {
            _dbcontext = dbcontext;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }
        public IActionResult AddImages(Guid id)
        {
            var viewModel = new SubImageViewModel
            {
                ProductId = id,
                SubImages = new List<IFormFile>()
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddImages(SubImageViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    foreach (var formFile in model.SubImages)
                    {
                        if (formFile.Length > 0)
                        {
                            var fileName = Path.GetFileName(formFile.FileName);
                            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await formFile.CopyToAsync(stream);
                            }

                            var image = new Image
                            {
                                ImageId = Guid.NewGuid(),
                                SupImage = fileName,
                                ProductId = model.ProductId
                            };

                            _dbcontext.Images.Add(image);
                        }
                    }

                    await _dbcontext.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Images added successfully!";
                    return RedirectToAction("Display", "Product");
                }
                catch (DbUpdateException ex)
                {
                    var innerException = ex.InnerException?.Message;
                    _logger.LogError(ex, "Error while adding images. Inner exception: {InnerException}", innerException);
                    TempData["ErrorMessage"] = $"Failed to add images. Error: {innerException}";
                    return View(model);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while adding images.");
                    TempData["ErrorMessage"] = "Failed to add images.";
                    return View(model);
                }
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogError(error.ErrorMessage);
                }
                TempData["ErrorMessage"] = "Model state is invalid.";
                return View(model);
            }
        }
        public async Task<IActionResult> ViewImages(Guid id)
        {
            var product = await _dbcontext.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductImagesViewModel
            {
                Product = product,
                Images = product.Images.ToList()
            };

            return View(viewModel);
        }

        public IActionResult UpdateSubImages(Guid id)
        {
            // Lấy thông tin sản phẩm từ cơ sở dữ liệu
            var product = _dbcontext.Products
                .Include(p => p.Images)
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        public IActionResult UpdateSubImage(Guid id)
        {
            var image = _dbcontext.Images
                .Include(i => i.Product)
                .FirstOrDefault(i => i.ImageId == id);

            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSubImage(Guid id, IFormFile newImage)
        {
            var image = await _dbcontext.Images
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.ImageId == id);

            if (image == null)
            {
                return NotFound();
            }

            if (newImage != null && newImage.Length > 0)
            {
                try
                {
                    var fileName = Path.GetFileName(newImage.FileName);
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await newImage.CopyToAsync(stream);
                    }

                    // Cập nhật đường dẫn mới cho ảnh
                    image.SupImage = fileName;

                    await _dbcontext.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Image updated successfully!";
                    return RedirectToAction("ViewImages", new { id = image.ProductId });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while updating image.");
                    TempData["ErrorMessage"] = "Failed to update image.";
                    return View(image);
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please select a file.";
                return View(image);
            }
        }
    }
}
