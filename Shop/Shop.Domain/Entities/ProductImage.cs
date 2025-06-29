using System;
using System.Collections.Generic;

namespace Shop.Infrastructure;

public partial class ProductImage
{
    public int ProductImageId { get; set; }

    public int ProductId { get; set; }

    public string? ImageUrl { get; set; }

    public bool? IsMain { get; set; }

    public virtual Product Product { get; set; } = null!;
}
