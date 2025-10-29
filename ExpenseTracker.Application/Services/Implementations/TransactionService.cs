using ExpenseTracker.Application.DTOs.Analytics;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;

    public TransactionService(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
        => await _transactionRepository.GetAllAsync();

    public async Task<IEnumerable<Transaction>> GetUserTransactionsAsync(long userId)
        => await _transactionRepository.GetUserTransactionsAsync(userId);

    public async Task<Transaction?> GetTransactionByIdAsync(long id)
        => await _transactionRepository.GetByIdAsync(id);

    public async Task AddTransactionAsync(Transaction transaction)
    {
        await _transactionRepository.AddAsync(transaction);
        await _transactionRepository.SaveChangesAsync();
    }

    public async Task UpdateTransactionAsync(Transaction transaction)
    {
        _transactionRepository.Update(transaction);
        await _transactionRepository.SaveChangesAsync();
    }

    public async Task DeleteTransactionAsync(long id)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id);
        if (transaction != null)
        {
            _transactionRepository.Remove(transaction);
            await _transactionRepository.SaveChangesAsync();
        }
    }
    public async Task<IEnumerable<CategorySummaryDto>> GetMonthlyCategorySummaryAsync(long userId, int year, int month)
    {
        var transactions = await _transactionRepository.GetUserTransactionsAsync(userId);
        var monthlyTransactions = transactions
            .Where(t => t.Date.Year == year && t.Date.Month == month);

        var summary = monthlyTransactions
            .GroupBy(t => t.Category!.Name)
            .Select(g => new CategorySummaryDto
            {
                CategoryName = g.Key,
                TotalAmount = g.Sum(t => t.Amount)
            });

        return summary;
    }
    public async Task<MonthlySummaryDto> GetMonthlySummaryAsync(long userId, int year, int month)
    {
        var transactions = await _transactionRepository.GetUserTransactionsAsync(userId);

        var monthly = transactions
            .Where(t => t.Date.Year == year && t.Date.Month == month);

        var totalIncome = monthly.Where(t => t.Amount > 0).Sum(t => t.Amount);
        var totalExpense = monthly.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount));

        var topCategories = monthly
            .GroupBy(t => t.Category!.Name)
            .Select(g => new CategorySummaryDto
            {
                CategoryName = g.Key,
                TotalAmount = g.Sum(t => Math.Abs(t.Amount))
            })
            .OrderByDescending(c => c.TotalAmount)
            .Take(3)
            .ToList();

        // Compare with previous month
        var prevMonth = month == 1 ? 12 : month - 1;
        var prevYear = month == 1 ? year - 1 : year;
        var prev = transactions.Where(t => t.Date.Year == prevYear && t.Date.Month == prevMonth);
        var prevBalance = prev.Sum(t => t.Amount);

        return new MonthlySummaryDto
        {
            Year = year,
            Month = month,
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            TopCategories = topCategories,
            PreviousMonthBalance = prevBalance
        };
    }

    public async Task<YearlySummaryDto> GetYearlySummaryAsync(long userId, int year)
    {
        var transactions = await _transactionRepository.GetUserTransactionsAsync(userId);
        var yearlyTransactions = transactions.Where(t => t.Date.Year == year);

        var totalIncome = yearlyTransactions
            .Where(t => t.Amount > 0)
            .Sum(t => t.Amount);

        var totalExpense = yearlyTransactions
            .Where(t => t.Amount < 0)
            .Sum(t => Math.Abs(t.Amount));

        return new YearlySummaryDto
        {
            Year = year,
            TotalIncome = totalIncome,
            TotalExpense = totalExpense
        };
    }

    public async Task<(IEnumerable<Transaction> Transactions, int TotalCount)> GetPagedUserTransactionsAsync
        (long userId, int page, int pageSize,
           string? sortBy, bool ascending,
           string? categoryFilter, DateTime? fromDate, DateTime? toDate)
    {
        return await _transactionRepository.GetPagedUserTransactionsAsync(
            userId, page, pageSize, sortBy, ascending, categoryFilter, fromDate, toDate);
    }

}
