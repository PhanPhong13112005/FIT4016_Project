using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using System;
using System.Linq;

namespace Shop.Web.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class ProductController : Controller
    {

        private readonly ShopDbContext _context;
        public ProductController(ShopDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? categoryId, bool? status, decimal? minPrice, decimal? maxPrice)
        {
            var products = _context.Products
                .Include(p => p.Category)
                .Where(p => !p.IsDeleted)
                .AsQueryable();

            if (categoryId.HasValue)
                products = products.Where(p => p.CategoryId == categoryId);

            if (status.HasValue)
                products = products.Where(p => p.IsActive == status);

            if (minPrice.HasValue)
                products = products.Where(p => p.Price >= minPrice);

            if (maxPrice.HasValue)
                products = products.Where(p => p.Price <= maxPrice);

            ViewBag.Categories = _context.Categories.ToList();
            return View(products.ToList());
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product model, IFormFileCollection images)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.Now;
                model.IsDeleted = false;
                model.IsActive = true;
                _context.Products.Add(model);
                _context.SaveChanges(); // Lúc này mới có ProductId

                // Xử lý ảnh
                if (images != null && images.Count > 0)
                {
                    foreach (var file in images)
                    {
                        if (file.Length > 0)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                            using (var stream = new FileStream(savePath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }

                            var image = new ProductImage
                            {
                                ProductId = model.ProductId,
                                ImageUrl = "/uploads/" + fileName,
                                IsMain = false
                            };

                            _context.ProductImages.Add(image);
                        }
                    }

                    // Gán ảnh đầu tiên làm ảnh chính
                    var firstImage = _context.ProductImages.FirstOrDefault(p => p.ProductId == model.ProductId);
                    if (firstImage != null)
                    {
                        firstImage.IsMain = true;
                    }

                    _context.SaveChanges();
                }

                TempData["Message"] = "✅ Thêm sản phẩm và ảnh thành công.";
                return RedirectToAction("Index");
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(model);
        }


        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();
            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product model, List<int>? RemoveImageIds, int? MainImageId, string? NewImageUrls)
        {
            var product = _context.Products
            .Include(p => p.ProductImages)
            .FirstOrDefault(p => p.ProductId == model.ProductId);
            if (product == null) return NotFound();

            // Cập nhật thông tin cơ bản
            product.ProductName = model.ProductName;
            product.Description = model.Description;
            product.Price = model.Price;
            product.CategoryId = model.CategoryId;
            product.IsActive = model.IsActive;

            // Xoá ảnh được chọn
            if (RemoveImageIds != null && RemoveImageIds.Count > 0)
            {
                var imagesToRemove = product.ProductImages
                    .Where(i => RemoveImageIds.Contains(i.ProductImageId))
                    .ToList();

                _context.ProductImages.RemoveRange(imagesToRemove);
            }

            // Cập nhật ảnh chính
            foreach (var img in product.ProductImages)
            {
                img.IsMain = (img.ProductImageId == MainImageId);
            }

            // Thêm ảnh mới
            if (!string.IsNullOrWhiteSpace(NewImageUrls))
            {
                var urls = NewImageUrls.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var url in urls)
                {
                    if (!string.IsNullOrWhiteSpace(url))
                    {
                        product.ProductImages.Add(new ProductImage
                        {
                            ImageUrl = url,
                            IsMain = false
                        });
                    }
                }
            }

            _context.SaveChanges();
            TempData["Message"] = "✅ Cập nhật sản phẩm thành công.";
            return RedirectToAction("Index");
        }


        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                product.IsDeleted = true;
                _context.SaveChanges();
            }

            TempData["Message"] = "🗑️ Đã ẩn sản phẩm.";
            return RedirectToAction("Index");
        }

    }
}