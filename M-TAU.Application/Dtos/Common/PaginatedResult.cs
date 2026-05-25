namespace M_TAU.Application.Dtos.Common;

public sealed record PaginatedResult<T>(
    IReadOnlyCollection<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize)
{
    public int TotalPages => PageSize <= 0 ? 1 : (int)Math.Ceiling(TotalCount / (double)PageSize);

    public static PaginatedResult<T> Create(IReadOnlyCollection<T> items, int totalCount, int? pageNumber, int? pageSize)
    {
        var pn = pageNumber is > 0 ? pageNumber.Value : 1;
        var ps = pageSize is > 0 ? pageSize.Value : (totalCount == 0 ? 10 : totalCount);
        return new PaginatedResult<T>(items, totalCount, pn, ps);
    }
}
