using System;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [SerializeField]
    private Camera _mainCamera;
    [SerializeField]
    private float _rotationSpeed;
    
    public float minCameraDistance;
    public float maxCameraDistance;

    [SerializeField]
    private float _trackingSpeed;
    [SerializeField]
    private ScreenTransformGesture _panAndRotateGesture;
    [SerializeField]
    private ScreenTransformGesture _zoomGesture;

    public float distance { get; private set; }

    private GameObject _followedDebris;

    private void OnEnable()
    {
        _panAndRotateGesture.Transformed += OnPanAndRotateGesture;
        _zoomGesture.Transformed += OnZoomGesture;
    }

    private void OnDisable()
    {
        _panAndRotateGesture.Transformed -= OnPanAndRotateGesture;
        _zoomGesture.Transformed -= OnZoomGesture;
    }

    void Start()
    {
        Instance = this;
        _followedDebris = null;

        OnZoomGesture(null, null);
    }

    void Update()
    {
        if (_followedDebris != null)
        {
            Vector3 debrisPosition = _followedDebris.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(debrisPosition);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _trackingSpeed);
        }
    }

    public void FollowDebris(GameObject debris)
    {
        _followedDebris = debris;
    }

    public void UnfollowDebris()
    {
        _followedDebris = null;
    }

    private void OnPanAndRotateGesture(object sender, EventArgs e)
    {
        // pan gesture
        if (EventSystem.current != null)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
        }

        Quaternion rotation = Quaternion.Euler(
            _panAndRotateGesture.DeltaPosition.y / Screen.height * _rotationSpeed,
            _panAndRotateGesture.DeltaPosition.x / Screen.width * _rotationSpeed,
            _panAndRotateGesture.DeltaRotation
        );
        transform.localRotation *= rotation;

        UnfollowDebris();
    }

    private void OnZoomGesture(object sender, EventArgs e)
    {
        _mainCamera.transform.localPosition *= 1.0f / _zoomGesture.DeltaScale;

        float distance = Math.Abs(_mainCamera.transform.localPosition.magnitude);
        if (distance < minCameraDistance)
        {
            _mainCamera.transform.localPosition *= minCameraDistance / distance;
        }

        if (distance > maxCameraDistance)
        {
            _mainCamera.transform.localPosition *= maxCameraDistance / distance;
        }
        this.distance = distance;
    }
}
