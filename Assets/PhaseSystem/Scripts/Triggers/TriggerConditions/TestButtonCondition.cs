using Sirenix.OdinInspector;

namespace PhaseSystem {
    public class TestButtonCondition : TriggerCondition {
        [Button]
        private void Trigger() {
            IsTrue = true;
        }

        public TestButtonCondition() : base() {
            IsTrue = false;
        }
    }
}