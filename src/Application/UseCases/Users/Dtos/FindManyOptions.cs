using Application.Common.Dtos;

namespace Application.UseCases.Users.Dtos;

public class FindManyOptionsDto
{
    public UserFiltersDto Filters { get; init; } = new();
    public PaginationParamsDto Pagination { get; init; } = new();
}
