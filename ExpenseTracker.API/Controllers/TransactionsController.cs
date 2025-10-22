using AutoMapper;
using ExpenseTracker.Application.DTOs;
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
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim))
            throw new UnauthorizedAccessException("User ID claim not found in token.");

        return long.Parse(userIdClaim);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserTransactions()
    {
        var userId = GetUserId();
        var transactions = await _transactionService.GetUserTransactionsAsync(userId);
        return Ok(_mapper.Map<IEnumerable<TransactionDto>>(transactions));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TransactionDto dto)
    {
        var userId = GetUserId();
        var transaction = _mapper.Map<Transaction>(dto);
        transaction.UserId = userId;

        await _transactionService.AddTransactionAsync(transaction);
        return Ok(_mapper.Map<TransactionDto>(transaction));
    }

    [HttpPut("{id}")]
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var userId = GetUserId();
        var transaction = await _transactionService.GetTransactionByIdAsync(id);

        if (transaction == null || transaction.UserId != userId)
            return NotFound("Transaction not found or unauthorized.");

        await _transactionService.DeleteTransactionAsync(id);
        return NoContent();
    }
}