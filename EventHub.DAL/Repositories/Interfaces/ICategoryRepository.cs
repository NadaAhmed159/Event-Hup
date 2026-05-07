using EventHub.Domain.Entities;

namespace EventHub.DAL.Repositories.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<Category?> GetByNameAsync(string name);
        Task<IEnumerable<Category>> GetWithEventCountAsync();
    }
}
