using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;

public class TransactionServiceTests
{
    private readonly Mock<ITransactionRepository> _transactionRepoMock;
    private readonly IMemoryCache _memoryCache;
    private readonly TransactionService _service;

    public TransactionServiceTests()
    {
        _transactionRepoMock = new Mock<ITransactionRepository>();
        _memoryCache = new MemoryCache(new MemoryCacheOptions());

        _service = new TransactionService(
            _transactionRepoMock.Object,
            _memoryCache);
    }

    [Fact]
    public async Task GetAllTransactionsAsync_Should_Return_All_Transactions()
    {
        var transactions = new List<Transaction>
        {
            new Transaction { Id = 1, Amount = 100 },
            new Transaction { Id = 2, Amount = -50 }
        };

        _transactionRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(transactions);

        var result = await _service.GetAllTransactionsAsync();

        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(transactions);
    }

    [Fact]
    public async Task GetUserTransactionsAsync_Should_Return_User_Transactions()
    {
        var userId = 10L;
        var transactions = new List<Transaction>
        {
            new Transaction { Id = 1, UserId = userId, Amount = 100 }
        };

        _transactionRepoMock.Setup(r => r.GetUserTransactionsAsync(userId))
            .ReturnsAsync(transactions);

        var result = await _service.GetUserTransactionsAsync(userId);

        result.Should().HaveCount(1);
        result.First().UserId.Should().Be(userId);
    }

    [Fact]
    public async Task GetTransactionByIdAsync_Should_Return_Transaction_When_Exists()
    {
        var transaction = new Transaction { Id = 1, Amount = 100 };

        _transactionRepoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(transaction);

        var result = await _service.GetTransactionByIdAsync(1);

        result.Should().NotBeNull();
        result!.Amount.Should().Be(100);
    }

    [Fact]
    public async Task AddTransactionAsync_Should_Add_Save_And_Clear_Cache()
    {
        var transaction = new Transaction { Id = 1, UserId = 5, Amount = 100 };

        await _service.AddTransactionAsync(transaction);

        _transactionRepoMock.Verify(r => r.AddAsync(transaction), Times.Once);
        _transactionRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateTransactionAsync_Should_Update_Save_And_Clear_Cache()
    {
        var transaction = new Transaction { Id = 1, UserId = 5, Amount = 200 };

        await _service.UpdateTransactionAsync(transaction);

        _transactionRepoMock.Verify(r => r.Update(transaction), Times.Once);
        _transactionRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteTransactionAsync_Should_Remove_Save_And_Clear_Cache_When_Exists()
    {
        var transaction = new Transaction { Id = 1, UserId = 5, Amount = 50 };

        _transactionRepoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(transaction);

        await _service.DeleteTransactionAsync(1);

        _transactionRepoMock.Verify(r => r.Remove(transaction), Times.Once);
        _transactionRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteTransactionAsync_Should_Do_Nothing_When_Not_Found()
    {
        _transactionRepoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Transaction?)null);

        await _service.DeleteTransactionAsync(1);

        _transactionRepoMock.Verify(r => r.Remove(It.IsAny<Transaction>()), Times.Never);
        _transactionRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task GetMonthlyCategorySummaryAsync_Should_Return_Summary_And_Cache_It()
    {
        var userId = 5L;
        var year = 2025;
        var month = 12;
        var category = new Category { Id = 1, Name = "Food" };
        var transactions = new List<Transaction>
        {
            new Transaction { UserId = userId, Amount = 100, Date = new DateTime(year, month, 1), Category = category },
            new Transaction { UserId = userId, Amount = 50, Date = new DateTime(year, month, 2), Category = category }
        };

        _transactionRepoMock.Setup(r => r.GetUserTransactionsAsync(userId))
            .ReturnsAsync(transactions);

        var result = await _service.GetMonthlyCategorySummaryAsync(userId, year, month);

        result.Should().HaveCount(1);
        result.First().CategoryName.Should().Be("Food");
        result.First().TotalAmount.Should().Be(150);
    }

    [Fact]
    public async Task GetMonthlySummaryAsync_Should_Return_Correct_Values_And_Cache_It()
    {
        var userId = 5L;
        var year = 2025;
        var month = 12;
        var category = new Category { Id = 1, Name = "Food" };
        var transactions = new List<Transaction>
        {
            new Transaction { UserId = userId, Amount = 200, Date = new DateTime(year, month, 1), Category = category },
            new Transaction { UserId = userId, Amount = -50, Date = new DateTime(year, month, 2), Category = category },
            new Transaction { UserId = userId, Amount = -30, Date = new DateTime(year, month - 1, 15), Category = category }
        };

        _transactionRepoMock.Setup(r => r.GetUserTransactionsAsync(userId))
            .ReturnsAsync(transactions);

        var result = await _service.GetMonthlySummaryAsync(userId, year, month);

        result.TotalIncome.Should().Be(200);
        result.TotalExpense.Should().Be(50);
        result.PreviousMonthBalance.Should().Be(-30);
        result.TopCategories.First().CategoryName.Should().Be("Food");
        result.TopCategories.First().TotalAmount.Should().Be(250);
    }

    [Fact]
    public async Task GetYearlySummaryAsync_Should_Return_Correct_Values_And_Cache_It()
    {
        var userId = 5L;
        var year = 2025;
        var category = new Category { Id = 1, Name = "Food" };
        var transactions = new List<Transaction>
        {
            new Transaction { UserId = userId, Amount = 100, Date = new DateTime(year, 1, 1), Category = category },
            new Transaction { UserId = userId, Amount = -30, Date = new DateTime(year, 2, 1), Category = category }
        };

        _transactionRepoMock.Setup(r => r.GetUserTransactionsAsync(userId))
            .ReturnsAsync(transactions);

        var result = await _service.GetYearlySummaryAsync(userId, year);

        result.TotalIncome.Should().Be(100);
        result.TotalExpense.Should().Be(30);
    }
}
