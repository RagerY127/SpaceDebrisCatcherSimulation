using UnityEngine;

public class DebrisManager : MonoBehaviour
{
    public static DebrisManager Instance { get; private set; }

    public GameObject DebrisPrefab;

    void Awake() 
    {
        Instance = this;
    }

    public void AddDebrisToSimulation(DebrisData debrisData)
    {
        GameObject debrisObject = Instantiate(this.DebrisPrefab, this.transform);
        DebrisController debrisController = debrisObject.GetComponent<DebrisController>();
        debrisController.AssignDebrisData(debrisData);
    }

    private void Update()
    {
        
    }
}
