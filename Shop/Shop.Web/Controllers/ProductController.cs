using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;

namespace Shop.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<IActionResult> AllProduct()
        {
            var product = await _productService.GetAllProductAsync();
            return View(product);
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
