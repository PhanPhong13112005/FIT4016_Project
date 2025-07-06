using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;
using Shop.Web.Models;

namespace Shop.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;

        public HomeController(ILogger<HomeController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }


        public async Task<IActionResult> Index()
        {
            var allProducts = await _productService.GetAllProductAsync();
            var top4Products = allProducts.Take(4); // chỉ lấy 4 sản phẩm đầu tiên
            return View(top4Products);
        }
    }
}
