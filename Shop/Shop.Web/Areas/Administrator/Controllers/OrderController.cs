using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;
using System.Threading.Tasks;

namespace Shop.Web.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int orderId)
        {
            var order = await _orderService.GetByIdAsync(orderId);
            if (order == null)
            {
                TempData["Message"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("Manage");
            }

            order.Status = "Đang giao";
            await _orderService.UpdateAsync(order);

            TempData["Message"] = $"📦 Đơn hàng #{order.OrderId} đã được duyệt.";
            return RedirectToAction("Manage");
        }

        [HttpPost]
        public async Task<IActionResult> Complete(int orderId)
        {
            var order = await _orderService.GetByIdAsync(orderId);
            if (order == null)
            {
                TempData["Message"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("Manage");
            }

            order.Status = "Hoàn thành";
            await _orderService.UpdateAsync(order);

            TempData["Message"] = $"✅ Đơn hàng #{order.OrderId} đã hoàn thành.";
            return RedirectToAction("Manage");
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int orderId)
        {
            var order = await _orderService.GetByIdAsync(orderId);
            if (order == null)
            {
                TempData["Message"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("Manage");
            }

            order.Status = "Đã hủy";
            await _orderService.UpdateAsync(order);

            TempData["Message"] = $"❌ Đơn hàng #{order.OrderId} đã bị hủy.";
            return RedirectToAction("Manage");
        }

        public IActionResult Index()
        {
            return RedirectToAction("Manage");
        }
    }

}
