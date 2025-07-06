using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shop.Application.DTOs;
using Shop.Application.Interfaces;
using Shop.Domain.Interfaces;
using Shop.Infrastructure;

namespace Shop.Application.Services
{
    public class CartService : ICartService
    {
        private readonly IRepository<CartItem> _cartRepo;
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<Order> _orderRepo;
        private readonly IRepository<OrderDetail> _orderDetailRepo;
        private readonly IRepository<Payment> _paymentRepo;


        public CartService(
            IRepository<CartItem> cartRepo,
            IRepository<Product> productRepo,
            IRepository<Order> orderRepo,
            IRepository<OrderDetail> orderDetailRepo,
            IRepository<Payment> paymentRepo    
            )

        {
            _cartRepo = cartRepo;
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _orderDetailRepo = orderDetailRepo;
            _paymentRepo = paymentRepo;
        }

        public async Task AddToCartAsync(int userId, int productId, int quantity)
        {
            try
            {
                var existing = (await _cartRepo.GetAllAsync())
                    .FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

                if (existing != null)
                {
                    existing.Quantity += quantity;
                    _cartRepo.Update(existing);
                }
                else
                {
                    await _cartRepo.AddAsync(new CartItem { UserId = userId, ProductId = productId, Quantity = quantity });
                }

                await _cartRepo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi khi thêm vào giỏ: " + ex.Message);
                throw;
            }
        }

        public async Task<List<CartItemDto>> GetCartItemsAsync(int userId)
        {
            var items = (await _cartRepo.GetAllAsync()).Where(c => c.UserId == userId).ToList();
            var products = await _productRepo.GetAllAsync();
            var productDict = products.ToDictionary(p => p.ProductId);

            return items.Select(i => new CartItemDto
            {
                CartItemId = i.CartItemId,
                UserId = i.UserId,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                ProductName = productDict.ContainsKey(i.ProductId) ? productDict[i.ProductId].ProductName : null,
                Price = productDict.ContainsKey(i.ProductId) ? productDict[i.ProductId].Price : null
            }).ToList();
        }

        public async Task RemoveFromCartAsync(int cartItemId)
        {
            var item = await _cartRepo.GetByIdAsync(cartItemId);
            if (item != null)
            {
                _cartRepo.Delete(item);
                await _cartRepo.SaveChangesAsync();
            }
        }

        public async Task<int> CheckoutAsync(int userId, string shipAddress, string shippingMethod, string paymentMethod)
        {
            var cartItems = (await _cartRepo.GetAllAsync())
                .Where(c => c.UserId == userId).ToList();

            if (!cartItems.Any()) return -1;

            var products = await _productRepo.GetAllAsync();
            var productDict = products.ToDictionary(p => p.ProductId);

            decimal total = cartItems.Sum(ci =>
                ci.Quantity * (productDict.ContainsKey(ci.ProductId) ? productDict[ci.ProductId].Price ?? 0 : 0));

            // Tạo đơn hàng
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = total,
                ShipAddress = shipAddress,
                Status = "Đang xử lý"
            };
            await _orderRepo.AddAsync(order);
            await _orderRepo.SaveChangesAsync();

            // Chi tiết đơn hàng
            foreach (var ci in cartItems)
            {
                if (!productDict.ContainsKey(ci.ProductId)) continue;

                await _orderDetailRepo.AddAsync(new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = productDict[ci.ProductId].Price ?? 0
                });

                // Xóa khỏi giỏ hàng
                _cartRepo.Delete(ci);
            }

            await _orderDetailRepo.SaveChangesAsync();
            await _cartRepo.SaveChangesAsync();

            // Thêm thông tin thanh toán
            var payment = new Payment
            {
                OrderId = order.OrderId,
                PaymentDate = DateTime.Now,
                Amount = total,
                PaymentMethod = paymentMethod
            };

            if (_paymentRepo == null)
                throw new NullReferenceException("_paymentRepo chưa được inject vào CartService.");

            await _paymentRepo.AddAsync(payment);
            await _paymentRepo.SaveChangesAsync();

            return order.OrderId;
        }



    }
}