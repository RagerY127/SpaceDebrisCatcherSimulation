// scripts/ObjectManager.cs
using UnityEngine;
using System.Collections.Generic;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Instance;

    [Header("Prefabs of objects")]
    public GameObject debrisPrefab;
    public GameObject catcherPrefab;

    [Header("Scaling settings")]
    public float distanceScale = 0.05f; 
    public float modelScale = 0.1f; 

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

        Vector3 spawnPos = Camera.main != null 
            ? Camera.main.transform.position + Camera.main.transform.forward * 0.8f 
            : new Vector3(0, 0, 0.8f);

        GameObject debris = Instantiate(debrisPrefab, spawnPos, Quaternion.identity);
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
            chaseScript.SetTarget(targetDebris.transform, data.distanceToTarget, data.minutesBeforeCatch);
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
                script.UpdateTargetDistance(data.distanceToTarget);
            }
        }
    }

    public void ClearSpawnedObjects()
    {
        foreach (var obj in spawnedObjects.Values) Destroy(obj);
        spawnedObjects.Clear();
    }
}