namespace Application.Dtos.User;

public class FindManyOptionsDto
{
    public UserFiltersDto Filters { get; init; } = new();
    public PaginationParamsDto Pagination { get; init; } = new();
}
