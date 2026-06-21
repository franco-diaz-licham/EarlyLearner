using EarlyLearner.Application.Features.IdentityContext;
using EarlyLearner.Domain.IdentityContext.ValueObjects;

namespace EarlyLearner.Application.Ports;

public interface IHouseholdQueryRepository
{
    Task<List<HouseholdResponse>> ListAsync(CancellationToken cancellationToken);
    Task<HouseholdResponse?> GetResponseAsync(HouseholdId id, CancellationToken cancellationToken);
}
