

namespace ExpenseTracker.Application.DTOs;

public class TransactionDto
{   
    public decimal Amount { get; set; }
    public string Description { get; set; } = default!;
    public DateTime Date { get; set; }

    public long UserId { get; set; }
    public long CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;
}