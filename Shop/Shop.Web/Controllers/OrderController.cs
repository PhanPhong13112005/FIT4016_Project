using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;

namespace Shop.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // Hiển thị chi tiết đơn hàng theo ID
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            return View(order);
        }

        // Hiển thị danh sách đơn hàng của người dùng đang đăng nhập
        public async Task<IActionResult> OrderDetails()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var orders = await _orderService.GetOrdersByUserIdAsync(userId.Value);
            return View(orders);
        }

        // Hủy đơn hàng (chỉ nếu đơn thuộc về người dùng hiện tại)
        [HttpGet]
        public async Task<IActionResult> Cancel(int orderId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Message"] = "Vui lòng đăng nhập để hủy đơn.";
                return RedirectToAction("Login", "Auth");
            }

            var order = await _orderService.GetByIdAsync(orderId);
            if (order == null)
            {
                TempData["Message"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("OrderDetails");
            }


            order.Status = "Đã hủy";
            await _orderService.UpdateAsync(order);

            TempData["Message"] = $"✅ Đơn hàng #{orderId} đã được hủy.";
            return RedirectToAction("OrderDetails");
        }



    }
}
