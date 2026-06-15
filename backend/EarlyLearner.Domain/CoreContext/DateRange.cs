namespace EarlyLearner.Domain.CoreContext;

/// <summary>
/// Represents a closed calendar date range used for goals and planning periods.
/// </summary>
public sealed record DateRange
{
    private DateRange(DateOnly startDate, DateOnly endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    public DateOnly StartDate { get; }

    public DateOnly EndDate { get; }

    public static DateRange Create(DateOnly startDate, DateOnly endDate)
    {
        if (endDate < startDate) throw new DomainException("End date must be on or after start date.");
        return new DateRange(startDate, endDate);
    }

    public bool Contains(DateOnly date)
    {
        return date >= StartDate && date <= EndDate;
    }
}
