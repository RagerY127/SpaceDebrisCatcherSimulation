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

    private Vector3 fixedApproachDirection;

    private LineRenderer distanceLine;

    private CatcherInfo activeInfoScript;
    private const double VISUAL_STOP_DISTANCE = 4.3;

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
        targetDistance = initialDistance + VISUAL_STOP_DISTANCE;

        double catchDurationSeconds = (minutesToCatch * 60.0) / simulationSpeedMultiplier;

        if (catchDurationSeconds > 0)
        {
            catchSpeedKmPerSec = initialDistance / catchDurationSeconds;
        }
        else
        {
            catchSpeedKmPerSec = 0;
        }

        currentVisualDistance = (float)targetDistance * ObjectManager.Instance.distanceScale;

        if (Camera.main != null)
        {
            fixedApproachDirection = (-Camera.main.transform.right).normalized;
            
            fixedApproachDirection.y = 0;
            fixedApproachDirection.Normalize();
        }
        else
        {
            fixedApproachDirection = Vector3.left;
        }
    }

    public void UpdateTargetDistance(double newDistance)
    {
        targetDistance = newDistance + VISUAL_STOP_DISTANCE;

        currentVisualDistance = (float)targetDistance * ObjectManager.Instance.distanceScale;
    }

    void Update()
    {
        if (targetTransform != null)
        {
            transform.LookAt(targetTransform);
            transform.Rotate(0, 180, 0, Space.Self);

            if (targetDistance > VISUAL_STOP_DISTANCE)
            {
                targetDistance -= catchSpeedKmPerSec * Time.deltaTime;
                if (targetDistance < VISUAL_STOP_DISTANCE) targetDistance = VISUAL_STOP_DISTANCE;
            }

            if (infoInstance != null && infoInstance.activeSelf && activeInfoScript != null)
            {
                activeInfoScript.UpdateInfo(targetName, catchSpeedKmPerSec * 3600.0, targetDistance);
            }

            float targetVisual = (float)targetDistance * ObjectManager.Instance.distanceScale;
            currentVisualDistance = Mathf.Lerp(currentVisualDistance, targetVisual, Time.deltaTime * 5f);
            if (isDragging)
            {
                targetTransform.position = transform.position - fixedApproachDirection * currentVisualDistance;
            }
            else
            {
                transform.position = targetTransform.position + fixedApproachDirection * currentVisualDistance;
            }

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

            activeInfoScript = infoInstance.GetComponent<CatcherInfo>();
            if (activeInfoScript != null)
            {
                activeInfoScript.Original = this;

                activeInfoScript.UpdateInfo(targetName, catchSpeedKmPerSec * 3600.0, targetDistance);
            }
        }
        else
        {
            bool isActive = !infoInstance.activeSelf;
            infoInstance.SetActive(isActive);
            if (isActive) activeInfoScript = infoInstance.GetComponent<CatcherInfo>();
        }
    }

    void OnDestroy()
    {
        if (infoInstance != null) Destroy(infoInstance);
    }
}