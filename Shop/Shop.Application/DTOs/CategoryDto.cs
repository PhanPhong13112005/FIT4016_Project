using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Infrastructure;

namespace Shop.Application.DTOs
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
