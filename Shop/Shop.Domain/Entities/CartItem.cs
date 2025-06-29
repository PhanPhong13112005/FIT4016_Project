using System;
using System.Collections.Generic;

namespace Shop.Infrastructure;

public partial class CartItem
{
    public int CartItemId { get; set; }

    public int ProductId { get; set; }

    public int UserId { get; set; }

    public int Quantity { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Product Product { get; set; } = null!;
}
