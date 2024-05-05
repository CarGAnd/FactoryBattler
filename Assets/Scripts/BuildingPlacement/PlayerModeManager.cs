using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerModeManager : NetworkBehaviour
{
    [Header("Systems")]
    [SerializeField] private Builder builder;
    [SerializeField] private FactoryGrid grid;

    [Header("Input")]
    [SerializeField] private MouseInput mouseInput;
    [SerializeField] private BuildingSelector buildingSelector;
    
    [Header("Input modes")]
    [SerializeField] private PlacementMode PlacementMode;
    [SerializeField] private SelectionMode SelectionMode;
    [SerializeField] private DeleteMode DeleteMode;

    public FactoryGrid Grid { get { return grid; } }

    private IMouseMode currentMode;

    private void Awake() {
        PlacementMode.Initialize(grid, builder, this);
        SelectionMode.Initialize(grid);
        DeleteMode.Initialize(grid, this);

        currentMode = SelectionMode;
        currentMode.EnterMode();
    }

    public void UpdateGridReference(FactoryGrid grid, Builder builder) {
        PlacementMode.ChangeGrid(grid, builder);
        SelectionMode.Initialize(grid);
        DeleteMode.Initialize(grid, this);
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
        PlacementMode.SetSelectedBuilding(building);
        ChangeMode(PlacementMode);
    }

    public void GoToSelectionMode() {
        ChangeMode(SelectionMode);
    }

    public void GoToDeleteMode() {
        ChangeMode(DeleteMode);
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


