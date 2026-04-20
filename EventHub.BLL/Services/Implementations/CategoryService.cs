using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.BLL.Services.Interfaces;
using EventHub.DAL.Repositories.Interfaces;
using EventHub.Domain.Entities;

namespace EventHub.BLL.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();
            return category;
        }

        public async Task DeleteCategoryAsync(string id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category != null)
            {
                _unitOfWork.Categories.Remove(category);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _unitOfWork.Categories.GetAllAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(string id)
        {
            return await _unitOfWork.Categories.GetByIdAsync(id);
        }

        public async Task<Category?> GetCategoryByNameAsync(string name)
        {
            return await _unitOfWork.Categories.GetByNameAsync(name);
        }

        public async Task<IEnumerable<Category>> GetCategoriesWithEventCountAsync()
        {
            return await _unitOfWork.Categories.GetWithEventCountAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
