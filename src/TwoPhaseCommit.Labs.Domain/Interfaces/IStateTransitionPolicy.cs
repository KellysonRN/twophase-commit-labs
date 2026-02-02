namespace TwoPhaseCommit.Labs.Domain.Interfaces;

public interface IStateTransitionPolicy<TState>
{
    bool CanTransition(TState from, TState to);
}
