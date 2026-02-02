namespace TwoPhaseCommit.Labs.Domain.Exceptions;

/// <summary>
/// HTTP 422 Unprocessable Entity
/// </summary>
public sealed class BusinessRuleViolationException(string message) : DomainException(message)
{
}
