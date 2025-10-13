using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController(ITransactionService transactionService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var transactions = await transactionService.GetAllTransactionsAsync();
        return Ok(transactions);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(long userId)
    {
        var transactions = await transactionService.GetUserTransactionsAsync(userId);
        return Ok(transactions);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Transaction transaction)
    {
        await transactionService.AddTransactionAsync(transaction);
        return Ok(transaction);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, Transaction updatedTransaction)
    {
        updatedTransaction.Id = id;
        await transactionService.UpdateTransactionAsync(updatedTransaction);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await transactionService.DeleteTransactionAsync(id);
        return NoContent();
    }
}