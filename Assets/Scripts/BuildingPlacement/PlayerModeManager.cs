using PlayerSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using Unity.Netcode;

public class PlayerModeManager : NetworkBehaviour, IPlayerOwned
{
    [FoldoutGroup("Systems")]
    [SerializeField] private IBuilder builder;
    [FoldoutGroup("Systems")]

    [SerializeField] private FactoryGrid grid;

    [SerializeField][FoldoutGroup("Building Selector")][HideLabel] private BuildingSelector buildingSelector = new BuildingSelector();
    
    [Header("Input modes")]
    [SerializeField] private PlacementMode placementMode;

    private SelectionMode selectionMode;
    private DeleteMode deleteMode;
    private MouseInput mouseInput;

    [SerializeReference]
    private IPlayer owner;

    public FactoryGrid Grid { get { return grid; } } 

    public IPlayer Owner { get => owner; set => owner = value; }

    private IMouseMode currentMode;
    private CameraInput cameraInput;
    private PlayerControls playerControls; 

    private void Awake() {
        playerControls = new PlayerControls();
        playerControls.Enable();

        mouseInput = new MouseInput(playerControls);
        cameraInput = new CameraInput(playerControls);

        placementMode.Initialize(grid, builder, this);
        selectionMode = new SelectionMode(grid);
        deleteMode = new DeleteMode(grid, this);

        currentMode = selectionMode;
        currentMode.EnterMode();
    }


    public void UpdateGridReference(FactoryGrid grid, IBuilder builder) {
        placementMode.ChangeGrid(grid, builder);
        selectionMode = new SelectionMode(grid);
        deleteMode = new DeleteMode(grid, this);
        this.builder = builder;
        this.grid = grid;
    }

    private void OnEnable() {
        buildingSelector.selectedObjectChanged.AddListener(OnNewBuildingSelected);
    }

    private void OnDisable() {
        buildingSelector.selectedObjectChanged.RemoveListener(OnNewBuildingSelected);
        GoToSelectionMode();
    }

    private void OnNewBuildingSelected(GridObjectSO newBuilding) {
        if(newBuilding == null || !IsOwner) {
            GoToSelectionMode();
        }
        else {
            GoToBuildMode(newBuilding);
        }
    }

    private void Update() {
        if(grid == null || !IsOwner) {
            return;
        }
        buildingSelector.Update();
        Vector3 mousePositionOnGrid = mouseInput.GetMousePosOnGrid(grid);
        currentMode.UpdateInput(mouseInput, mousePositionOnGrid);
        if (Input.GetKeyDown(KeyCode.Escape)) {
            GoToSelectionMode();
        }
        if (Input.GetKeyDown(KeyCode.X)) {
            GoToDeleteMode();
        }
    }

    public void GoToBuildMode(GridObjectSO building) {
        placementMode.SetSelectedBuilding(building);
        ChangeMode(placementMode);
    }

    public void GoToSelectionMode() {
        ChangeMode(selectionMode);
    }

    public void GoToDeleteMode() {
        ChangeMode(deleteMode);
    }

    private void ChangeMode(IMouseMode newMode) {
        currentMode.ExitMode();
        currentMode = newMode;
        currentMode.EnterMode();
    }
}

public interface IMouseMode {
    void UpdateInput(MouseInput mouseInput, Vector3 mousePositionOnGrid);
    void EnterMode();
    void ExitMode();
}


