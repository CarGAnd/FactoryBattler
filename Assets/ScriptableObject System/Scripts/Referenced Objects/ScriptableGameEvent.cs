using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SOS {
    [CreateAssetMenu (fileName = "GameEvent", menuName = "SOS/Game Event")]
    public class ScriptableGameEvent : ScriptableObject
    {
        public Action DynamicInvoked;
    }
}