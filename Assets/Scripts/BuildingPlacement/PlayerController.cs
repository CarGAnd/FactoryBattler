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

    private void SetOwner(IPlayer newOwner) {
        this.owner = newOwner;
        placementStateMachine.Owner = newOwner;
    }

    public void AssignGrid(FactoryGrid grid, IBuilder builder) {
        placementStateMachine.AssignGrid(grid, builder);
    }

    private void Update() {
        if(IsOwner || NetworkManager.Singleton == null) {
            cameraController.Update();
            placementStateMachine.UpdateInput(mouseInput);
        }
    }
}
