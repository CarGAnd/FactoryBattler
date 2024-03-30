using System.Collections;
using UnityEngine;

public class PlayerModeManager : MonoBehaviour
{
    [SerializeField] private MouseInput mouseInput;
    [SerializeField] private BuildingSelector buildingSelector;

    [Header("Systems")]
    [SerializeField] private ModulePlacer modulePlacer;
    [SerializeField] private FactoryGrid grid;
    
    [Header("Input modes")]
    [SerializeField] private PlacementMode PlacementMode;
    [SerializeField] private SelectionMode SelectionMode;
    [SerializeField] private DeleteMode DeleteMode;

    public FactoryGrid Grid { get { return grid; } }

    private IMouseMode currentMode;

    private void Awake() {
        PlacementMode.Initialize(grid, modulePlacer, this);
        SelectionMode.Initialize(grid);
        DeleteMode.Initialize(grid, this);
    }

    private void Start() {
        currentMode = SelectionMode;
        currentMode.EnterMode();
    }

    private void OnEnable() {
        buildingSelector.selectedObjectChanged.AddListener(OnNewBuildingSelected);
    }

    private void OnDisable() {
        buildingSelector.selectedObjectChanged.RemoveListener(OnNewBuildingSelected);
    }

    private void OnNewBuildingSelected(GridObjectSO newBuilding) {
        if(newBuilding == null) {
            GoToSelectionMode();
        }
        else {
            GoToBuildMode(newBuilding);
        }
    }

    private void Update() {
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


