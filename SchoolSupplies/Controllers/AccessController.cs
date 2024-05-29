using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSupplies.Data;
using SchoolSupplies.Models;
using SchoolSupplies.Models.Entities;
using SchoolSupplies.Services;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SchoolSupplies.Controllers
{
    public class AccessController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IEmailService _emailService;
        public AccessController(ApplicationDbContext dbcontext, IPasswordHasher<User> passwordHasher, IEmailService emailService)
        {
            _dbcontext = dbcontext;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
        }
        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (claimUser.Identity.IsAuthenticated)
                return RedirectToAction("Demo", "Home");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Hash the user input password using MD5
            string hashedPassword = GetMd5Hash(model.Password);
            var user = await _dbcontext.Users
                                .Include(u => u.Role)
                                .FirstOrDefaultAsync(u => u.UserName == model.UserName && u.Password == hashedPassword);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Invalid username or password";
                return View(model);
            }

            // Compare the hashed user input password with the password stored in the database
            if (user.Password != hashedPassword)
            {
                TempData["ErrorMessage"] = "Invalid username or password";
                return View(model);
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.UserEmail),
                new Claim(ClaimTypes.Role, user.Role?.RoleName) // Nếu có thông tin về vai trò
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            if (user.Role != null)
            {
                switch (user.Role.RoleName)
                {
                    case "Guest":
                        return RedirectToAction("Demo", "Home");
                    case "Staff":
                        return RedirectToAction("Dashboard", "DashBoardStaff");
                    case "Shop Owner":
                        return RedirectToAction("Dashboard", "DashBoardShopOwner");
                    default:
                        return RedirectToAction("Display", "NotFound");
                }
            }

            return RedirectToAction("Display", "NotFound");
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(AddUserViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            //Check if the email already exists
            var existingEmail = await _dbcontext.Users
                                   .FirstOrDefaultAsync(u => u.UserEmail == viewModel.UserEmail);
            if (existingEmail != null)
            {
                TempData["WarningMessage"] = "Email address already exists.";
                return View(viewModel);
            }
            //Check if the username already exists
            var existingUser = await _dbcontext.Users
                                   .FirstOrDefaultAsync(u => u.UserName == viewModel.UserName);
            if (existingUser != null)
            {
                TempData["WarningMessage"] = "Username already exists.";
                return View(viewModel);
            }
            //Take the Guest role
            var guestRole = await _dbcontext.Roles
                                            .Where(r => r.RoleName == "Guest")
                                            .FirstOrDefaultAsync();
            if (guestRole == null)
            {
                TempData["WarningMessage"] = "Guest role not found.";
                return View(viewModel);
            }
            //Create new user
            var user = new User
            {
                UserEmail = viewModel.UserEmail,
                UserName = viewModel.UserName,
                Gender = viewModel.Gender,
                RoleId = guestRole.RoleId
            };
            //Hash user password
            user.Password = GetMd5Hash(viewModel.Password);
            //Add new users to the database
            await _dbcontext.Users.AddAsync(user);
            await _dbcontext.SaveChangesAsync();
            return RedirectToAction("Login", "Access");
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Access");
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPassword model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.UserEmail == model.UserEmail);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Email address not found.";
                return View(model);
            }

            var token = Guid.NewGuid().ToString();
            user.ResetPasswordToken = token;
            user.ResetPasswordTokenExpiry = DateTime.UtcNow.AddHours(1);
            await _dbcontext.SaveChangesAsync();

            var callbackUrl = Url.Action("ResetPassword", "Access", new { token }, Request.Scheme);
            await _emailService.SendForgotPasswordEmailAsync(model.UserEmail, callbackUrl);

            TempData["SuccessMessage"] = "An email with instructions to reset your password has been sent to your email address.";
            return RedirectToAction("ForgotPassword", "Access");
        }


        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["WarningMessage"] = "A token must be supplied for password reset.";
            }

            var model = new ResetPassword { Token = token };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPassword model)
        {
            if (string.IsNullOrEmpty(model.Token))
            {
                TempData["WarningMessage"] = "A token must be supplied for password reset.";
                return View(model);
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.ResetPasswordToken == model.Token && u.ResetPasswordTokenExpiry > DateTime.UtcNow);
            if (user == null)
            {
                TempData["WarningMessage"] = "Invalid token or token expired.";
                return View(model);
            }
            user.Password = GetMd5Hash(model.Password);
            user.ResetPasswordToken = null;
            user.ResetPasswordTokenExpiry = null;
            await _dbcontext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your password has been reset successfully.";
            return RedirectToAction("Login");
        }

        private string GetMd5Hash(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Create a new StringBuilder to collect the bytes
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data and format each one as a hexadecimal string
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string
                return sBuilder.ToString();
            }
        }

    }
}
