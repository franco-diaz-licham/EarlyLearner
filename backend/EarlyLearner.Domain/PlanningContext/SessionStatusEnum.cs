namespace EarlyLearner.Domain.PlanningContext;

/// <summary>Describes whether a planned learning session is still upcoming or how it ended.</summary>
public enum SessionStatusEnum
{
    Planned = 1,
    Completed = 2,
    Skipped = 3,
    Moved = 4
}
