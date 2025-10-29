using ExpenseTracker.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
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
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim))
            throw new UnauthorizedAccessException("User ID claim not found in token.");

        return long.Parse(userIdClaim);
    }

    [HttpGet("monthly-summaries")]
    public async Task<IActionResult> GetMonthlySummaries([FromQuery] int year, [FromQuery] int month)
    {
        var userId = GetUserId();
        var summary = await _transactionService.GetMonthlyCategorySummaryAsync(userId, year, month);
        return Ok(summary);
    }

    [HttpGet("monthly-summary")]
    public async Task<IActionResult> GetMonthlySummary([FromQuery] int year, [FromQuery] int month)
    {
        var userId = GetUserId();
        var summary = await _transactionService.GetMonthlySummaryAsync(userId, year, month);
        return Ok(summary);
    }

    [HttpGet("yearly-summary")]
    public async Task<IActionResult> GetYearlySummary([FromQuery] int year)
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var summary = await _transactionService.GetYearlySummaryAsync(userId, year);
        return Ok(summary);
    }
}