using EarlyLearner.Application.Features.ReadinessContext;
using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Infrastructure.Features.ReadinessContext;

public sealed class EfReadinessOutcomeCommandRepository(DatabaseContext db) : IReadinessOutcomeCommandRepository
{
    public Task<ReadinessOutcome?> GetAsync(Guid readinessOutcomeId, CancellationToken cancellationToken)
    {
        return db.ReadinessOutcomes.SingleOrDefaultAsync(item => item.Id.Value == readinessOutcomeId, cancellationToken);
    }

    public async Task<ReadinessOutcomeResponse?> GetResponseAsync(Guid readinessOutcomeId, CancellationToken cancellationToken)
    {
        return await db.ReadinessOutcomes
            .AsNoTracking()
            .Where(item => item.Id.Value == readinessOutcomeId)
            .Select(item => new ReadinessOutcomeResponse(
                ReadinessOutcomeId: item.Id.Value,
                Code: item.Code,
                Name: item.Name,
                Description: item.Description,
                Category: item.Category,
                SortOrder: item.SortOrder,
                Status: item.Status))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public void Add(ReadinessOutcome readinessOutcome)
    {
        db.ReadinessOutcomes.Add(readinessOutcome);
    }
}
