namespace ExpenseTracker.Application.DTOs.Analytics;

public class CategorySummaryDto
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}
public class MonthlySummaryDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal NetBalance => TotalIncome - TotalExpense;
    public IEnumerable<CategorySummaryDto> TopCategories { get; set; } = new List<CategorySummaryDto>();
    public decimal? PreviousMonthBalance { get; set; } 
}