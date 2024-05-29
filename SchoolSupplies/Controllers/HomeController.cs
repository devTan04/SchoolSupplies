using Microsoft.AspNetCore.Mvc;

namespace SchoolSupplies.Controllers
{
    [RoleAuthorization("Guest, Staff, Shop Owner")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Demo()
        {
            return View();
        }
    }
}
