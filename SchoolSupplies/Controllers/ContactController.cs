using Microsoft.AspNetCore.Mvc;

namespace SchoolSupplies.Controllers
{
    [RoleAuthorization("Guest, Staff, Shop Owner")]
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
