namespace EarlyLearner.Domain.CoreContext;

/// <summary>
/// Represents a business rule violation raised by the domain model.
/// Domain exceptions describe invalid states or transitions that should be
/// prevented regardless of which application workflow attempted the change.
/// </summary>
public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
