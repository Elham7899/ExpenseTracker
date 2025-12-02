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

    private long GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (claim == null) throw new UnauthorizedAccessException("User ID not found");
        return long.Parse(claim);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();
        var transactions = await _transactionService.GetUserTransactionsAsync(userId);

        return Ok(new ApiResponse<IEnumerable<TransactionDto>>(
            _mapper.Map<IEnumerable<TransactionDto>>(transactions)
        ));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TransactionDto dto)
    {
        var userId = GetUserId();

        var transaction = _mapper.Map<Transaction>(dto);
        transaction.UserId = userId;

        await _transactionService.AddTransactionAsync(transaction);

        return Ok(new ApiResponse<TransactionDto>(
            _mapper.Map<TransactionDto>(transaction),
            "Transaction created successfully."
        ));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] TransactionDto dto)
    {
        var userId = GetUserId();
        var transaction = await _transactionService.GetTransactionByIdAsync(id);

        if (transaction == null || transaction.UserId != userId)
            return NotFound(ApiResponse<string>.Fail("Transaction not found or unauthorized."));

        _mapper.Map(dto, transaction);
        await _transactionService.UpdateTransactionAsync(transaction);

        return Ok(new ApiResponse<string>("Transaction updated successfully."));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var userId = GetUserId();
        var transaction = await _transactionService.GetTransactionByIdAsync(id);

        if (transaction == null || transaction.UserId != userId)
            return NotFound(ApiResponse<string>.Fail("Transaction not found or unauthorized."));

        await _transactionService.DeleteTransactionAsync(id);

        return Ok(new ApiResponse<string>("Transaction deleted successfully."));
    }

    [HttpGet("paged")]
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

        return Ok(new ApiResponse<PagedResult<TransactionDto>>(result));
    }
}