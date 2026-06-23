using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Shared.Enums;
using EarlyLearner.Shared.Utilities;

namespace EarlyLearner.Application.Features.IdentityContext;

public sealed record HouseholdResponse(Guid Id, string Name, List<CarerResponse> Carers, List<ChildResponse> Children, List<HouseholdInvitationResponse> Invitations);
public sealed record CarerResponse(Guid Id, Guid UserId, string Email, string FirstName, string LastName, int RoleId, string Role, string AccountStatus);
public sealed record ChildResponse(Guid Id, string FirstName, string LastName, DateOnly DateOfBirth);
public sealed record HouseholdInvitationResponse(Guid Id, string Email, string FirstName, string LastName, int RoleId, string Role, string Status, DateTimeOffset InvitedAt, DateTimeOffset ExpiresAt);

public interface IHouseholdQueryService
{
    Task<Result<List<HouseholdResponse>>> ListAsync(CancellationToken cancellationToken);
    Task<Result<HouseholdResponse>> GetAsync(HouseholdId id, CancellationToken cancellationToken);
}


public sealed class HouseholdQueryService(IHouseholdQueryRepository householdRepo) : IHouseholdQueryService
{
    public async Task<Result<List<HouseholdResponse>>> ListAsync(CancellationToken cancellationToken)
    {
        var households = await householdRepo.ListAsync(cancellationToken);
        return Result<List<HouseholdResponse>>.Success(households, ResultTypeEnum.Success, households.Count);
    }

    public async Task<Result<HouseholdResponse>> GetAsync(HouseholdId id, CancellationToken cancellationToken)
    {
        var household = await householdRepo.GetResponseAsync(id, cancellationToken);
        return household is null
            ? Result<HouseholdResponse>.Fail("Household was not found.", ResultTypeEnum.NotFound)
            : Result<HouseholdResponse>.Success(household, ResultTypeEnum.Success);
    }
}
