using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Services;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<IEnumerable<Category>> GetUserCategoriesAsync(long userId);
    Task<Category?> GetCategoryByIdAsync(long id);
    Task AddCategoryAsync(Category category);
    Task UpdateCategoryAsync(Category category);
    Task DeleteCategoryAsync(long id);
}