using EarlyLearner.Domain.CoreContext.Entities;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.LearningContext.Entities;
using EarlyLearner.Domain.ReadinessContext.Entities;
using EarlyLearner.Infrastructure.Persistence.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Infrastructure.Persistence;

public sealed class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Household> Households => Set<Household>();
    public DbSet<HouseholdInvitation> HouseholdInvitations => Set<HouseholdInvitation>();
    public DbSet<Carer> Carers => Set<Carer>();
    public DbSet<Child> Children => Set<Child>();
    public DbSet<StoredFile> StoredFiles => Set<StoredFile>();
    public DbSet<ReadinessOutcome> ReadinessOutcomes => Set<ReadinessOutcome>();
    public DbSet<ReadinessProfile> ReadinessProfiles => Set<ReadinessProfile>();
    public DbSet<ReadinessOutcomeProgress> ReadinessOutcomeProgress => Set<ReadinessOutcomeProgress>();
    public DbSet<EvidenceReference> EvidenceReferences => Set<EvidenceReference>();
    public DbSet<DailyLog> DailyLogs => Set<DailyLog>();
    public DbSet<LearningMoment> LearningMoments => Set<LearningMoment>();
    public DbSet<AuditTrailReadModel> AuditTrailEntries => Set<AuditTrailReadModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
        base.OnModelCreating(modelBuilder);
    }
}
