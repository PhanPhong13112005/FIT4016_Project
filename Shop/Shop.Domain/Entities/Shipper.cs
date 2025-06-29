using System;
using System.Collections.Generic;

namespace Shop.Infrastructure;

public partial class Shipper
{
    public int ShipperId { get; set; }

    public string? CompanyName { get; set; }

    public string? Phone { get; set; }
}
