using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.Domain.Entities;

namespace EventHub.BLL.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<Category?> GetCategoryByIdAsync(string id);
        Task<Category?> GetCategoryByNameAsync(string name);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<IEnumerable<Category>> GetCategoriesWithEventCountAsync();
        Task<Category> CreateCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(string id);
    }
}
