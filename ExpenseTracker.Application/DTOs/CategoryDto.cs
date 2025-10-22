namespace ExpenseTracker.Application.DTOs;

public class CategoryDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Name { get; set; } = default!;
}