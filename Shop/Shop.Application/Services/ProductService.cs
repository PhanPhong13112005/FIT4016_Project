using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Application.DTOs;
using Shop.Application.Interfaces;
using Shop.Domain.Interfaces;
using Shop.Infrastructure;


namespace Shop.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _repository;
        public ProductService(IRepository<Product> repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<ProductDto>> GetAllProductAsync()
        {
            var products = await _repository.GetAllAsync();
            return products.Select(c => new ProductDto
            {
                ProductName = c.ProductName,
                Description = c.Description,
                ProductId = c.ProductId,
                Price = c.Price,
                ImageUrl = c.ImageUrl
            }).ToList();//Mapping thủ công
        }

        public async Task AddProductAsync(ProductDto productDto)
        {
            var product = new Product
            {
                ProductName = productDto.ProductName,
                Description = productDto.Description,
                ProductId = productDto.ProductId,
                Price = productDto.Price,
                ImageUrl = productDto.ImageUrl,
                ProductImages = productDto.ProductImages.Select(pi => new ProductImage
                {
                    ImageUrl = pi.ImageUrl
                }).ToList(),

            };
            await _repository.AddAsync(product);
            await _repository.SaveChangesAsync();
        }
        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var p = await _repository.GetByIdAsync(id);
            if (p == null) return null;

            return new ProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Description = p.Description,
                Price = p.Price,
                ImageUrl=p.ImageUrl,
                CategoryId = p.CategoryId
            };
        }

        public async Task<IEnumerable<ProductDto>> GetByCategoryIdAsync(int categoryId)
        {
            var products = await _repository.GetAllAsync();
            return products
                .Where(p => p.CategoryId == categoryId)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    CategoryId = p.CategoryId
                }).ToList();
        }

    }
}
