using AutoMapper;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService userService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await userService.GetAllUsersAsync();
        var usersDto = mapper.Map<IEnumerable<UserDto>>(users);
        return Ok(usersDto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var user = await userService.GetUserByIdAsync(id);
        if (user == null) return NotFound();

        var userDto = mapper.Map<UserDto>(user);
        return Ok(userDto);
    }

    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetByEmail(string email)
    {
        var user = await userService.GetUserByEmailAsync(email);
        if (user == null) return NotFound();

        var userDto = mapper.Map<UserDto>(user);
        return Ok(userDto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserDto userDto)
    {
        var user = mapper.Map<User>(userDto);

        await userService.AddUserAsync(user);
        return Ok(mapper.Map<UserDto>(user));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UserDto userDto)
    {
        var user = mapper.Map<User>(userDto);
        user.Id = id;

        await userService.UpdateUserAsync(user);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await userService.DeleteUserAsync(id);
        return NoContent();
    }
}