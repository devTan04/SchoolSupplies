using Microsoft.AspNetCore.Mvc;
using SchoolSupplies.Helpers;
using SchoolSupplies.Models;

namespace SchoolSupplies.Components
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();
            return View(new CartModel
            {
                Quantity = cart.Sum(x => x.Quantity),
                Total = cart.Sum(x => x.Total),
            });
        }
    }
}
