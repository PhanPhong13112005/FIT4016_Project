﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Infrastructure;

namespace Shop.Application.DTOs
{
    public class ProductDto
    {
        public int ProductId { get; set; }

        public string? ProductName { get; set; }

        public string? Description { get; set; }

        public decimal? Price { get; set; }

        public int? Quantity { get; set; }

        public int? CategoryId { get; set; }

        public DateTime? CreatedAt { get; set; }
        public string? ImageUrl { get; set; }

        public virtual Category? Category { get; set; }
        public int TotalSold { get; set; }


        public virtual ICollection<OrderDetail> OrderDetails { get; set; }


        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    }
}
