namespace Application.Dtos;

public class PaginatedList<T>
{
    public List<T> Items { get; }
    public int CurrentPage { get; }
    public int TotalPages { get; }
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;

    public PaginatedList(List<T> items, int pageIndex, int totalPages)
    {
        Items = items;
        CurrentPage = pageIndex;
        TotalPages = totalPages;
    }
}
