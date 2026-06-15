namespace EarlyLearner.Domain.CoreContext;

/// <summary>
/// Describes the lifecycle of a file after upload and before use in parent-facing evidence.
/// </summary>
public enum StoredFileStatusEnum
{
    Pending = 1,
    Available = 2,
    Rejected = 3,
    Deleted = 4
}
