using System;
using System.Collections.Generic;

namespace Shop.Application.DTOs
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public int? UserId { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? Status { get; set; }
        public string? ShippingMethod { get; set; }
        public string? ShipAddress { get; set; }
        public List<OrderDetailDto> Items { get; set; } = new();
    }
}
