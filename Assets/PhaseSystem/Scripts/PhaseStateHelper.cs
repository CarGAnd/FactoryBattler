using UnityEngine;
using UnityEngine.Events;

namespace PhaseSystem {
    public static class PhaseStateHelper {
        public static UnityEvent<string> PhaseStateChangedTo = new UnityEvent<string>();
        public static UnityEvent<string, string> PhaseStateChangedFromTo = new UnityEvent<string, string>();
        private static string currentStateName = "name";

        public static string GetCurrentStateName() {
            return currentStateName;
        }

        public static void SetCurrentStateName(string newStateName) {
            PhaseStateChangedTo?.Invoke(newStateName);
            PhaseStateChangedFromTo.Invoke(currentStateName, newStateName);

            Debug.Log($"Change from {currentStateName} to {newStateName}.");
            currentStateName = newStateName;
        }
    }
}