// scripts/Catcher.cs
using UnityEngine;

public class Catcher : MonoBehaviour
{
    [Header("Info prefab")]
    public GameObject infoPrefab;
    public GameObject infoInstance;

    [Header("Animation time control")]
    public float simulationSpeedMultiplier = 1.0f;

    public string targetName { get; set; }
    public double targetDistance { get; set; }

    [Header("Debug Status")]
    public bool isDragging = false;
    private float dragStartTime;
    private const float CLICK_THRESHOLD = 0.3f;


    private Transform targetTransform;
    private float currentVisualDistance;
    private double catchSpeedKmPerSec;

    private LineRenderer distanceLine;

    void Start()
    {
        distanceLine = gameObject.AddComponent<LineRenderer>();
        distanceLine.startWidth = 0.002f;
        distanceLine.endWidth = 0.002f;
        distanceLine.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        distanceLine.startColor = Color.yellow;
        distanceLine.endColor = Color.red;
    }

    public void SetTarget(Transform target, double initialDistance, double minutesToCatch)
    {
        targetTransform = target;
        targetDistance = initialDistance;

        double catchDurationSeconds = (minutesToCatch * 60.0) / simulationSpeedMultiplier;

        if (catchDurationSeconds > 0)
        {
            catchSpeedKmPerSec = targetDistance / catchDurationSeconds;
        }
        else
        {
            catchSpeedKmPerSec = 0;
            targetDistance = 0;
        }

        currentVisualDistance = (float)targetDistance * ObjectManager.Instance.distanceScale;
    }

    public void UpdateTargetDistance(double newDistance, double newMinutesToCatch)
    {
        SetTarget(targetTransform, newDistance, newMinutesToCatch);
    }

    void Update()
    {
        if (targetTransform != null)
        {
            transform.LookAt(targetTransform);
            transform.Rotate(0, 180, 0, Space.Self);

            if (targetDistance > 0)
            {
                targetDistance -= catchSpeedKmPerSec * Time.deltaTime;
                if (targetDistance <= 0) targetDistance = 0;
            }

            float targetVisual = (float)targetDistance * ObjectManager.Instance.distanceScale;
            currentVisualDistance = Mathf.Lerp(currentVisualDistance, targetVisual, Time.deltaTime * 5f);

            Vector3 approachDirection = Vector3.left; 
            if (Camera.main != null)
            {
                approachDirection = (-Camera.main.transform.right).normalized;
            }
            transform.position = targetTransform.position + approachDirection * currentVisualDistance;

            if (distanceLine != null)
            {
                distanceLine.SetPosition(0, transform.position);
                distanceLine.SetPosition(1, targetTransform.position);
            }
        }
    }

    public void OnDragStarted() { isDragging = true; dragStartTime = Time.time; }
    public void OnDragEnded()
    {
        if (Time.time - dragStartTime < CLICK_THRESHOLD) isDragging = false;
        else Invoke("ResetDragStatus", 0.2f);
    }
    private void ResetDragStatus() { isDragging = false; }

    public void onClick()
    {
        if (isDragging) return;
        if (infoInstance == null)
        {
            Transform camTransform = Camera.main.transform;
            Vector3 spawnPosition = transform.position - Vector3.forward * 1.2f + Vector3.right * 0.6f + Vector3.down * 0.1f;
            infoInstance = Instantiate(infoPrefab, spawnPosition, camTransform.rotation);

            var script = infoInstance.GetComponent<CatcherInfo>();
            if (script != null) script.UpdateInfo(targetName, 0, targetDistance);
        }
        else
        {
            infoInstance.SetActive(!infoInstance.activeSelf);
        }
    }

    void OnDestroy()
    {
        if (infoInstance != null) Destroy(infoInstance);
    }
}