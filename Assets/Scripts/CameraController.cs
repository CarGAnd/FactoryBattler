using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController
{
    private CinemachineVirtualCamera virtualCamera;
    private CameraInput cameraInput;

    private float cameraMoveSpeed = 15;

    public CameraController(CameraInput camInput) {
        this.cameraInput = camInput;
        //TODO: figure out camera management to avoid having to use Camera.main
        this.virtualCamera = Camera.main.GetComponent<CinemachineVirtualCamera>();
    }

    public void Update()
    {
        Vector3 cameraMoveDir = cameraInput.GetMovementInput().normalized;
        UpdateCameraPosition(cameraMoveDir * Time.deltaTime * cameraMoveSpeed);

        float zoomDelta = cameraInput.GetZoomInput();
        //UpdateCameraZoom(zoomDelta * Time.deltaTime);
    }


    public void UpdateCameraPosition(Vector3 delta)
    {
        virtualCamera.transform.position += (Quaternion.Euler(virtualCamera.transform.root.eulerAngles.y * Vector3.up) * delta);
    }

    /*public void UpdateCameraZoom(float delta)
    {
        // Use inputManager to get zoom input and apply it to the camera's FOV or Orthographic Size.
        // Adjust zoom within limits defined in zoomSettings (Design Doc: Camera Zoom).
        if (virtualCamera.m_Lens.Orthographic) {
            virtualCamera.m_Lens.OrthographicSize -= delta * zoomSettings.zoomSpeed;
            if(virtualCamera.m_Lens.OrthographicSize > zoomSettings.maxZoom) {
                virtualCamera.m_Lens.OrthographicSize = zoomSettings.maxZoom;
            }
            if (virtualCamera.m_Lens.OrthographicSize < zoomSettings.minZoom) {
                virtualCamera.m_Lens.OrthographicSize = zoomSettings.minZoom;
            }
        }
        else {
            virtualCamera.transform.position += virtualCamera.transform.forward * delta * zoomSettings.zoomSpeed;
        }
    }*/

}
