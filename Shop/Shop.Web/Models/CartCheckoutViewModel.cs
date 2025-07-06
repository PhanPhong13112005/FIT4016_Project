using Shop.Application.DTOs;

namespace Shop.Web.Models
{
        public class CartCheckoutViewModel
        {
            public List<CartItemDto> CartItems { get; set; } = new();

        public string ShipAddress { get; set; }
        public string ShippingMethod { get; set; }
        public string PaymentMethod { get; set; }
        }

}