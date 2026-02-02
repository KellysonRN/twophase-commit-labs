using TwoPhaseCommit.Labs.Domain.Interfaces;
using TwoPhaseCommit.Labs.Domain.ValueObjects;

namespace TwoPhaseCommit.Labs.Domain.Policies;

public class StatePolicy
    : IStateTransitionPolicy<State>
{
    public bool CanTransition(State from, State to)
        => (from, to) switch
        {
            (State.Pending, State.Active) => true,
            (State.Pending, State.Failed) => true,
            (State.Failed, State.Failed) => true,
            _ => false
        };
}
