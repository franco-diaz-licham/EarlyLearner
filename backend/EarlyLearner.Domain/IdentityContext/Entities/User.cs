using EarlyLearner.Domain.CoreContext;
using EarlyLearner.Domain.IdentityContext.ValueObjects;

namespace EarlyLearner.Domain.IdentityContext.Entities;

public sealed class User : Entity<UserId>
{
    private User() { }

    private User(UserId id, string email, string firstName, string lastName, UserAccountStatusEnum status)
    {
        Id = id;
        Email = Required(email, nameof(email)).ToLowerInvariant();
        FirstName = Required(firstName, nameof(firstName));
        LastName = Required(lastName, nameof(lastName));
        Status = status;
        SetCreatedOn();
    }

    public string Email { get; private set; } = default!;
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public UserAccountStatusEnum Status { get; private set; }
    public string DisplayName => $"{FirstName} {LastName}";

    public static User CreatePendingParent(string email, string firstName, string lastName)
    {
        return new User(new UserId(Guid.NewGuid()), email, firstName, lastName, UserAccountStatusEnum.Pending);
    }

    public static User CreateActiveParent(UserId id, string email, string firstName, string lastName)
    {
        return new User(id, email, firstName, lastName, UserAccountStatusEnum.Active);
    }

    public void MarkActive()
    {
        Status = UserAccountStatusEnum.Active;
        SetUpdatedOn();
    }

    public void Disable()
    {
        Status = UserAccountStatusEnum.Disabled;
        SetUpdatedOn();
    }

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{name} is required.");
        return value.Trim();
    }
}
