using UnityEngine;

public class Catcher : MonoBehaviour
{
    [Header ("Info prefab")]
    public GameObject infoPrefab;

    public string targetName { get; set; }
    public double speed { get; set; }
    public double targetDistance { get; set; }
    public GameObject infoInstance{get; set;}
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     public void onClick()
    {
       if(infoInstance == null)
        {
            Transform camTransform = Camera.main.transform;
            infoInstance = Instantiate(infoPrefab, camTransform.position - camTransform.forward * 0.2f + camTransform.right*0.5f + camTransform.up*0.2f,
             camTransform.rotation);
            var script = infoInstance.GetComponent<CatcherInfo>();
            script.Original = this;
            script.UpdateInfo(targetName, speed, targetDistance);
        }
       else
        {
            Destroy(infoInstance);
            infoInstance = null;
        }
    }
    void OnDestroy()
    {
        if(infoInstance != null)
        {
            Destroy(infoInstance);
            infoInstance = null;
        }
    }
}
