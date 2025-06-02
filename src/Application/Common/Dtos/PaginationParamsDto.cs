namespace Application.Common.Dtos;

public record PaginationParamsDto
{
    public int PageSize { get; init; } = 30;
    public int PageIndex { get; init; } = 1;
}
