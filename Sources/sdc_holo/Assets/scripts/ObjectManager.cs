// scripts/ObjectManager.cs
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct DebrisPrefabMap
{
    public string shapeName; 
    public GameObject prefab;
}

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Instance;

    [Header("Prefabs of objects")]
    public List<DebrisPrefabMap> debrisPrefabs;
    public GameObject catcherPrefab;

    [Header("Scaling settings")]
    public float distanceScale = 0.05f; 
    public float modelScale = 0.1f;
    // patch ：the ancient logic of caculate the distance in TABLE was wrong but we don't want to have a big change
    public float realDataShrinkFactor = 0.01f;

    private Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void ProcessJsonMessage(string json)
    {
        try
        {
            NetMessage msg = JsonUtility.FromJson<NetMessage>(json);
            if (msg == null || string.IsNullOrEmpty(msg.command)) return;

            switch (msg.command)
            {
                case "SPAWN":
                    if (msg.debrisData != null && !string.IsNullOrEmpty(msg.debrisData.id))
                        SpawnDebris(msg.debrisData);
                    if (msg.catcherData != null && !string.IsNullOrEmpty(msg.catcherData.Id))
                        SpawnCatcher(msg.catcherData);
                    break;
                case "UPDATE":
                    if (msg.catcherData != null && !string.IsNullOrEmpty(msg.catcherData.Id))
                        UpdateCatcher(msg.catcherData);
                    break;
                case "DELETE":
                    ClearSpawnedObjects();
                    break;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Analyse JSON fail: {e.Message}");
        }
    }

    private void SpawnDebris(DebrisData data)
    {
        if (spawnedObjects.ContainsKey(data.id)) return;

        GameObject selectedPrefab = null;
        foreach (var map in debrisPrefabs)
        {
   
            if (map.shapeName.Equals(data.shape, System.StringComparison.OrdinalIgnoreCase))
            {
                selectedPrefab = map.prefab;
                break;
            }
        }

        if (selectedPrefab == null)
        {
            Debug.LogWarning($"Shape '{data.shape}' not found, using default.");
            if (debrisPrefabs.Count > 0) selectedPrefab = debrisPrefabs[0].prefab;
            else return;
        }

        Vector3 spawnPos = Camera.main != null
            ? Camera.main.transform.position + Camera.main.transform.forward * 0.8f
            : new Vector3(0, 0, 0.8f);

        GameObject debris = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
        debris.transform.localScale = Vector3.one * modelScale;

        var script = debris.GetComponent<Debris>();
        if (script != null)
        {
            script.debrisName = data.name;
            script.mass = data.mass;
            script.revolution = data.revolutionsPerDay;
        }

        spawnedObjects.Add(data.id, debris);
    }

    private void SpawnCatcher(CatcherData data)
    {
        if (spawnedObjects.ContainsKey(data.Id)) return;
        if (!spawnedObjects.TryGetValue(data.targetId, out GameObject targetDebris)) return;

        GameObject catcher = Instantiate(catcherPrefab, targetDebris.transform.position, Quaternion.identity);
        catcher.transform.localScale = Vector3.one * modelScale;

        Catcher chaseScript = catcher.GetComponent<Catcher>();
        if (chaseScript != null)
        {
            chaseScript.targetName = data.targetName;
            double visualDistance = data.distanceToTarget * realDataShrinkFactor;
            chaseScript.SetTarget(targetDebris.transform, visualDistance, data.minutesBeforeCatch);
        }

        spawnedObjects.Add(data.Id, catcher);
    }

    private void UpdateCatcher(CatcherData data)
    {
        if (spawnedObjects.TryGetValue(data.Id, out GameObject catcherObj))
        {
            Catcher script = catcherObj.GetComponent<Catcher>();
            if (script != null)
            {
                double visualDistance = data.distanceToTarget * realDataShrinkFactor;
                script.UpdateTargetDistance(visualDistance);
            }
        }
    }

    public void ClearSpawnedObjects()
    {
        foreach (var obj in spawnedObjects.Values) Destroy(obj);
        spawnedObjects.Clear();
    }
}