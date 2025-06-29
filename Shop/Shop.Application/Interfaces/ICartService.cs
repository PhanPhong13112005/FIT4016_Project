using Shop.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Application.Interfaces
{
    public interface ICartService
    {
        Task AddToCartAsync(int userId, int productId, int quantity);
        Task<List<CartItemDto>> GetCartItemsAsync(int userId);
        Task RemoveFromCartAsync(int cartItemId);

        // ① Trả về OrderId vừa tạo
        Task<int> CheckoutAsync(int userId);
    }
}
