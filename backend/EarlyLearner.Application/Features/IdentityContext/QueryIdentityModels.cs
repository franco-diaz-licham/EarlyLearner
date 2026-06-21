using EarlyLearner.Domain.IdentityContext;
using EarlyLearner.Domain.IdentityContext.ValueObjects;

namespace EarlyLearner.Application.Features.IdentityContext;

public sealed record UserModel(
    string FullName,
    UserId UserId,
    HouseholdId HouseholdId,
    bool IsSuperAdmin,
    UserAccountStatusEnum Status,
    CarerId? CarerId = null);

public sealed record UserResponse(
    string FullName,
    UserId UserId,
    HouseholdId HouseholdId,
    bool IsSuperAdmin,
    UserAccountStatusEnum Status,
    CarerId? CarerId = null);
