using AutoMapper;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController(ITransactionService transactionService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var transactions = await transactionService.GetAllTransactionsAsync();
        var transactionDtos = mapper.Map<IEnumerable<TransactionDto>>(transactions);
        return Ok(transactionDtos);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(long userId)
    {
        var transaction = await transactionService.GetUserTransactionsAsync(userId);
        if (transaction == null) return NotFound();

        var transactionDto = mapper.Map<TransactionDto>(transaction);
        return Ok(transactionDto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TransactionDto transactionDto)
    {
        var transaction = mapper.Map<Transaction>(transactionDto);
        await transactionService.AddTransactionAsync(transaction);
        return Ok(mapper.Map<TransactionDto>(transaction));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] TransactionDto transactionDto)
    {
        var transaction = mapper.Map<Transaction>(transactionDto);
        transaction.Id = id;
        await transactionService.UpdateTransactionAsync(transaction);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await transactionService.DeleteTransactionAsync(id);
        return NoContent();
    }
}