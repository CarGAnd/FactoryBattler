using UnityEngine;

namespace PhaseSystem {
    public interface IActive
    {
        bool IsActive { get; set; }

        bool CheckIfActive() {
            if (IsActive)
                return IsActive;

            #if UNITY_EDITOR
                if (!Application.isPlaying)
                    return true;
            #endif

            return false;
        }
    }
}