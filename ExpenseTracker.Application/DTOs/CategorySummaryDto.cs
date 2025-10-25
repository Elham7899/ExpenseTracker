namespace ExpenseTracker.Application.DTOs;

public class CategorySummaryDto
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}