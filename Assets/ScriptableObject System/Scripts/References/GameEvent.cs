using System;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace SOS {
    [Serializable]
    [InitializeOnLoadAttribute]
    public class GameEvent : SOSReference<ScriptableGameEvent>
    {
        // Local event, allows to disallow listening from this reference.
        private Action GameEventInvoked;
        private bool allowInvoking = true;
        private bool allowListening = true;
        [NonSerialized]
        private bool isSubscribed = false;
        [NonSerialized]
        private int subscriptionCounter = 0; 

        // Scriptable Object of ScriptableGameEvent type.
        [ShowInInspector]
        [HorizontalGroup("row", Width = 0.14f)]
        [PropertyOrder(100)]
        [DrawWithUnity]
        [HideLabel]
        [Required]
        [DisableIf("@isSubscribed")]
        protected override ScriptableGameEvent variable {
            get { return scriptableObject; }
            set { scriptableObject = value; SubscribeToScriptableGameEvent(); }
        }
        
        GameEvent() {
            SubscribeToPlayModeStateChanged();
            
        }

        // Allows knowing when the game starts from a non-monobehaviour.
        void SubscribeToPlayModeStateChanged()
        {
            EditorApplication.playModeStateChanged += SubscribeToScriptableGameEventBasedOnState;
        }

        // Subscripes to the current ScriptableGameObject when entering PlayMode and unsubscripes when leaving. 
        private void SubscribeToScriptableGameEventBasedOnState(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredPlayMode) {
                SubscribeToScriptableGameEvent();
            } else if (change == PlayModeStateChange.ExitingPlayMode) {
                UnsubscribeFromScriptableGameEvent();
            } else {
                // Debug.Log($"This change happened: {change}.");
            }
        }

        // Allows subscribing to the current ScriptableGameEvent when the inspector initializes, also updates buttons to show current state.
        private void OnInspectorInit() {
            if (!EditorApplication.isPlaying) {
                SubscribeToScriptableGameEvent();
            }

            UpdateInvokingButton();
            UpdateListeningButton();
        }

        // Test element, to ensure that subscriptions and unsubscriptions are equal.
        private void CountSubscriptions(int value) {
            subscriptionCounter += value;

            if (subscriptionCounter > 1 || subscriptionCounter < 0)
                Debug.LogWarning($"Incorrect subscription count at {subscriptionCounter}!");
        }

        private void SubscribeToScriptableGameEvent() {
            // Debug.Log("Attempting to subscribe.");
            if (variable == null)
                return;

            // Debug.Log($"Variable is not null and isSubscribed is {isSubscribed}.");
            if (isSubscribed)
                return;
                
            // Debug.LogWarning("Subscribed.");
            // CountSubscriptions(1);
            variable.DynamicInvoked += OnGameEventInvoked;
            isSubscribed = true;
        }

        // Unsubscribes only if currently subscribed.
        private void UnsubscribeFromScriptableGameEvent() {
            // Debug.Log("Attempting to unsubscribe.");
            if (variable == null)
                return;

            if (variable.DynamicInvoked == null)
                return;

            Delegate[] clientList = variable.DynamicInvoked.GetInvocationList();

            if (clientList == null)
                return;

            foreach (Delegate client in clientList) {
                if (client as Action == OnGameEventInvoked)
                {
                    // Debug.LogWarning("Unsubscribed.");
                    // CountSubscriptions(-1);
                    variable.DynamicInvoked -= client as Action;
                    isSubscribed = false;
                }
            }
        }

        // Fired when the current ScriptableGameEvent Action is invoked.
        private void OnGameEventInvoked()
        {
            if (allowListening)
                GameEventInvoked?.Invoke();
        }

        [Button(ButtonSizes.Small, Name = "Subscribe")]
        [HorizontalGroup("row", Width = 0.195f)]
        [ShowIf("@variable && !isSubscribed")]
        [PropertyOrder(90)]
        [GUIColor(0, 0.8f, 0)]
        [Tooltip("Subscribe from the current ScriptableObject.")]
        private void Subscribe() {
            SubscribeToScriptableGameEvent();
        }

        // Unsubscribe button, needed to be clicked to ensure reference is unsubscribed to the current ScriptableGameEvent before changing it.
        [Button(ButtonSizes.Small, Name = "Unsubscribe")]
        [HorizontalGroup("row", Width = 0.195f)]
        [PropertyOrder(90)]
        [GUIColor("@SettingsHelperSOS.GetSettings().GameEventUnsubscribeColor")]
        [ShowIf("@isSubscribed")]
        [Tooltip("Unsubscribe from the current ScriptableObject in order to select a new.")]
        private void Unsubscribe () {
            UnsubscribeFromScriptableGameEvent();
        }

        // Toggle between allowing and disallowing invocation of the ScriptableGameEvent Action from this reference.
        [Button("$toggleNameInvoking"), GUIColor("$toggleColorInvoking")]
        [HorizontalGroup("row", Width = 0.195f)]
        [ShowIf("@variable")]
        [Tooltip("Toggle between allowing event invocations from this reference.")]
        //[OnInspectorInit("@OnUpdateInvokingColor()")] 
        private void ToggleAllowInvoking () {
            allowInvoking = !allowInvoking;
            UpdateInvokingButton();
        }

        // Toggle between allowing and disallowing listening to the ScriptableGameEvent Action from this reference.
        [Button("$toggleNameListening")]
        [GUIColor("$toggleColorListening")]
        [HorizontalGroup("row", Width = 0.195f)]
        [Tooltip("Toggle between allowing this reference to listen to its event invocations.")]
        //[OnInspectorInit("@UpdateListeningColor()")] 
        [ShowIf("@variable")]
        private void ToggleAllowListening () {
            allowListening = !allowListening;
            UpdateListeningButton();
        }

        // Button and method for invoking the ScriptableGameEvent Action.
        [Button]
        [GUIColor(0, 1, 0.5058824f)]
        [HorizontalGroup("row", Width = 0.195f)]
        [ShowIf("@variable")]
        [Tooltip("Invoke the current ScriptableGameEvent.")]
        public void Invoke() {
            if (variable != null && allowInvoking)
                variable.DynamicInvoked?.Invoke();
        }

        // Updates text and colors of the allow Invocation toggle button.
        private void UpdateInvokingButton() {
            ToggleColor(ref toggleColorInvoking, allowInvoking);
            if (allowInvoking)
                toggleNameInvoking = "Invoking"; 
            else
                toggleNameInvoking = "Not Invoking";
        }

        // Updates text and colors of the allow listening toggle button.
        private void UpdateListeningButton() {
            ToggleColor(ref toggleColorListening, allowListening);
            if (allowListening)
                toggleNameListening = "Listening";
            else
                toggleNameListening = "Not Listening";
        }

        private void ToggleColor(ref Color colorToToggle, bool state) {
            if (state)
                colorToToggle = new Color(0, 0.8f, 0);
            else
                colorToToggle = new Color32(243, 109, 134, 255);
        }

        public static implicit operator Action(GameEvent gameEvent) => gameEvent.EventInvoked;

        // Event to subscribe to, scripts only.
        [HideInInspector]
        public Action EventInvoked {
            get {
                if (variable != null)
                    return GameEventInvoked;

                return null;
            }
            set {
                if (variable != null)
                    GameEventInvoked = value;
            }
        }

        // Small denotion of type, only for inspector.
        [ShowInInspector]
        [HorizontalGroup("row", Width = 0.06f)]
        [PropertyOrder(-1)]
        [GUIColor("$denotionColor")]
        [HideLabel]
        [DisplayAsString]
        [Sirenix.OdinInspector.ReadOnly]
        [OnInspectorInit("@OnInspectorInit()")]
        protected override string denotation {
            get {
                return "GE";
            }
        }

        protected override Color32 denotionColor {
            get {
                return SettingsHelperSOS.GetSettings().GameEventDenotionColor;
            }
        }

        // Creation of Object.
        [HorizontalGroup("row", Width = 0.2375f)]
        [LabelWidth(150)]
        [Button("Create"), GUIColor(0, 0.9f, 0)]
        [HideIf("@variable || string.IsNullOrEmpty(name)")]
        protected override void CreateScriptableObject () {
            sOSaveLocation = SettingsHelperSOS.GetSettings().GameEventRefPath;
            base.CreateScriptableObject();
        }

        // Fields used for odin inspector visuals
        #region Private Fields

        #pragma warning disable 0414
        private Color toggleColorInvoking = new Color(0, 0.8f, 0);
        private Color toggleColorListening = new Color(0, 0.8f, 0);
        private string toggleNameListening = "Listening";
        private string toggleNameInvoking = "Invoking";

        #pragma warning restore 0414

        #endregion

    }
}