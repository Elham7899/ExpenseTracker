using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Interfaces;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task<IEnumerable<Transaction>> GetUserTransactionsAsync(long userId);
    Task<(IEnumerable<Transaction> Transactions, int TotalCount)> GetPagedUserTransactionsAsync
        (long userId, int page, int pageSize,
             string? sortBy, bool ascending,
             string? categoryFilter, DateTime? fromDate, DateTime? toDate);
}