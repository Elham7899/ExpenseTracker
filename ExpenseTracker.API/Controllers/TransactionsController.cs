using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var transactions = await _transactionService.GetAllTransactionsAsync();
        return Ok(transactions);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(long userId)
    {
        var transactions = await _transactionService.GetUserTransactionsAsync(userId);
        return Ok(transactions);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Transaction transaction)
    {
        await _transactionService.AddTransactionAsync(transaction);
        return Ok(transaction);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, Transaction updatedTransaction)
    {
        updatedTransaction.Id = id;
        await _transactionService.UpdateTransactionAsync(updatedTransaction);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _transactionService.DeleteTransactionAsync(id);
        return NoContent();
    }
}