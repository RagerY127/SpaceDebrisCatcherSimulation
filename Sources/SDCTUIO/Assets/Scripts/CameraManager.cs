using System;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [SerializeField]
    private Camera MainCamera;
    [SerializeField]
    private float RotationSpeed;
    [SerializeField]
    private float MinCameraDistance;
    [SerializeField]
    private float MaxCameraDistance;
    [SerializeField]
    private float TrackingSpeed;
    [SerializeField]
    private ScreenTransformGesture PanGesture;
    [SerializeField]
    private ScreenTransformGesture ZoomGesture;

    private GameObject FollowedDebris;

    private void OnEnable()
    {
        PanGesture.Transformed += OnPanGesture;
        PanGesture.TransformCompleted += OnPanCompletedGesture;
        ZoomGesture.Transformed += OnZoomGesture;
    }

    private void OnDisable()
    {
        PanGesture.Transformed -= OnPanGesture;
        PanGesture.TransformCompleted -= OnPanCompletedGesture;
        ZoomGesture.Transformed -= OnZoomGesture;
    }

    void Start()
    {
        Instance = this;
        FollowedDebris = null;
    }

    void Update()
    {
        if (FollowedDebris != null)
        {
            Vector3 debrisPosition = FollowedDebris.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(debrisPosition);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * TrackingSpeed);
        }
        else
        {

        }
    }

    public void FollowDebris(GameObject debris)
    {
        FollowedDebris = debris;
    }

    public void UnfollowDebris()
    {
        FollowedDebris = null;
    }

    private void OnPanGesture(object sender, System.EventArgs e)
    {
        Quaternion rotation = Quaternion.Euler(
            PanGesture.DeltaPosition.y / Screen.height * RotationSpeed,
            PanGesture.DeltaPosition.x / Screen.width * RotationSpeed,
            0
        );
        transform.localRotation *= rotation;

        UnfollowDebris();
    }

    private void OnPanCompletedGesture(object sender, System.EventArgs e)
    {

    }

    private void OnZoomGesture(object sender, System.EventArgs e)
    {
        MainCamera.transform.localPosition *= 1.0f / ZoomGesture.DeltaScale;

        float distance = Math.Abs(MainCamera.transform.localPosition.magnitude);
        if (distance < MinCameraDistance)
        {
            MainCamera.transform.localPosition *= MinCameraDistance / distance;
        }

        if (distance > MaxCameraDistance)
        {
            MainCamera.transform.localPosition *= MaxCameraDistance / distance;
        }
    }
}
