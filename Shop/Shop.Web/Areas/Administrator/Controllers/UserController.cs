using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;

namespace Shop.Web.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class UserController : Controller
    {
        private readonly ShopDbContext _context;
        public UserController(ShopDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        public IActionResult Edit(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(User model)
        {
            var user = _context.Users.Find(model.UserId);
            if (user == null) return NotFound();

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.Role = model.Role;
            user.IsActive = model.IsActive;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ToggleLock(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            user.IsActive = !user.IsActive.GetValueOrDefault();
            _context.SaveChanges();

            TempData["Message"] = user.IsActive == true ? "Tài khoản đã được mở khóa." : "Tài khoản đã bị khóa.";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                TempData["Message"] = "Không tìm thấy người dùng.";
                return RedirectToAction("Index");
            }

            // 1. Kiểm tra có đơn hàng đang xử lý không
            bool hasProcessingOrders = _context.Orders
                .Any(o => o.UserId == id && o.Status == "Đang xử lý");

            if (hasProcessingOrders)
            {
                TempData["Message"] = "❌ Không thể xóa vì người dùng còn đơn hàng đang xử lý.";
                return RedirectToAction("Index");
            }

            // 2. Lấy danh sách các OrderId của user
            var orderIds = _context.Orders
                .Where(o => o.UserId == id)
                .Select(o => o.OrderId)
                .ToList();

            // 3. Xóa các OrderDetails trước
            var orderDetails = _context.OrderDetails
                .Where(od => od.OrderId != null && orderIds.Contains(od.OrderId.Value))
                .ToList();

            if (orderDetails.Any())
                _context.OrderDetails.RemoveRange(orderDetails);

            // 4. Xóa các Order
            var orders = _context.Orders
                .Where(o => o.UserId == id)
                .ToList();

            if (orders.Any())
                _context.Orders.RemoveRange(orders);

            // 5. Xóa người dùng
            _context.Users.Remove(user);

            // 6. Lưu thay đổi
            _context.SaveChanges();

            TempData["Message"] = "✅ Đã xóa người dùng và toàn bộ dữ liệu đơn hàng liên quan.";
            return RedirectToAction("Index");
        }
    }

    }

