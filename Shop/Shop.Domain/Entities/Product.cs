using System;
using System.Collections.Generic;

namespace Shop.Infrastructure;

public partial class Product
{
    public int ProductId { get; set; }

    public string? ProductName { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public int? Quantity { get; set; }

    public int? CategoryId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsActive { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
}
