using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using Shop.Web.Models;

namespace Shop.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;

        public CartController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }

        // Hiển thị giỏ hàng
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var cartItems = await _cartService.GetCartItemsAsync(userId.Value);
            return View(cartItems);
        }

        // Thêm sản phẩm vào giỏ
        [HttpPost]
        public async Task<IActionResult> Add(int productId, int quantity = 1)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Message"] = "Bạn cần đăng nhập để thêm vào giỏ hàng.";
                return RedirectToAction("Login", "Auth");
            }

            TempData["Message"] = $"🛒 Đang thêm ProductId={productId}, Quantity={quantity} cho UserId={userId}";

            await _cartService.AddToCartAsync(userId.Value, productId, quantity);
            return RedirectToAction(nameof(Index));
        }

        // Xóa sản phẩm khỏi giỏ
        public async Task<IActionResult> Remove(int cartItemId)
        {
            await _cartService.RemoveFromCartAsync(cartItemId);
            return RedirectToAction(nameof(Index));
        }

        // Thanh toán giỏ hàng và chuyển đến trang thành công
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var cartItems = await _cartService.GetCartItemsAsync(userId.Value);
            if (!cartItems.Any()) return RedirectToAction(nameof(Index));

            var model = new CartCheckoutViewModel
            {
                CartItems = cartItems
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(CartCheckoutViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var cartItems = await _cartService.GetCartItemsAsync(userId.Value);
            if (!cartItems.Any()) return RedirectToAction(nameof(Index));

            // ✅ Gọi đúng hàm Checkout với 4 tham số
            int orderId = await _cartService.CheckoutAsync(
                userId.Value,
                model.ShipAddress,
                model.ShippingMethod,
                model.PaymentMethod
            );

            return RedirectToAction(nameof(CheckoutSuccess), new { orderId });
        }




        // Hiển thị trang thanh toán thành công
        public async Task<IActionResult> CheckoutSuccess(int orderId)
        {
            var order = await _orderService.GetByIdAsync(orderId);
            if (order == null)
                return NotFound();

            return View(order);
        }
    }
}
