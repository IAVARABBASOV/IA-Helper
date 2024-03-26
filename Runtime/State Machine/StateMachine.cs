using System;

namespace IA.FSM
{
    public class StateMachine
    {
        private IState currentState { get; set; }

        public StateMachine(IState state) => SetState(state);

        public void SetState(IState state) => currentState = state;

        public void Update() => SetState(currentState.Run());

        public bool IsState(Type type) => type == currentState.GetType();
    }
}
