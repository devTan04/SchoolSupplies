using Microsoft.AspNetCore.Mvc;

namespace SchoolSupplies.Controllers
{
    public class NotFoundController : Controller
    {
        [Route("/404")]
        public IActionResult Display()
        {
            return View();
        }
    }
}
