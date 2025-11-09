namespace ExpenseTracker.Application.DTOs.Pagination;

public class PagedResult<T>
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public IEnumerable<T> Items { get; set; } = new List<T>();

    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}