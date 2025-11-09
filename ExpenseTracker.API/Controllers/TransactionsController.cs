using AutoMapper;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.DTOs.Pagination;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly IMapper _mapper;

    public TransactionsController(ITransactionService transactionService, IMapper mapper)
    {
        _transactionService = transactionService;
        _mapper = mapper;
    }

    /// <summary>
    /// Extracts the logged-in user ID from the JWT token.
    /// </summary>
    /// <returns>User ID as long</returns>
    /// <exception cref="UnauthorizedAccessException"></exception>
    private long GetUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userIdClaim))
            throw new UnauthorizedAccessException("User ID claim not found in token.");

        return long.Parse(userIdClaim);
    }

    /// <summary>
    /// Retrieves all transactions belonging to the authenticated user.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TransactionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();
        var transactions = await _transactionService.GetUserTransactionsAsync(userId);
        return Ok(_mapper.Map<IEnumerable<TransactionDto>>(transactions));
    }

    /// <summary>
    /// Creates a new transaction for the authenticated user.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] TransactionDto dto)
    {
        var userId = GetUserId();

        var transaction = _mapper.Map<Transaction>(dto);
        transaction.UserId = userId;

        await _transactionService.AddTransactionAsync(transaction);

        return Ok(_mapper.Map<TransactionDto>(transaction));
    }

    /// <summary>
    /// Updates a specific transaction if it belongs to the authenticated user.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update(long id, [FromBody] TransactionDto dto)
    {
        var userId = GetUserId();
        var transaction = await _transactionService.GetTransactionByIdAsync(id);

        if (transaction == null || transaction.UserId != userId)
            return NotFound("Transaction not found or unauthorized.");

        _mapper.Map(dto, transaction);
        await _transactionService.UpdateTransactionAsync(transaction);

        return Ok("Transaction updated successfully.");
    }

    /// <summary>
    /// Deletes a specific transaction if it belongs to the authenticated user.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(long id)
    {
        var userId = GetUserId();
        var transaction = await _transactionService.GetTransactionByIdAsync(id);

        if (transaction == null || transaction.UserId != userId)
            return NotFound("Transaction not found or unauthorized.");

        await _transactionService.DeleteTransactionAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Retrieves a paginated, filtered, and sorted list of user transactions.
    /// Supports filtering by date range and category, and sorting by amount or date.
    /// </summary>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResult<TransactionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPaged(
        int page = 1,
        int pageSize = 10,
        string? sortBy = "date",
        bool ascending = false,
        string? categoryFilter = null,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        var userId = GetUserId();

        var (transactions, totalCount) = await _transactionService.GetPagedUserTransactionsAsync(
            userId, page, pageSize, sortBy, ascending, categoryFilter, fromDate, toDate);

        var result = new PagedResult<TransactionDto>
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Items = _mapper.Map<IEnumerable<TransactionDto>>(transactions)
        };

        return Ok(result);
    }
}