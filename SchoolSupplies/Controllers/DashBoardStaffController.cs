using Microsoft.AspNetCore.Mvc;

namespace SchoolSupplies.Controllers
{
    [RoleAuthorization("Staff, Shop Owner")]
    public class DashBoardStaffController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
