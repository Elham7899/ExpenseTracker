namespace ExpenseTracker.Application.DTOs.Analytics;

public class YearlySummaryDto
{
    public int Year { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal NetBalance => TotalIncome - TotalExpense;
}