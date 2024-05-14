using PlayerSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;

public class PlayerModeManager : NetworkBehaviour, IPlayerOwned
{
    [HideInInspector] public UnityEvent<IPlayerMode> modeChanged;

    [FoldoutGroup("Systems")]
    [SerializeField] private FactoryGrid grid;

    [SerializeField][FoldoutGroup("Building Selector")][HideLabel] private BuildingSelector buildingSelector = new BuildingSelector();
    
    private PlacementMode placementMode;
    private SelectionMode selectionMode;
    private DeleteMode deleteMode;

    private PlayerControls playerControls; 
    private MouseInput mouseInput;
    private CameraInput cameraInput;
    private CameraController cameraController;

    private IPlayer owner;
    private IBuilder builder;

    public FactoryGrid Grid { get { return grid; } } 
    public IPlayer Owner { get => owner; set => SetOwner(value); }

    private IPlayerMode currentMode;

    private void Awake() {
        //If we are in single player mode, assign the builder here
        //This is mainly for being able to test the PlayerModeManager without having to go through the multiplayer scene flow
        if(NetworkManager.Singleton == null) {
            builder = grid.transform.root.GetComponentInChildren<Builder>();
        }

        playerControls = new PlayerControls();
        EnableControls();

        mouseInput = new MouseInput(playerControls);
        cameraInput = new CameraInput(playerControls);

        cameraController = new CameraController(cameraInput);

        placementMode = new PlacementMode(grid, builder, this);
        selectionMode = new SelectionMode(grid);
        deleteMode = new DeleteMode(grid, builder, this);

        currentMode = selectionMode;
        currentMode.EnterMode();
    }

    public void DisableControls() {
        playerControls.Disable();
    }

    public void EnableControls() {
        playerControls.Enable();
    }

    private void SetOwner(IPlayer newOwner) {
        this.owner = newOwner;
        if(this.builder != null) {
            builder.Owner = newOwner;
        }
    }

    public void UpdateGridReference(FactoryGrid grid, IBuilder builder) {
        placementMode.ChangeGrid(grid, builder);
        selectionMode = new SelectionMode(grid);
        deleteMode = new DeleteMode(grid, builder, this);
        this.builder = builder;
        this.grid = grid;
        builder.Owner = Owner;
    }

    private void OnEnable() {
        buildingSelector.selectedObjectChanged.AddListener(OnNewBuildingSelected);
    }

    private void OnDisable() {
        buildingSelector.selectedObjectChanged.RemoveListener(OnNewBuildingSelected);
    }

    private void OnNewBuildingSelected(GridObjectSO newBuilding) {
        if(!CanPlaceObjects()) {
            GoToSelectionMode();
        }
        else {
            GoToBuildMode(newBuilding);
        }
    }

    private void Update() {
        if(!CanPlaceObjects()) {
            return;
        }
        buildingSelector.Update();
        cameraController.Update();
        Vector3 mousePositionOnGrid = mouseInput.GetMousePosOnGrid(grid);
        currentMode.UpdateInput(mouseInput, mousePositionOnGrid);
        if (Input.GetKeyDown(KeyCode.Escape)) {
            GoToSelectionMode();
        }
        if (Input.GetKeyDown(KeyCode.X)) {
            GoToDeleteMode();
        }
    }

    private bool CanPlaceObjects() {
        return grid != null && (IsOwner || NetworkManager.Singleton == null);
    }

    public void GoToBuildMode(GridObjectSO building) {
        ChangeMode(placementMode);
        placementMode.SetSelectedBuilding(building);
    }

    public void GoToSelectionMode() {
        ChangeMode(selectionMode);
    }

    public void GoToDeleteMode() {
        ChangeMode(deleteMode);
    }

    private void ChangeMode(IPlayerMode newMode) {
        currentMode.ExitMode();
        currentMode = newMode;
        currentMode.EnterMode();
        modeChanged?.Invoke(newMode);
    }
}

public interface IPlayerMode {
    void UpdateInput(MouseInput mouseInput, Vector3 mousePositionOnGrid);
    void EnterMode();
    void ExitMode();
}

