namespace TwoPhaseCommit.Labs.Domain.Exceptions;

/// <summary>
/// HTTP 409 Conflict
/// </summary>
public sealed class InvalidStateTransitionException(
    string entityName,
    string fromState,
    string toState) : DomainException(
        $"Invalid state transition for {entityName}: " +
            $"from '{fromState}' to '{toState}'.")
{
}
