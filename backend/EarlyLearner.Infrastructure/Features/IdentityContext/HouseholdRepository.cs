using EarlyLearner.Application.Features.IdentityContext;
using EarlyLearner.Application.Ports;
using EarlyLearner.Domain.IdentityContext.Entities;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EarlyLearner.Infrastructure.Features.IdentityContext;

public sealed class HouseholdRepository(DatabaseContext db) : IHouseholdQueryRepository, IHouseholdCommandRepository
{
    public async Task<List<HouseholdResponse>> ListAsync(CancellationToken cancellationToken)
    {
        return await db.Households
            .AsNoTracking()
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
                        string.Empty,
                        child.DateOfBirth))
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
            .AsSplitQuery()
            .Include(household => household.Carers).ThenInclude(carer => carer.User)
            .Include(household => household.Children)
            .Include(household => household.Invitations)
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
                        string.Empty,
                        child.DateOfBirth))
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

    public Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return db.Users.SingleOrDefaultAsync(user => user.Id.Value == userId, cancellationToken);
    }

    public Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return db.Users.SingleOrDefaultAsync(user => user.Email == normalizedEmail, cancellationToken);
    }

    public void Add(Household household)
    {
        db.Households.Add(household);
    }

    public void AddUser(User user)
    {
        db.Users.Add(user);
    }

    public void Remove(Household household)
    {
        db.Households.Remove(household);
    }
}
