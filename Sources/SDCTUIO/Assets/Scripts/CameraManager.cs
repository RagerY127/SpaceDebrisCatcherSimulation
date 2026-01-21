using System;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [SerializeField]
    private Camera _mainCamera;
    [SerializeField]
    private float _rotationSpeed;
    [SerializeField]
    private float _minCameraDistance;
    [SerializeField]
    private float _maxCameraDistance;
    [SerializeField]
    private float _trackingSpeed;
    [SerializeField]
    private ScreenTransformGesture _panGesture;
    [SerializeField]
    private ScreenTransformGesture _zoomGesture;

    private GameObject _followedDebris;

    private void OnEnable()
    {
        _panGesture.Transformed += OnPanGesture;
        _panGesture.TransformCompleted += OnPanCompletedGesture;
        _zoomGesture.Transformed += OnZoomGesture;
    }

    private void OnDisable()
    {
        _panGesture.Transformed -= OnPanGesture;
        _panGesture.TransformCompleted -= OnPanCompletedGesture;
        _zoomGesture.Transformed -= OnZoomGesture;
    }

    void Start()
    {
        Instance = this;
        _followedDebris = null;
    }

    void Update()
    {
        if (_followedDebris != null)
        {
            Vector3 debrisPosition = _followedDebris.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(debrisPosition);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _trackingSpeed);
        }
        else
        {

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

    private void OnPanGesture(object sender, EventArgs e)
    {
        Quaternion rotation = Quaternion.Euler(
            _panGesture.DeltaPosition.y / Screen.height * _rotationSpeed,
            _panGesture.DeltaPosition.x / Screen.width * _rotationSpeed,
            0
        );
        transform.localRotation *= rotation;

        UnfollowDebris();
    }

    private void OnPanCompletedGesture(object sender, EventArgs e)
    {

    }

    private void OnZoomGesture(object sender, EventArgs e)
    {
        _mainCamera.transform.localPosition *= 1.0f / _zoomGesture.DeltaScale;

        float distance = Math.Abs(_mainCamera.transform.localPosition.magnitude);
        if (distance < _minCameraDistance)
        {
            _mainCamera.transform.localPosition *= _minCameraDistance / distance;
        }

        if (distance > _maxCameraDistance)
        {
            _mainCamera.transform.localPosition *= _maxCameraDistance / distance;
        }
    }
}
