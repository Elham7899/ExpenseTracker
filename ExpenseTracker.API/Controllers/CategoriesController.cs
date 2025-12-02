using AutoMapper;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ICategoryService categoryService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await categoryService.GetAllCategoriesAsync();
        return Ok(new ApiResponse<IEnumerable<CategoryDto>>(
            mapper.Map<IEnumerable<CategoryDto>>(categories)
        ));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var category = await categoryService.GetCategoryByIdAsync(id);
        if (category == null)
            return NotFound(ApiResponse<string>.Fail("Category not found."));

        return Ok(new ApiResponse<CategoryDto>(mapper.Map<CategoryDto>(category)));
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(long userId)
    {
        var categories = await categoryService.GetUserCategoriesAsync(userId);
        return Ok(new ApiResponse<IEnumerable<CategoryDto>>(
            mapper.Map<IEnumerable<CategoryDto>>(categories)
        ));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategoryDto dto)
    {
        var category = mapper.Map<Category>(dto);
        await categoryService.AddCategoryAsync(category);

        return Ok(new ApiResponse<CategoryDto>(
            mapper.Map<CategoryDto>(category),
            "Category created successfully."
        ));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] CategoryDto dto)
    {
        var category = mapper.Map<Category>(dto);
        category.Id = id;

        await categoryService.UpdateCategoryAsync(category);

        return Ok(new ApiResponse<string>("Category updated successfully."));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await categoryService.DeleteCategoryAsync(id);
        return Ok(new ApiResponse<string>("Category deleted successfully."));
    }
}