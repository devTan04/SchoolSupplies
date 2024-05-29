using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSupplies.Data;
using SchoolSupplies.Models;
using SchoolSupplies.Models.Entities;
using SchoolSupplies.Services;
using System.Security.Cryptography;
using System.Text;

namespace SchoolSupplies.Controllers
{
    [RoleAuthorization("Shop Owner")]
    public class DashBoardShopOwnerController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IEmailService _emailService;
        public DashBoardShopOwnerController(ApplicationDbContext dbcontext, IPasswordHasher<User> passwordHasher, IEmailService emailService)
        {
            _dbcontext = dbcontext;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
        }
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult RegisterStaff()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterStaff(AddUserViewModel viewModel)
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
                                            .Where(r => r.RoleName == "Staff")
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
            return RedirectToAction("Account", "DashboardShopOwner");
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
                                            .Where(r => r.RoleName == "Shop Owner")
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
            return RedirectToAction("Account", "DashboardShopOwner");
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

        public async Task<IActionResult> Account()
        {
            var users = await _dbcontext.Users
                .Include(u => u.Role)
                .Select(u => new AccountViewModel
                {
                    UserEmail = u.UserEmail,
                    UserName = u.UserName,
                    Gender = u.Gender,
                    RoleName = u.Role.RoleName
                })
                .ToListAsync();
            return View(users);
        }

    }
}
