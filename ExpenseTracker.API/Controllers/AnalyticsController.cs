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

    /// <summary>
    /// Retrieves the authenticated user's ID from the JWT token.
    /// </summary>
    private long GetUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userIdClaim))
            throw new UnauthorizedAccessException("User ID not found in token.");

        return long.Parse(userIdClaim);
    }

    /// <summary>
    /// Returns a breakdown of spending per category for the specified month and year.
    /// </summary>
    /// <param name="year">Year (e.g., 2025)</param>
    /// <param name="month">Month (1-12)</param>
    [HttpGet("monthly/categories")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMonthlyCategorySummary(int year, int month)
    {
        var userId = GetUserId();
        var summary = await _transactionService.GetMonthlyCategorySummaryAsync(userId, year, month);
        return Ok(summary);
    }

    /// <summary>
    /// Returns full analytics for a given month, including total income,
    /// total expenses, top categories, and comparison with the previous month.
    /// </summary>
    [HttpGet("monthly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMonthlySummary(int year, int month)
    {
        var userId = GetUserId();
        var summary = await _transactionService.GetMonthlySummaryAsync(userId, year, month);
        return Ok(summary);
    }

    /// <summary>
    /// Returns a yearly analytics summary including total income and total expenses.
    /// </summary>
    [HttpGet("yearly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetYearlySummary(int year)
    {
        var userId = GetUserId();
        var summary = await _transactionService.GetYearlySummaryAsync(userId, year);
        return Ok(summary);
    }
}