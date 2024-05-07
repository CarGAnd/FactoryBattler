using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MouseInput {
    private PlayerControls playerControls;

    public MouseInput(PlayerControls playerControls) {
        this.playerControls = playerControls;
        playerControls.Modules.Enable();
    }

    public Vector3 GetMousePosOnGrid(FactoryGrid grid) {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Vector3 worldPosition = grid.RaycastGridPlane(ray);
        return worldPosition;
    }

    public Vector2 GetMousePosition() {
        return Mouse.current.position.ReadValue();
    }

    public bool RotateModuleClockwise() {
        return playerControls.Modules.RotateModule.ReadValue<float>() < -0.01f;
    }

    public bool RotateModuleCounterClockwise() {
        return playerControls.Modules.RotateModule.ReadValue<float>() > 0.01f;
    }

    public bool PlaceModule() {
        return playerControls.Modules.PlaceModule.WasPressedThisFrame();
    }

    public bool PlaceModuleStarted() {
        return playerControls.Modules.PlaceModule.WasPressedThisFrame();
    }

    public bool PlaceModuleEnded() {
        return playerControls.Modules.PlaceModule.WasReleasedThisFrame();
    }

    public bool DeleteModule() {
        return playerControls.Modules.DeleteModule.WasPressedThisFrame();
    }

    public bool CancelModulePlacement() {
        return playerControls.Modules.CancelModulePlacement.WasPressedThisFrame();
    }
}
