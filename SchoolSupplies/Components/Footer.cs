using Microsoft.AspNetCore.Mvc;

namespace SchoolSupplies.Components
{
    public class Footer : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
