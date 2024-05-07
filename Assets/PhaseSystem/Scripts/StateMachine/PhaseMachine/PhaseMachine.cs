using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PhaseSystem {
    [Serializable][InlineProperty][HideLabel]
    public class PhaseMachine : StateMachine
    {
        [SerializeReference][ListDrawerSettings(ShowFoldout = false, DefaultExpandedState = true)][HideReferenceObjectPicker]
        internal IState[] states = new IState[0];
        private IState currentState;
        public override IState CurrentState { get => currentState; set => currentState = value; }
        public override IState[] States { get => states; protected set => states = value; }
    }
}
        