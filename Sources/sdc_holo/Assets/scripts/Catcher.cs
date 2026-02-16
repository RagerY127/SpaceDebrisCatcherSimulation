using UnityEngine;

public class Catcher : MonoBehaviour
{
    [Header("Info prefab")]
    public GameObject infoPrefab;

    public string targetName { get; set; }
    public double speed { get; set; }
    public double targetDistance { get; set; }

    public GameObject infoInstance;

    [Header("Debug Status")]
    public bool isDragging = false;

    private float dragStartTime;

    private const float CLICK_THRESHOLD = 0.3f;

    void Start()
    {

    }

    void Update()
    {

    }

    public void OnDragStarted()
    {
        isDragging = true;
        dragStartTime = Time.time; 
    }


    public void OnDragEnded()
    {
        float duration = Time.time - dragStartTime;


        if (duration < CLICK_THRESHOLD)
        {
            isDragging = false;
        }
        else
        {
            Invoke("ResetDragStatus", 0.2f);
        }
    }

    private void ResetDragStatus()
    {
        isDragging = false;
    }

    public void onClick()
    {
        if (isDragging)
        {
            return;
        }

        if (infoInstance == null)
        {
            Transform camTransform = Camera.main.transform;
            Vector3 spawnPosition = this.transform.position - Vector3.forward * 1.2f + Vector3.right * 0.6f + Vector3.down * 0.1f;
            infoInstance = Instantiate(infoPrefab, spawnPosition, camTransform.rotation);

            var script = infoInstance.GetComponent<CatcherInfo>();
            if (script != null)
            {
                script.UpdateInfo(targetName, speed, targetDistance);
            }
        }
        else
        {
            bool isActive = infoInstance.activeSelf;
            infoInstance.SetActive(!isActive);
        }
    }

    void OnDestroy()
    {
        if (infoInstance != null)
        {
            Destroy(infoInstance);
            infoInstance = null;
        }
    }
}