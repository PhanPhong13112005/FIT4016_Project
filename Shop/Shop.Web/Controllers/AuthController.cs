using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Shop.Web.Models;
using Shop.Infrastructure; // namespace chứa DbContext và User
using System.Linq;
using System.Threading.Tasks;
using Shop.Infrastructure.Data;

namespace Shop.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly ShopDbContext _context;

        public AuthController(ShopDbContext context)
        {
            _context = context;
        }

        // GET: /Auth/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = _context.Users
                .FirstOrDefault(u => u.Email == model.Email && u.PasswordHash == model.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Sai email hoặc mật khẩu");
                return View(model);
            }
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.FullName ?? user.Email);
            HttpContext.Session.SetString("Role", user.Role);

            return RedirectToAction("Index", "Home");
        }

        // GET: /Auth/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Auth/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var exists = _context.Users.Any(u => u.Email == model.Email);
            if (exists)
            {
                ModelState.AddModelError("", "Email đã được sử dụng");
                return View(model);
            }

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                PasswordHash = model.Password, // chưa mã hóa
                Role = "user",
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // tự động đăng nhập sau khi đăng ký
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.FullName ?? user.Email);
            HttpContext.Session.SetString("UserRole", user.Role?.ToLower());
            HttpContext.Session.SetString("UserRole", user.Role);




            return RedirectToAction("Index", "Home");
        }

        // GET: /Auth/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
