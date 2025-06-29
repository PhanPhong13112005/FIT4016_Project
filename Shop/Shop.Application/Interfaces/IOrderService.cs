using System.Collections.Generic;
using System.Threading.Tasks;
using Shop.Application.DTOs;

namespace Shop.Application.Interfaces
{
    public interface IOrderService
    {
        // 1. Người dùng
        Task<List<OrderDto>> GetOrdersByUserIdAsync(int userId);
        Task<OrderDto?> GetByIdAsync(int id);
        Task UpdateAsync(OrderDto orderDto);

        // 2. Quản lý Admin
        Task<List<OrderDto>> GetAllOrdersAsync();                   // Xem toàn bộ đơn hàng
        Task UpdateOrderStatusAsync(int orderId, string status);    // Cập nhật trạng thái đơn hàng
    }
}
