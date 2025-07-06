using Shop.Application.DTOs;

namespace Shop.Web.Models
{
    public class ProductFilterViewModel
    {
        public string? SearchTerm { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; } // "mostPurchased", "priceAsc", "priceDesc"

        public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();
    }

}
