using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Domain.Entities;

public class User : BaseEntity
{
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;

    // Navigation property
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
