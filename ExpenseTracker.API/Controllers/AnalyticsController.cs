using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public AnalyticsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    private long GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (claim == null) throw new UnauthorizedAccessException("User ID not found");
        return long.Parse(claim);
    }

    [HttpGet("monthly/categories")]
    public async Task<IActionResult> GetMonthlyCategorySummary(int year, int month)
    {
        var userId = GetUserId();
        var summary = await _transactionService.GetMonthlyCategorySummaryAsync(userId, year, month);

        return Ok(new ApiResponse<object>(summary));
    }

    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthlySummary(int year, int month)
    {
        var userId = GetUserId();
        var summary = await _transactionService.GetMonthlySummaryAsync(userId, year, month);

        return Ok(new ApiResponse<object>(summary));
    }

    [HttpGet("yearly")]
    public async Task<IActionResult> GetYearlySummary(int year)
    {
        var userId = GetUserId();
        var summary = await _transactionService.GetYearlySummaryAsync(userId, year);

        return Ok(new ApiResponse<object>(summary));
    }
}