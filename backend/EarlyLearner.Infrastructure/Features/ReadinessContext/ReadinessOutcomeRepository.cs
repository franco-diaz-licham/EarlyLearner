using EarlyLearner.Application.UseCases.ReadinessContext;
using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Domain.ReadinessContext.ValueObjects;
using EarlyLearner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Infrastructure.Features.ReadinessContext;

public sealed class ReadinessOutcomeRepository(DatabaseContext db) : IReadinessOutcomeQueryRepository, IReadinessOutcomeCommandRepository
{
    public async Task<List<ReadinessOutcomeResponse>> ListAsync(CancellationToken cancellationToken)
    {
        return await db.ReadinessOutcomes
            .AsNoTracking()
            .OrderBy(outcome => outcome.SortOrder)
            .Select(outcome => new ReadinessOutcomeResponse(
                ReadinessOutcomeId: outcome.Id.Value,
                Code: outcome.Code,
                Name: outcome.Name,
                Description: outcome.Description,
                Category: outcome.Category,
                SortOrder: outcome.SortOrder,
                Status: outcome.Status))
            .ToListAsync(cancellationToken);
    }

    public Task<ReadinessOutcome?> GetAsync(ReadinessOutcomeId readinessOutcomeId, CancellationToken cancellationToken)
    {
        return db.ReadinessOutcomes.SingleOrDefaultAsync(item => item.Id == readinessOutcomeId, cancellationToken);
    }

    public async Task<ReadinessOutcomeResponse?> GetResponseAsync(ReadinessOutcomeId readinessOutcomeId, CancellationToken cancellationToken)
    {
        return await db.ReadinessOutcomes
            .AsNoTracking()
            .Where(item => item.Id == readinessOutcomeId)
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
