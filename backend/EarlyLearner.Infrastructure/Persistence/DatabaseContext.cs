using EarlyLearner.Domain.CoreContext.Entities;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.LearningRecordContext.Entities;
using EarlyLearner.Domain.PlanningContext.Entities;
using EarlyLearner.Domain.ReadinessContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Infrastructure.Persistence;

public sealed class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<Household> Households => Set<Household>();
    public DbSet<Carer> Carers => Set<Carer>();
    public DbSet<Child> Children => Set<Child>();
    public DbSet<StoredFile> StoredFiles => Set<StoredFile>();
    public DbSet<ReadinessOutcome> ReadinessOutcomes => Set<ReadinessOutcome>();
    public DbSet<ReadinessProfile> ReadinessProfiles => Set<ReadinessProfile>();
    public DbSet<ReadinessOutcomeProgress> ReadinessOutcomeProgress => Set<ReadinessOutcomeProgress>();
    public DbSet<EvidenceReference> EvidenceReferences => Set<EvidenceReference>();
    public DbSet<SuggestedNextStep> SuggestedNextSteps => Set<SuggestedNextStep>();
    public DbSet<PortfolioItem> PortfolioItems => Set<PortfolioItem>();
    public DbSet<Goal> Goals => Set<Goal>();
    public DbSet<LearningPlan> LearningPlans => Set<LearningPlan>();
    public DbSet<PlannedLearningSession> PlannedLearningSessions => Set<PlannedLearningSession>();
    public DbSet<DailyLog> DailyLogs => Set<DailyLog>();
    public DbSet<CompletedActivity> CompletedActivities => Set<CompletedActivity>();
    public DbSet<ReadingEntry> ReadingEntries => Set<ReadingEntry>();
    public DbSet<RoutineEntry> RoutineEntries => Set<RoutineEntry>();
    public DbSet<Observation> Observations => Set<Observation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
