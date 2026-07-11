using EarlyLearner.Application.UseCases.IdentityContext;
using EarlyLearner.Domain.IdentityContext.ValueObjects;

namespace EarlyLearner.Application.Ports;

public interface IHouseholdQueryRepository
{
    Task<List<HouseholdResponse>> ListAsync(IReadOnlyCollection<HouseholdId> householdIds, CancellationToken cancellationToken);
    Task<HouseholdResponse?> GetResponseAsync(HouseholdId id, CancellationToken cancellationToken);
}
