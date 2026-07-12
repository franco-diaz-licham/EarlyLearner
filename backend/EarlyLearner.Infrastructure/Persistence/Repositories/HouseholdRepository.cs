using EarlyLearner.Application.UseCases.IdentityContext;
using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Infrastructure.Persistence.Repositories;

public sealed class HouseholdRepository(DatabaseContext db) : IHouseholdQueryRepository, IHouseholdCommandRepository
{
    public async Task<List<HouseholdResponse>> ListAsync(IReadOnlyCollection<HouseholdId> householdIds, CancellationToken cancellationToken)
    {
        if (householdIds.Count == 0) return [];

        var ids = householdIds.Distinct().ToArray();

        return await db.Households
            .AsNoTracking()
            .Where(household => ids.Contains(household.Id))
            .OrderBy(household => household.Name)
            .Select(household => new HouseholdResponse(
                household.Id.Value,
                household.Name,
                household.Carers
                    .OrderBy(carer => carer.User.FirstName)
                    .ThenBy(carer => carer.User.LastName)
                    .Select(carer => new CarerResponse(
                        carer.Id.Value,
                        carer.UserId.Value,
                        carer.User.Email,
                        carer.User.FirstName,
                        carer.User.LastName,
                        carer.Role.ToString(),
                        carer.User.Status.ToString()))
                    .ToList(),
                household.Children
                    .Where(child => !child.IsArchived)
                    .OrderBy(child => child.FirstName)
                    .Select(child => new ChildResponse(
                        child.Id.Value,
                        child.FirstName,
                        child.LastName,
                        child.DateOfBirth,
                        child.AvatarStoredFileId == null ? null : child.AvatarStoredFileId.Value.Value))
                    .ToList(),
                household.Invitations
                    .OrderByDescending(invitation => invitation.InvitedAt)
                    .Select(invitation => new HouseholdInvitationResponse(
                        invitation.Id.Value,
                        invitation.Email,
                        invitation.FirstName,
                        invitation.LastName,
                        invitation.Role.ToString(),
                        invitation.Status.ToString(),
                        invitation.InvitedAt,
                        invitation.ExpiresAt))
                    .ToList()))
            .ToListAsync(cancellationToken);
    }

    public Task<Household?> GetAsync(HouseholdId householdId, CancellationToken cancellationToken)
    {
        return db.Households
            .Include(household => household.Carers).ThenInclude(carer => carer.User)
            .Include(household => household.Children)
            .Include(household => household.Invitations)
            .AsSplitQuery()
            .SingleOrDefaultAsync(item => item.Id == householdId, cancellationToken);
    }

    public async Task<HouseholdResponse?> GetResponseAsync(HouseholdId id, CancellationToken cancellationToken)
    {
        return await db.Households
            .AsNoTracking()
            .Where(item => item.Id == id)
            .Select(item => new HouseholdResponse(
                item.Id.Value,
                item.Name,
                item.Carers
                    .OrderBy(carer => carer.User.FirstName)
                    .ThenBy(carer => carer.User.LastName)
                    .Select(carer => new CarerResponse(
                        carer.Id.Value,
                        carer.UserId.Value,
                        carer.User.Email,
                        carer.User.FirstName,
                        carer.User.LastName,
                        carer.Role.ToString(),
                        carer.User.Status.ToString()))
                    .ToList(),
                item.Children
                    .Where(child => !child.IsArchived)
                    .OrderBy(child => child.FirstName)
                    .Select(child => new ChildResponse(
                        child.Id.Value,
                        child.FirstName,
                        child.LastName,
                        child.DateOfBirth,
                        child.AvatarStoredFileId == null ? null : child.AvatarStoredFileId.Value.Value))
                    .ToList(),
                item.Invitations
                    .OrderByDescending(invitation => invitation.InvitedAt)
                    .Select(invitation => new HouseholdInvitationResponse(
                        invitation.Id.Value,
                        invitation.Email,
                        invitation.FirstName,
                        invitation.LastName,
                        invitation.Role.ToString(),
                        invitation.Status.ToString(),
                        invitation.InvitedAt,
                        invitation.ExpiresAt))
                    .ToList()))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public Task<User?> GetUserByIdAsync(UserId userId, CancellationToken cancellationToken)
    {
        return db.Users.SingleOrDefaultAsync(user => user.Id == userId, cancellationToken);
    }

    public Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return db.Users.SingleOrDefaultAsync(user => user.Email == normalizedEmail, cancellationToken);
    }

    public void AddUser(User user)
    {
        db.Users.Add(user);
    }

}
