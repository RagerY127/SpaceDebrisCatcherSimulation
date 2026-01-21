using System.Collections.Generic;
using UnityEngine;
using One_Sgp4;

public class DebrisManager : MonoBehaviour
{
    public static DebrisManager Instance { get; private set; }

    public GameObject DebrisPrefab;
    public GameObject SelectedDebris;
    [SerializeField]
    private int OrbitPointCount;
    [SerializeField]
    private LineRenderer LineRenderer;

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

        // Change prefab data from the debris data
        switch (debrisData.Shape)
        {
            case DebrisShape.Cube:
                debrisObject.GetComponent<MeshFilter>().mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
                break;
            case DebrisShape.Cylinder:
                debrisObject.GetComponent<MeshFilter>().mesh = Resources.GetBuiltinResource<Mesh>("Cylinder.fbx");
                break;
        }
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

    public void SelectDebris(string debrisId)
    {
        if (!DebrisObjects.ContainsKey(debrisId))
        {
            return;
        }

        SelectedDebris = DebrisObjects[debrisId];
        DrawDebrisOrbit(debrisId);
    }

    public void DrawDebrisOrbit(string debrisId)
    {
        if (!DebrisObjects.ContainsKey(debrisId))
        {
            return;
        }

        DebrisController debrisController = DebrisObjects[debrisId].GetComponent<DebrisController>();

        LineRenderer.positionCount = OrbitPointCount;
        LineRenderer.loop = true;

        Vector3[] orbitPoints = new Vector3[OrbitPointCount];

        float meanMotion = debrisController.DebrisData.RevolutionsPerDay;
        float periodMinutes = 60f * 24f / meanMotion;

        EpochTime startTime = new EpochTime(SimulationManager.SimulationTime);
        for (int i = 0; i < OrbitPointCount; i++)
        {
            double timeOffsetMinutes = periodMinutes / OrbitPointCount * i;

            EpochTime time = new EpochTime(startTime);
            time.addMinutes(timeOffsetMinutes);

            orbitPoints[i] = debrisController.DebrisData.GetPositionKmAtTime(time).ToUnityVector3() * SimulationManager.ScaleFactor;
        }

        LineRenderer.SetPositions(orbitPoints);
    }

    private void Update()
    {
        
    }
}
