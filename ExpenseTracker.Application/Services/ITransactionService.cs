using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Services;

public interface ITransactionService
{
    Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
    Task<IEnumerable<Transaction>> GetUserTransactionsAsync(long userId);
    Task<Transaction?> GetTransactionByIdAsync(long id);
    Task AddTransactionAsync(Transaction transaction);
    Task UpdateTransactionAsync(Transaction transaction);
    Task DeleteTransactionAsync(long id);
    Task<IEnumerable<CategorySummaryDto>> GetMonthlyCategorySummaryAsync(long userId, int year, int month);
}