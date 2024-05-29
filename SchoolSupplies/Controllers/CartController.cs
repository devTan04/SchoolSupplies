using Microsoft.AspNetCore.Mvc;
using SchoolSupplies.Data;
using SchoolSupplies.Helpers;
using SchoolSupplies.Models;

namespace SchoolSupplies.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;

        public CartController(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            // Log giỏ hàng ra để kiểm tra
            foreach (var item in cart)
            {
                Console.WriteLine($"ProductId: {item.ProductId}, ProductName: {item.ProductName}, Quantity: {item.Quantity}");
            }
            return View(cart);
        }



        public List<CartItem> GetCart()
        {
            return HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();
        }

        public IActionResult AddToCart(Guid id, int quantity = 1)
        {
            var cart = GetCart();
            var item = cart.SingleOrDefault(p => p.ProductId == id);
            if (item == null)
            {
                var product = _dbcontext.Products.SingleOrDefault(p => p.ProductId == id);
                if (product == null)
                {
                    TempData["Message"] = $"Not Found product {id}";
                    return Redirect("/404");
                }
                item = new CartItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Price = product.ProductPrice,
                    ProductImage = product.ProductUrlImageMain ?? string.Empty,
                    Quantity = quantity,
                };
                cart.Add(item);
            }
            else
            {
                item.Quantity += quantity;
            }
            HttpContext.Session.Set(MySetting.CART_KEY, cart);
            return RedirectToAction("Index");
        }
        public IActionResult RemoveCart(Guid id) 
        {
            var cart = GetCart();
            var item = cart.SingleOrDefault(p => p.ProductId == id);
            if(item != null)
            {
                cart.Remove(item);
                HttpContext.Session.Set(MySetting.CART_KEY, cart);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult UpdateCart(List<CartItem> updatedCart)
        {
            var cart = GetCart();
            foreach (var updatedItem in updatedCart)
            {
                var cartItem = cart.SingleOrDefault(p => p.ProductId == updatedItem.ProductId);
                if (cartItem != null)
                {
                    cartItem.Quantity = updatedItem.Quantity;
                }
            }
            HttpContext.Session.Set(MySetting.CART_KEY, cart);
            return RedirectToAction("Index");
        }
    }
}
