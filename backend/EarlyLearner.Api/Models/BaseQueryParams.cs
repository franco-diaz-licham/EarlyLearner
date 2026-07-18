using FluentValidation;

namespace EarlyLearner.Api.Models;

public class BaseQueryParams
{
    public const int MAX_PAGE_SIZE = 50;
    public const int PAGE_SIZE = 8;
    public const int PAGE_NUMBER = 1;

    private int? _pageNumber;
    private int? _pageSize;

    public int PageNumber
    {
        get => _pageNumber ?? PAGE_NUMBER;
        set => _pageNumber = value;
    }

    public int PageSize
    {
        get => _pageSize ?? PAGE_SIZE;
        set => _pageSize = value;
    }

    public string? SearchTerm { get; set; }
}

public class BaseQueryParamsValidator : AbstractValidator<BaseQueryParams>
{
    public BaseQueryParamsValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, BaseQueryParams.MAX_PAGE_SIZE);

        RuleFor(query => query.SearchTerm)
            .MaximumLength(120)
            .When(query => !string.IsNullOrWhiteSpace(query.SearchTerm));
    }
}
