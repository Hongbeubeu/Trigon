using UnityEngine;

public class CameraService : ICameraService
{
    public Camera MainCamera { get; }

    public CameraService(Camera mainCamera)
    {
        MainCamera = mainCamera;
    }
}
