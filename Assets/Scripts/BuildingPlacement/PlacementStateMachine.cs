using PlayerSystem;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class PlacementStateMachine : MonoBehaviour, IPlayerOwned
{
    [HideInInspector] public UnityEvent<IPlayerMode> modeChanged;

    [SerializeField][FoldoutGroup("Building Selector")][HideLabel] private BuildingSelector buildingSelector = new BuildingSelector();

    private FactoryGrid grid;
    
    private PlacementMode placementMode;
    private SelectionMode selectionMode;
    private DeleteMode deleteMode;
    private CombatMode combatMode;

    private IPlayer owner;
    private IPlayerGrid builder;

    public FactoryGrid Grid { get { return grid; } } 
    public IPlayer Owner { get => owner; set => SetOwner(value); }

    private IPlayerMode currentMode;

    private void Awake() {
        modeChanged = new UnityEvent<IPlayerMode>();
        //If we are in single player mode, assign the builder here
        //This is mainly for being able to test the PlayerModeManager without having to go through the multiplayer scene flow
        if(NetworkManager.Singleton == null) {
            grid = GameObject.FindAnyObjectByType<FactoryGrid>();
            builder = grid.transform.root.GetComponentInChildren<PlayerGrid>();
        }

        placementMode = new PlacementMode(grid, builder, this);
        selectionMode = new SelectionMode(grid);
        deleteMode = new DeleteMode(grid, builder, this);
        combatMode = new CombatMode();

        currentMode = selectionMode;
        currentMode.EnterMode();
    }

    private void SetOwner(IPlayer newOwner) {
        this.owner = newOwner;
        if(this.builder != null) {
            builder.Owner = newOwner;
        }
    }

    public void AssignGrid(IPlayerGrid builder) {
        this.builder = builder;
        this.grid = builder.FactoryGrid;
        placementMode.ChangeGrid(grid, builder);
        selectionMode = new SelectionMode(grid);
        deleteMode = new DeleteMode(grid, builder, this);
        builder.Owner = Owner;
    }

    public void OnNewBuildingSelected(GridObjectSO newBuilding) {
        if(!CanPlaceObjects()) {
            GoToSelectionMode();
        }
        else {
            GoToBuildMode(newBuilding);
        }
    }

    public void UpdateInput(MouseInput mouseInput) {
        if(!CanPlaceObjects()) {
            return;
        }
        if (mouseInput.isEnabled()) {
            buildingSelector.Update();
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

    private bool CanPlaceObjects() {
        return grid != null;
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

    public void GoToCombatMode() {
        ChangeMode(combatMode);
    }

    private void ChangeMode(IPlayerMode newMode) {
        currentMode.ExitMode();
        currentMode = newMode;
        currentMode.EnterMode();
        modeChanged?.Invoke(newMode);
    }

    private void OnEnable() {
        buildingSelector.selectedObjectChanged.AddListener(OnNewBuildingSelected);
    }

    private void OnDisable() {
        buildingSelector.selectedObjectChanged.RemoveListener(OnNewBuildingSelected);
    }
}

public interface IPlayerMode {
    void UpdateInput(MouseInput mouseInput, Vector3 mousePositionOnGrid);
    void EnterMode();
    void ExitMode();
}
