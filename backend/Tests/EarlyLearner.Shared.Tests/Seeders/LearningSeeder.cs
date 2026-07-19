using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.LearningContext;
using EarlyLearner.Domain.LearningContext.Entities;
using EarlyLearner.Infrastructure.Persistence;

namespace EarlyLearner.Shared.Tests.Seeders;

public static class LearningSeeder
{
    public static LearningOutcome CreateOutcome(
        string code = "language-listening",
        string name = "Listens and responds",
        string description = "Listens to instructions.",
        string category = "Language",
        int sortOrder = 10)
    {
        return LearningOutcome.Create(code, name, description, category, sortOrder);
    }

    public static DailyLog CreateDailyLog(
        Household household,
        Child child,
        DateOnly? logDate = null,
        LearningMomentKindEnum kind = LearningMomentKindEnum.Activity,
        string title = "Paint mixing",
        string notes = "Mixed colours.",
        params LearningOutcome[] outcomes)
    {
        var dailyLog = DailyLog.Create(household.Id, child.Id, logDate ?? new DateOnly(2026, 7, 18));
        dailyLog.RecordLearningMoment(kind, title, notes, outcomes);
        return dailyLog;
    }

    public static async Task SeedOutcomesAsync(DatabaseContext db, params LearningOutcome[] outcomes)
    {
        db.LearningOutcomes.AddRange(outcomes);
        await db.SaveChangesAsync();
    }

    public static async Task SeedDailyLogsAsync(DatabaseContext db, params DailyLog[] dailyLogs)
    {
        db.DailyLogs.AddRange(dailyLogs);
        await db.SaveChangesAsync();
    }
}
