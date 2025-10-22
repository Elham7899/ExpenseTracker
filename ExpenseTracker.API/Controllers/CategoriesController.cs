using AutoMapper;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController(ICategoryService categoryService,IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await categoryService.GetAllCategoriesAsync();
        var categoriesDto = mapper.Map<IEnumerable<CategoryDto>>(categories);
        return Ok(categoriesDto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var category = await categoryService.GetCategoryByIdAsync(id);
        if (category == null) return NotFound();

        var categoryDto = mapper.Map<CategoryDto>(category);
        return Ok(categoryDto);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(long userId)
    {
        var categories = await categoryService.GetUserCategoriesAsync(userId);

        var categoriesDto = mapper.Map<IEnumerable<CategoryDto>>(categories);
        return Ok(categoriesDto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody]CategoryDto categoryDto)
    {
        var category = mapper.Map<Category>(categoryDto);

        await categoryService.AddCategoryAsync(category);

        return Ok(mapper.Map<CategoryDto>(category));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id,[FromBody] CategoryDto categoryDto)
    {
        var category = mapper.Map<Category>(categoryDto);
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