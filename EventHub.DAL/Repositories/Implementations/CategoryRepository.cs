using EventHub.DAL.Data;
using EventHub.DAL.Repositories.Interfaces;
using EventHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventHub.DAL.Repositories.Implementations
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context) { }

        public async Task<Category?> GetByNameAsync(string name) =>
            await _dbSet.FirstOrDefaultAsync(c => c.Name == name);

        public async Task<IEnumerable<Category>> GetWithEventCountAsync() =>
            await _dbSet
                .Include(c => c.Events)
                .OrderBy(c => c.Name)
                .ToListAsync();
    }
}
