using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSupplies.Data;
using SchoolSupplies.Helpers;
using SchoolSupplies.Models;
using SchoolSupplies.Models.Entities;
using System.Security.Claims;

namespace SchoolSupplies.Controllers
{
    [RoleAuthorization("Guest, Staff, Shop Owner")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }
        [RoleAuthorization("Staff, Shop Owner")]
        public IActionResult OrderHomeDetail(Guid Id)
        {
            var orderDetails = _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .Where(od => od.OrderId == Id)
                .Select(od => new OrderDetailViewModel
                {
                    OrderStatus = od.Order.OrderStatus,
                    OrderDate = od.Order.OrderDate,
                    ProductId = od.ProductId,
                    ProductName = od.Product.ProductName,
                    ProductUrlImageMain = od.Product.ProductUrlImageMain,
                    Quantity = od.Quantity,
                    Price = od.Price,
                    Total = od.Total,
                    FullName = $"{od.Order.OrderFirstName} {od.Order.OrderLastName}"
                })
                .ToList();

            if (orderDetails == null || orderDetails.Count == 0)
            {
                return NotFound();
            }

            return View(orderDetails);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmOrder(Guid orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
            {
                return NotFound();
            }

            // Update order status to "Received"
            order.OrderStatus = "Confirmed";
            _context.Update(order);
            await _context.SaveChangesAsync();

            // Redirect back to OrderDetail view
            return RedirectToAction("OrderHome");
        }
        [RoleAuthorization("Staff, Shop Owner")]
        public async Task<IActionResult> OrderHome()
        {
            var orders = await _context.Orders
                .Select(o => new OrderHomeViewModel
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    OrderStatus = o.OrderStatus,
                    FullName = $"{o.OrderFirstName} {o.OrderLastName}",
                    OrderPhoneNumber = o.OrderPhoneNumber,
                    OrderAddress = o.OrderAddress,
                    OrderTotal = o.OrderTotal
                })
                .ToListAsync();

            if (orders == null || orders.Count == 0)
            {
                return NotFound();
            }

            return View(orders);
        }



        [HttpGet]
        public IActionResult Create()
        {
            var cartItems = HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();
            var model = new OrderViewModel { CartItems = cartItems };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem người dùng đã đăng nhập chưa
                var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập hoặc hiển thị thông báo lỗi
                    return RedirectToAction("Login", "Access"); // Thay đổi tên Controller và Action tùy thuộc vào ứng dụng của bạn
                }

                var cartItems = HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();

                if (cartItems == null || !cartItems.Any())
                {
                    ModelState.AddModelError("", "Your cart is empty.");
                    return View(model);
                }

                var order = new Order
                {
                    OrderId = Guid.NewGuid(),
                    OrderDate = DateTime.UtcNow,
                    OrderPhoneNumber = model.OrderPhoneNumber,
                    OrderAddress = model.OrderAddress,
                    OrderFirstName = model.OrderFirstName,
                    OrderLastName = model.OrderLastName,
                    OrderStatus = "Pending",
                    UserId = Guid.Parse(userId), // Chuyển đổi từ string sang Guid
                    OrderTotal = cartItems.Sum(ci => ci.Price * ci.Quantity)
                };

                _context.Orders.Add(order);

                foreach (var cartItem in cartItems)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        Price = (decimal)cartItem.Price,
                        Total = (double)(cartItem.Price * cartItem.Quantity)
                    };
                    _context.OrderDetails.Add(orderDetail);
                }

                await _context.SaveChangesAsync();

                // Xóa giỏ hàng sau khi đặt hàng thành công
                HttpContext.Session.Remove(MySetting.CART_KEY);

                return RedirectToAction("Confirmation", new { id = order.OrderId });
            }
            return View(model);
        }

        public IActionResult Confirmation(Guid id)
        {
            var order = _context.Orders
                                .Include(o => o.OrderDetails)
                                .ThenInclude(od => od.Product)
                                .FirstOrDefault(o => o.OrderId == id);

            if (order == null || order.OrderDetails == null)
            {
                return NotFound();
            }

            return View(order);
        }
        [HttpGet]
        public IActionResult OrderDisplay()
        {
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Access");
            }

            var orders = _context.Orders
                                .Where(o => o.UserId == Guid.Parse(userId))
                                .ToList();

            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveOrder(Guid orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
            {
                return NotFound();
            }

            // Update order status to "Received"
            order.OrderStatus = "Received";
            _context.Update(order);
            await _context.SaveChangesAsync();

            // Redirect back to OrderDetail view
            return RedirectToAction("OrderDisplay");
        }

        public IActionResult OrderDetail(Guid Id)
        {
            var orderDetails = _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .Where(od => od.OrderId == Id)
                .Select(od => new OrderDetailViewModel
                {
                    OrderStatus = od.Order.OrderStatus,
                    OrderDate = od.Order.OrderDate,
                    ProductId = od.ProductId,
                    ProductName = od.Product.ProductName,
                    ProductUrlImageMain = od.Product.ProductUrlImageMain,
                    Quantity = od.Quantity,
                    Price = od.Price,
                    Total = od.Total,
                    FullName = $"{od.Order.OrderFirstName} {od.Order.OrderLastName}"
                })
                .ToList();

            if (orderDetails == null || orderDetails.Count == 0)
            {
                return NotFound();
            }

            return View(orderDetails);
        }

        public IActionResult Feedback(Guid productId)
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa
            var existingFeedback = _context.Feedbacks.FirstOrDefault(f => f.ProductId == productId);
            if (existingFeedback != null)
            {
                return RedirectToAction("Index", "Shop");
            }
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập hoặc hiển thị thông báo lỗi
                return RedirectToAction("Login", "Access"); // Thay đổi tên Controller và Action tùy thuộc vào ứng dụng của bạn
            }

            // Lấy thông tin sản phẩm dựa trên productId
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
            {
                return NotFound();
            }

            // Tạo model Feedback và gán giá trị cho các thuộc tính
            var feedbackModel = new FeedbackViewModel
            {
                ProductId = productId,
                UserId = Guid.Parse(userId),

            };

            return View(feedbackModel);
        }

        [HttpPost]
        public async Task<IActionResult> Feedback(FeedbackViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Tạo đối tượng Feedback từ dữ liệu mẫu
                var feedback = new Feedback
                {
                    UserId = model.UserId,
                    ProductId = model.ProductId,
                    Content = model.Content,
                    Rating = model.Rating,
                    Datatime = model.Datetime
                };

                // Thêm phản hồi vào cơ sở dữ liệu và lưu thay đổi
                _context.Feedbacks.Add(feedback);
                await _context.SaveChangesAsync();

                // Chuyển hướng người dùng đến trang Shop Index sau khi gửi phản hồi thành công
                return RedirectToAction("Index", "Shop"); // Thay đổi tên Controller và Action tùy thuộc vào ứng dụng của bạn
            }

            // Nếu dữ liệu mẫu không hợp lệ, hiển thị lại mẫu gửi phản hồi
            return View(model);
        }
    }
}

