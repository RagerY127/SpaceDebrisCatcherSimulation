using UnityEngine;

public class Debris : MonoBehaviour
{
    [Header("Info prefab")]
    public GameObject infoPrefab;

    public GameObject infoInstance;

    public string debrisName { get; set; }
    public double revolution { get; set; }
    public double mass { get; set; }
    public double position { get; set; }

    void Start()
    {

    }

    void Update()
    {

    }

    public void onClick()
    {
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