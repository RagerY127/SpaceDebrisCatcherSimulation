using UnityEngine;

public class Debris : MonoBehaviour
{
    [Header("Info prefab")]
    public GameObject infoPrefab;

    public GameObject infoInstance { get; set; }

    public string debrisName { get; set; }
    public double revolution { get; set; }
    public double mass { get; set; }
    public double position { get; set; }


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

        if (isDragging) return;

        if (infoInstance == null)
        {
            Transform camTransform = Camera.main.transform;

            Vector3 spawnPosition = this.transform.position - Vector3.forward * 1.3f + Vector3.right * 0.6f;

            infoInstance = Instantiate(infoPrefab, spawnPosition, camTransform.rotation);

            var script = infoInstance.GetComponent<DebrisInfo>();
            if (script != null)
            {
                script.UpdateInfo(debrisName, revolution, mass, position);
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