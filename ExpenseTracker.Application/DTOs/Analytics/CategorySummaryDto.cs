namespace ExpenseTracker.Application.DTOs.Analytics;

public class CategorySummaryDto
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}