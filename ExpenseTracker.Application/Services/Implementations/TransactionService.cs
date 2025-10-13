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
}
