using Microsoft.AspNetCore.Mvc;

namespace SchoolSupplies.Components
{
    public class Background : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
