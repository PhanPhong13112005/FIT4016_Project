using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Application.Interfaces;
using Shop.Domain.Interfaces;
using Shop.Application.DTOs;
using Shop.Infrastructure;

namespace Shop.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _repository;
        public CategoryService(IRepository<Category> repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync() 
        {
            var categories = await _repository.GetAllAsync();
            return categories.Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
            }).ToList();//Mapping thủ công
        }   

        public async Task AddCategoryAsync(CategoryDto categoryDto)
        {
            var category = new Category
            {
                CategoryId = categoryDto.CategoryId,
                CategoryName = categoryDto.CategoryName,
                Description = categoryDto.Description,
                ImageUrl = categoryDto.ImageUrl
            };
            await _repository.AddAsync(category);
            await _repository.SaveChangesAsync();
        }

    }
}
