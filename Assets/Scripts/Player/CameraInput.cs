using UnityEngine;
using UnityEngine.InputSystem;

//This class is not fully developed yet, but serves as a placeholder to implement the camera system. will definitely be revised
public class CameraInput
{
    private PlayerControls playerControls;

    public CameraInput (PlayerControls playerControls) {
        this.playerControls = playerControls;
        playerControls.Camera.Enable();
    }

    public Vector3 GetMovementInput()
    {
        Vector2 WASDinput = playerControls.Camera.MoveCamera.ReadValue<Vector2>();
        Vector2 mouseInput = GetMouseMovement();
        Vector3 totalInput = new Vector3(WASDinput.x + mouseInput.x, 0, WASDinput.y + mouseInput.y);
        totalInput.Normalize();

        return totalInput;
    }

    public float GetZoomInput()
    {
        float delta = playerControls.Camera.Zoom.ReadValue<float>();
        return delta;
    }

    private Vector2 GetMouseMovement() {
        if (!MouseIsInsideGameWindow()) {
            return Vector3.zero; 
        }

        //How much of the screen is considered the border on each side (percentage)
        float boundsX = 0.05f;
        float boundsY = 0.05f;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        Vector2 mouseDir = new Vector2();
        if(mousePos.x < Screen.width * boundsX || 
           mousePos.x > Screen.width - boundsX * Screen.width ||
           mousePos.y < Screen.height * boundsY || 
           mousePos.y > Screen.height - boundsY * Screen.height) 
        {
            mouseDir.x = mousePos.x - Screen.width / 2.0f;
            mouseDir.y = mousePos.y - Screen.height / 2.0f;
        }

        mouseDir.Normalize();
        return mouseDir;
    }

    private bool MouseIsInsideGameWindow() {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        return mousePos.x < Screen.width && mousePos.x > 0 && mousePos.y > 0 && mousePos.y < Screen.height;
    }
}
