using PlayerSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;

public class PlayerController : NetworkBehaviour, IPlayerOwned
{
    [SerializeField] private PlacementStateMachine placementStateMachine;

    private PlayerControls playerControls; 
    private MouseInput mouseInput;
    private CameraInput cameraInput;
    private CameraController cameraController;

    private IPlayer owner;

    public IPlayer Owner { get => owner; set => SetOwner(value); }

    private void Awake() {
        playerControls = new PlayerControls();
        EnableControls();

        mouseInput = new MouseInput(playerControls);
        cameraInput = new CameraInput(playerControls);

        cameraController = new CameraController(cameraInput);
    }

    public void DisableControls() {
        playerControls.Disable();
    }

    public void EnableControls() {
        playerControls.Enable();
    }

    public void DisableModuleControls() {
        playerControls.Modules.Disable();
    }

    public void EnableModuleControls() {
        playerControls.Modules.Enable();
    }

    private void SetOwner(IPlayer newOwner) {
        this.owner = newOwner;
        placementStateMachine.Owner = newOwner;
    }

    public void AssignGrid(IPlayerGrid builder) {
        placementStateMachine.AssignGrid(builder);
    }

    private void Update() {
        //Only update if we own this player object (multiplayer) or if we are in single player mode (mostly for editor testing)
        if(IsOwner || NetworkManager.Singleton == null) {
            cameraController.Update();
            placementStateMachine.UpdateInput(mouseInput);
        }
    }
}
