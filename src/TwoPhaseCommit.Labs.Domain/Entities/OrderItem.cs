using System.Runtime.CompilerServices;
using TwoPhaseCommit.Labs.Domain.Exceptions;
using TwoPhaseCommit.Labs.Domain.Interfaces;
using TwoPhaseCommit.Labs.Domain.Policies;
using TwoPhaseCommit.Labs.Domain.ValueObjects;

[assembly: InternalsVisibleTo("TwoPhaseCommit.Labs.Domain.Tests")]

namespace TwoPhaseCommit.Labs.Domain.Entities;


public class OrdemItem
{
    public Guid OrdemItemId { get; private set; }

    public State Status { get; private set; } = State.Pending;

    private readonly IStateTransitionPolicy<State> _stateTransitionPolicy;

    public OrdemItem(IStateTransitionPolicy<State> stateTransitionPolicy) => _stateTransitionPolicy = stateTransitionPolicy;

    public static OrdemItem Create(Guid ordemItemId)
    {
        return new OrdemItem(new StatePolicy())
        {
            OrdemItemId = ordemItemId
        };
    }  

    internal void Activate() => ChangeStatus(State.Active);

    internal void Fail() => ChangeStatus(State.Failed);

    public bool IsFailed()
        => Status == State.Failed;

    private void ChangeStatus(State newStatus)
    {
        if (!_stateTransitionPolicy.CanTransition(Status, newStatus))
            throw new InvalidStateTransitionException(
                nameof(OrdemItem),
                Status.ToString(),
                newStatus.ToString());

        Status = newStatus;
    }
}
