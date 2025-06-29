using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Domain.Interfaces;

namespace Shop.Application.DTOs
{
    public class CartItemDto
    {

        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public int Quantity { get; set; }

        public string? ProductName { get; set; }
        public decimal? Price { get; set; }


    }
}
