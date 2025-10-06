using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Repositories
{
    public class TransactionRepository : GenericRepository<Transaction>
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transaction>> GetByTypeAsync(TransactionType type)
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.Type == type)
                .ToListAsync();
        }
    }
}