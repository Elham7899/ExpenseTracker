using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        => await _categoryRepository.GetAllAsync();

    public async Task<IEnumerable<Category>> GetUserCategoriesAsync(long userId)
        => await _categoryRepository.GetUserCategoriesAsync(userId);

    public async Task<Category?> GetCategoryByIdAsync(long id)
        => await _categoryRepository.GetByIdAsync(id);

    public async Task AddCategoryAsync(Category category)
    {
        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();
    }

    public async Task UpdateCategoryAsync(Category category)
    {
        _categoryRepository.Update(category);
        await _categoryRepository.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(long id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category != null)
        {
            _categoryRepository.Remove(category);
            await _categoryRepository.SaveChangesAsync();
        }
    }
}