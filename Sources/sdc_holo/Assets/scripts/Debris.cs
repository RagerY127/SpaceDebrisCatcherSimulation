using UnityEngine;

public class Debris : MonoBehaviour
{
    [Header ("Info prefab")]
    public GameObject infoPrefab;

    public GameObject infoInstance{ get; set; }

    public string debrisName { get; set; }
    public double revolution { get; set; }
    public double mass { get; set;}
    public double position { get; set; }

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
            var script = infoInstance.GetComponent<DebrisInfo>();
            script.originalDebris = this;
            script.UpdateInfo(debrisName, revolution, mass, position);
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
