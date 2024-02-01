using Bank.Data;
using Bank.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Runtime.Intrinsics.X86;
using Microsoft.EntityFrameworkCore;

namespace Bank.Controllers
{
    public class HomeController : Controller
    {
        private readonly dbcontext _context;
        public HomeController(dbcontext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(Employees employees)
        {
            _context.Employees.Add(employees);
            _context.SaveChanges();
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        public async Task<IActionResult> Login(Employees loginPage)
        {
            var user = await _context.Employees
                .FirstOrDefaultAsync(u => u.Email == loginPage.Email && u.Password == loginPage.Password);

            if (user != null)
            {
                //var userRole = await _context.userRoles
                //    .Include(ur => ur.Role)
                //    .FirstOrDefaultAsync(ur => ur.UserId == user.Id);

                //if (userRole != null)
                //{
                //var roleName = userRole.Role.RoleName;

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Email),
                new Claim(ClaimTypes.Sid, Convert.ToString(user?.Id)),
                // Add other claims as needed
            };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = true,
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                return View();
            }
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}