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

    private long GetUserId() => long.Parse(User.FindFirstValue("sub")!);

    [HttpGet("monthly-summary")]
    public async Task<IActionResult> GetMonthlySummary([FromQuery] int year, [FromQuery] int month)
    {
        var userId = GetUserId();
        var summary = await _transactionService.GetMonthlyCategorySummaryAsync(userId, year, month);
        return Ok(summary);
    }
}