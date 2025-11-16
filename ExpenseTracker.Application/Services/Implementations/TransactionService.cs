using ExpenseTracker.Application.DTOs.Analytics;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace ExpenseTracker.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMemoryCache _cache;

    // Stores all cache keys per user for easy invalidation
    private static readonly Dictionary<long, HashSet<string>> _userCacheKeys = new();

    public TransactionService(ITransactionRepository transactionRepository, IMemoryCache cache)
    {
        _transactionRepository = transactionRepository;
        _cache = cache;
    }

    private string GetCacheKey(string prefix, long userId, int year, int? month = null)
    {
        return month is null
            ? $"{prefix}_{userId}_{year}"
            : $"{prefix}_{userId}_{year}_{month}";
    }

    private void TrackKey(long userId, string cacheKey)
    {
        if (!_userCacheKeys.ContainsKey(userId))
            _userCacheKeys[userId] = new HashSet<string>();

        _userCacheKeys[userId].Add(cacheKey);
    }

    private void ClearUserAnalyticsCache(long userId)
    {
        if (_userCacheKeys.TryGetValue(userId, out var keys))
        {
            foreach (var key in keys)
                _cache.Remove(key);

            keys.Clear();
        }
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
        ClearUserAnalyticsCache(transaction.UserId);
    }

    public async Task UpdateTransactionAsync(Transaction transaction)
    {
        _transactionRepository.Update(transaction);
        await _transactionRepository.SaveChangesAsync();
        ClearUserAnalyticsCache(transaction.UserId);
    }

    public async Task DeleteTransactionAsync(long id)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id);
        if (transaction != null)
        {
            _transactionRepository.Remove(transaction);
            await _transactionRepository.SaveChangesAsync();
            ClearUserAnalyticsCache(transaction.UserId);
        }
    }

    public async Task<IEnumerable<CategorySummaryDto>> GetMonthlyCategorySummaryAsync(long userId, int year, int month)
    {
        var cacheKey = GetCacheKey("MonthlyCategory", userId, year, month);

        if (_cache.TryGetValue(cacheKey, out IEnumerable<CategorySummaryDto>? cached))
            return cached!;

        var transactions = await _transactionRepository.GetUserTransactionsAsync(userId);

        var summary = transactions
            .Where(t => t.Date.Year == year && t.Date.Month == month)
            .GroupBy(t => t.Category!.Name)
            .Select(g => new CategorySummaryDto
            {
                CategoryName = g.Key,
                TotalAmount = g.Sum(t => t.Amount)
            })
            .ToList();

        _cache.Set(cacheKey, summary, TimeSpan.FromMinutes(10));
        TrackKey(userId, cacheKey);

        return summary;
    }

    public async Task<MonthlySummaryDto> GetMonthlySummaryAsync(long userId, int year, int month)
    {
        var cacheKey = GetCacheKey("MonthlySummary", userId, year, month);

        if (_cache.TryGetValue(cacheKey, out MonthlySummaryDto? cached))
            return cached!;

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

        // Previous month
        var prevMonth = month == 1 ? 12 : month - 1;
        var prevYear = month == 1 ? year - 1 : year;

        var prev = transactions
            .Where(t => t.Date.Year == prevYear && t.Date.Month == prevMonth)
            .Sum(t => t.Amount);

        var summary = new MonthlySummaryDto
        {
            Year = year,
            Month = month,
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            TopCategories = topCategories,
            PreviousMonthBalance = prev
        };

        _cache.Set(cacheKey, summary, TimeSpan.FromMinutes(10));
        TrackKey(userId, cacheKey);

        return summary;
    }

    public async Task<YearlySummaryDto> GetYearlySummaryAsync(long userId, int year)
    {
        var cacheKey = GetCacheKey("YearlySummary", userId, year);

        if (_cache.TryGetValue(cacheKey, out YearlySummaryDto? cached))
            return cached!;

        var transactions = await _transactionRepository.GetUserTransactionsAsync(userId);
        var yearly = transactions.Where(t => t.Date.Year == year);

        var summary = new YearlySummaryDto
        {
            Year = year,
            TotalIncome = yearly.Where(t => t.Amount > 0).Sum(t => t.Amount),
            TotalExpense = yearly.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount))
        };

        _cache.Set(cacheKey, summary, TimeSpan.FromMinutes(10));
        TrackKey(userId, cacheKey);

        return summary;
    }

    public async Task<(IEnumerable<Transaction> Transactions, int TotalCount)> GetPagedUserTransactionsAsync(
        long userId, int page, int pageSize,
        string? sortBy, bool ascending,
        string? categoryFilter, DateTime? fromDate, DateTime? toDate)
    {
        return await _transactionRepository.GetPagedUserTransactionsAsync(
            userId, page, pageSize, sortBy, ascending, categoryFilter, fromDate, toDate);
    }
}
