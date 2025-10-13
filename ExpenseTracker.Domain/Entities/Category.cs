using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = default!;

    public long UserId { get; set; }     
    public User User { get; set; } = default!;

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
