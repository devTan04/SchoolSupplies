using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace SchoolSupplies.Controllers
{
    public class RoleAuthorizationAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public RoleAuthorizationAttribute(string roles)
        {
            _roles = roles.Split(',').Select(r => r.Trim()).ToArray();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (_roles.Any(role => user.IsInRole(role)))
            {
                return; // User is in one of the roles
            }

            // User is not in any of the roles, redirect to Not Found
            context.Result = new RedirectToActionResult("Display", "NotFound", null);
        }
    }
}
