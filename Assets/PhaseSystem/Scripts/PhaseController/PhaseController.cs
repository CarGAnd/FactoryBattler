using UnityEngine;

namespace PhaseSystem { 
    public class PhaseController : MonoBehaviour 
    {
        [SerializeField]
        private PhaseMachine phaseMachine = new PhaseMachine();
        public PhaseMachine PhaseMachine { get => phaseMachine; set => phaseMachine = value; }

        void Start()
        {
            if (PhaseMachine.States == null || PhaseMachine.States.Length == 0) {
                Debug.LogError("No states assigned to PhaseController.");
                return;
            }

            phaseMachine.ChangeState(PhaseMachine.States[0]);
        }

        private void Update() {
            PhaseMachine.Update();
        }

        private void FixedUpdate() {
            PhaseMachine.FixedUpdate();
        }
    }
}