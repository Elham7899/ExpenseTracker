using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Repositories
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Transaction>> GetUserTransactionsAsync(long userId)
        {
            return await _dbSet
                .Where(t => t.UserId == userId)
                .Include(t => t.Category)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Transaction> Transactions, int TotalCount)>
            GetPagedUserTransactionsAsync(long userId, int page, int pageSize,
                string? sortBy, bool ascending,
                string? categoryFilter, DateTime? fromDate, DateTime? toDate)
        {
            var query = _dbSet
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(categoryFilter))
                query = query.Where(t => t.Category!.Name == categoryFilter);

            if (fromDate.HasValue)
                query = query.Where(t => t.Date >= fromDate);

            if (toDate.HasValue)
                query = query.Where(t => t.Date <= toDate);

            query = sortBy?.ToLower() switch
            {
                "date" => ascending ? query.OrderBy(t => t.Date) : query.OrderByDescending(t => t.Date),
                "amount" => ascending ? query.OrderBy(t => t.Amount) : query.OrderByDescending(t => t.Amount),
                _ => query.OrderByDescending(t => t.Date)
            };

            var totalCount = await query.CountAsync();
            var transactions = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (transactions, totalCount);
        }
    }
}