namespace EarlyLearner.Api.Models;

public sealed class PagedList<T>
{
    /// <summary>
    /// Wraps list models with pagination metadata for API responses.
    /// </summary>
    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        Items = items;
        Pagination = new PaginationMetadata(pageNumber: pageNumber, pageSize: pageSize, totalCount: count, totalPages: (int)Math.Ceiling(count / (double)pageSize));
    }

    public IReadOnlyList<T> Items { get; }
    public PaginationMetadata Pagination { get; }
}

