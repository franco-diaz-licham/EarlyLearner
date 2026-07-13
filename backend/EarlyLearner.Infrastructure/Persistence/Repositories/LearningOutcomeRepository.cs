using EarlyLearner.Application.UseCases.LearningContext;
using EarlyLearner.Domain.LearningContext.Entities;
using EarlyLearner.Domain.LearningContext.ValueObjects;
using EarlyLearner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Infrastructure.Persistence.Repositories;

public sealed class LearningOutcomeRepository(DatabaseContext db) : ILearningOutcomeQueryRepository, ILearningOutcomeCommandRepository
{
    public async Task<List<LearningOutcomeResponse>> ListAsync(CancellationToken cancellationToken)
    {
        return await db.LearningOutcomes
            .AsNoTracking()
            .OrderBy(outcome => outcome.SortOrder)
            .Select(outcome => new LearningOutcomeResponse(
                LearningOutcomeId: outcome.Id.Value,
                Code: outcome.Code,
                Name: outcome.Name,
                Description: outcome.Description,
                Category: outcome.Category,
                SortOrder: outcome.SortOrder,
                Status: outcome.Status))
            .ToListAsync(cancellationToken);
    }

    public Task<LearningOutcome?> GetAsync(LearningOutcomeId learningOutcomeId, CancellationToken cancellationToken)
    {
        return db.LearningOutcomes.SingleOrDefaultAsync(item => item.Id == learningOutcomeId, cancellationToken);
    }

    public async Task<LearningOutcomeResponse?> GetResponseAsync(LearningOutcomeId learningOutcomeId, CancellationToken cancellationToken)
    {
        return await db.LearningOutcomes
            .AsNoTracking()
            .Where(item => item.Id == learningOutcomeId)
            .Select(item => new LearningOutcomeResponse(
                LearningOutcomeId: item.Id.Value,
                Code: item.Code,
                Name: item.Name,
                Description: item.Description,
                Category: item.Category,
                SortOrder: item.SortOrder,
                Status: item.Status))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public Task<bool> IsUsedByLearningMomentAsync(LearningOutcomeId learningOutcomeId, CancellationToken cancellationToken)
    {
        return db.LearningMoments
            .AnyAsync(moment => moment.LearningOutcomes.Any(outcome => outcome.Id == learningOutcomeId), cancellationToken);
    }

    public void Add(LearningOutcome learningOutcome)
    {
        db.LearningOutcomes.Add(learningOutcome);
    }

    public void Remove(LearningOutcome learningOutcome)
    {
        db.LearningOutcomes.Remove(learningOutcome);
    }
}
