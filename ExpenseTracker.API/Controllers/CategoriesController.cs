using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var category = await categoryService.GetCategoryByIdAsync(id);
        if (category == null) return NotFound();
        return Ok(category);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(long userId)
    {
        var categories = await categoryService.GetUserCategoriesAsync(userId);
        return Ok(categories);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Category category)
    {
        await categoryService.AddCategoryAsync(category);
        return Ok(category);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, Category category)
    {
        category.Id = id;
        await categoryService.UpdateCategoryAsync(category);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await categoryService.DeleteCategoryAsync(id);
        return NoContent();
    }
}