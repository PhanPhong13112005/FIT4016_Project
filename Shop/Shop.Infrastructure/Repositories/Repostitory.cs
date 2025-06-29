using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.Interfaces;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class Repostitory<T> : IRepository<T> where T : class
    {
        private readonly ShopDbContext _context;
        private readonly DbSet<T> _dbSet;
        public Repostitory(ShopDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
        public void Update(T entity) => _dbSet.Update(entity);
        public void Delete(T entity) => _dbSet.Remove(entity);
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }   
}
