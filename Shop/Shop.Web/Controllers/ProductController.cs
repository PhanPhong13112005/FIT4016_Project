using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;
using Shop.Web.Models;

namespace Shop.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<IActionResult> AllProduct(string? searchTerm, decimal? minPrice, decimal? maxPrice, string? sortBy)
        {
            var allProducts = await _productService.GetAllProductAsync();

            // Tìm kiếm theo tên
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                allProducts = allProducts.Where(p => p.ProductName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            // Lọc theo giá
            if (minPrice.HasValue)
                allProducts = allProducts.Where(p => p.Price >= minPrice);
            if (maxPrice.HasValue)
                allProducts = allProducts.Where(p => p.Price <= maxPrice);

            // Sắp xếp
            allProducts = sortBy switch
            {
                "priceAsc" => allProducts.OrderBy(p => p.Price),
                "priceDesc" => allProducts.OrderByDescending(p => p.Price),
                "mostPurchased" => allProducts.OrderByDescending(p => p.TotalSold), // cần có TotalSold
                _ => allProducts
            };

            var viewModel = new ProductFilterViewModel
            {
                SearchTerm = searchTerm,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SortBy = sortBy,
                Products = allProducts
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        public async Task<IActionResult> ListByCategory(int categoryId)
        {
            var products = await _productService.GetByCategoryIdAsync(categoryId);
            ViewBag.CategoryId = categoryId; // nếu muốn hiển thị tên danh mục thì truyền thêm
            return View(products);
        }

    }
}
