using System.Collections.Generic;
using UnityEngine;

public class DebrisManager : MonoBehaviour
{
    public static DebrisManager Instance { get; private set; }

    public GameObject DebrisPrefab;

    private Dictionary<string, GameObject> DebrisObjects = new();

    void Awake() 
    {
        Instance = this;
    }

    public void AddDebrisToSimulation(DebrisData debrisData)
    {
        GameObject debrisObject = Instantiate(this.DebrisPrefab, this.transform);
        DebrisController debrisController = debrisObject.GetComponent<DebrisController>();
        debrisController.AssignDebrisData(debrisData);

        DebrisObjects[debrisData.Id] = debrisObject;
    }

    public void RemoveDebris(string debrisId)
    {
        if (DebrisObjects.ContainsKey(debrisId))
        {
            GameObject debrisObject = DebrisObjects[debrisId];
            DebrisObjects.Remove(debrisId);
            Destroy(debrisObject);
        }
    }

    private void Update()
    {
        
    }
}
