namespace EarlyLearner.Domain.PlanningContext;

/// <summary>Describes the planning horizon for a learning or development goal.</summary>
public enum GoalTypeEnum
{
    ShortTerm = 1,
    LongTerm = 2
}

/// <summary>Describes the lifecycle status of a goal.</summary>
public enum GoalStatusEnum
{
    Active = 1,
    Completed = 2,
    Archived = 3
}
