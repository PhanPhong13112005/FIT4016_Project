using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Application.DTOs;

namespace Shop.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductAsync();
        Task AddProductAsync(ProductDto productDto);
        Task<ProductDto?> GetByIdAsync(int id);
        Task<IEnumerable<ProductDto>> GetByCategoryIdAsync(int categoryId);

    }
}
