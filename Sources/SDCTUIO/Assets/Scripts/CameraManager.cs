using System;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private Camera MainCamera;
    [SerializeField]
    private float RotationSpeed;
    [SerializeField]
    private float MinCameraDistance;
    [SerializeField]
    private float MaxCameraDistance;
    [SerializeField]
    private ScreenTransformGesture PanGesture;
    [SerializeField]
    private ScreenTransformGesture ZoomGesture;

    private void OnEnable()
    {
        PanGesture.Transformed += OnPanGesture;
        ZoomGesture.Transformed += OnZoomGesture;
    }

    private void OnDisable()
    {
        PanGesture.Transformed -= OnPanGesture;
        ZoomGesture.Transformed -= OnZoomGesture;
    }

    void Update()
    {
        
    }

    private void OnPanGesture(object sender, System.EventArgs e)
    {
        print("Panning camera");
        Quaternion rotation = Quaternion.Euler(
            -PanGesture.DeltaPosition.y / Screen.height * RotationSpeed,
            PanGesture.DeltaPosition.x / Screen.width * RotationSpeed,
            0
        );
        transform.localRotation *= rotation;
    }

    private void OnZoomGesture(object sender, System.EventArgs e)
    {
        print("Zooming camera");
        print(ZoomGesture.DeltaScale);

        MainCamera.transform.localPosition *= 1.0f / ZoomGesture.DeltaScale;

        float distance = Math.Abs(MainCamera.transform.localPosition.z);
        if (distance < MinCameraDistance)
        {
            MainCamera.transform.localPosition = new Vector3(
                0, 0, -MinCameraDistance
            );
        }

        if (distance > MaxCameraDistance)
        {
            MainCamera.transform.localPosition = new Vector3(
                0, 0, -MaxCameraDistance
            );
        }
    }
}
