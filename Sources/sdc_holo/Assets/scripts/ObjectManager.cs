using UnityEngine;
using System.Collections.Generic;

public class ObjectManager : MonoBehaviour
{
    [Header("Préfabs d'objets")]
    public GameObject debrisPrefab;
    public GameObject catcherPrefab;

    private static GameObject debrisPrefabInstance;
    private static GameObject catcherPrefabInstance;

    private static Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        debrisPrefabInstance = debrisPrefab;
        catcherPrefabInstance = catcherPrefab;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // ===============================
    // Nettoyage des objets spawnés 
    // ===============================
    public static void ClearSpawnedObjects()
    {
        foreach (var obj in spawnedObjects.Values)
        {
            Destroy(obj);
        }
        spawnedObjects.Clear();
    }
    // ===============================
    // Génération du débris 
    // ===============================
    public static GameObject SpawnDebris(string id="",string debrisName="debris", double revolution=0, double mass=1, double position=0)
    {
        if(spawnedObjects.ContainsKey(id))
        {
            Debug.LogWarning($"Un objet avec l'ID {id} existe déjà. Suppression de l'ancien objet avant de créer le nouveau.");
            Destroy(spawnedObjects[id]);
            spawnedObjects.Remove(id);
        }
        if (debrisPrefabInstance != null)
        {
            if (Camera.main != null)
            {
                Transform camTransform = Camera.main.transform;
                var obj = Instantiate(
                    debrisPrefabInstance,
                    camTransform.position + camTransform.forward * 1.5f,
                    camTransform.rotation
                );
                var script=obj.GetComponent<Debris>();
                if(script!=null)
                {
                    script.debrisName=debrisName;
                    script.revolution=revolution;
                    script.mass=mass;
                    script.position=position;
                }
                spawnedObjects.Add(id,obj);
                Debug.Log("Débris apparu !");
                return obj;
            }
            else
            {
                var obj = Instantiate(debrisPrefabInstance, new Vector3(0, 0, 1f), Quaternion.identity);
                spawnedObjects.Add(id,obj);
                Debug.Log("Débris apparu !");
                return obj;
            }
            
        }
        else
        {
            Debug.LogError("Prefab non assigné dans l'inspecteur");
            return null;
        }
    }
    // ===============================
    // Génération du catcher
    // ===============================
    public static GameObject SpawnCatcher(string id="", string targetName="", double speed=0, double targetDistance=0)
    {
        if(spawnedObjects.ContainsKey(id))
        {
            Debug.LogWarning($"Un objet avec l'ID {id} existe déjà. Suppression de l'ancien objet avant de créer le nouveau.");
            Destroy(spawnedObjects[id]);
            spawnedObjects.Remove(id);
        }
        if (catcherPrefabInstance != null)
        {
            if (Camera.main != null)
            {
                Transform camTransform = Camera.main.transform;
                var obj = Instantiate(
                    catcherPrefabInstance,
                    camTransform.position + camTransform.forward * 1.5f,
                    camTransform.rotation
                );
                var script=obj.GetComponent<Catcher>();
                if(script!=null)
                {
                    script.targetName=targetName;
                    script.speed=speed;
                    script.targetDistance=targetDistance;
                }
                spawnedObjects.Add(id,obj);
                Debug.Log("Catcher apparu !");
                return obj;
            }
            else
            {
                var obj = Instantiate(catcherPrefabInstance, new Vector3(0, 0, 1f), Quaternion.identity);
                spawnedObjects.Add(id,obj);
                Debug.Log("Catcher apparu !");
                return obj;
            }

            
        }
        else
        {
            Debug.LogError("Prefab non assigné dans l'inspecteur");
            return null;
        }
    }
    
    // ===============================
    // Mise à jour d'un objet
    // ===============================  
    public static void UpdateObject(string id, Vector3 newPosition, Vector3 newRotation)
    {
        if (spawnedObjects.ContainsKey(id))
        {
            GameObject obj = spawnedObjects[id];
            obj.transform.position = newPosition;
            obj.transform.rotation = Quaternion.Euler(newRotation);
            Debug.Log($"Objet {id} mis à jour !");
        }
        else
        {
            Debug.LogWarning($"Objet avec l'ID {id} non trouvé pour la mise à jour.");
        }
    }

    public void HandleNetworkMessage(string json)
    {
        NetMessage msg = JsonUtility.FromJson<NetMessage>(json);
        if (msg == null) return;

        if (msg.command == "SPAWN")
        {
            if (msg.targetType == "DEBRIS") SpawnDebris(msg.debrisData);
            else if (msg.targetType == "CATCHER") SpawnCatcher(msg.catcherData);
        }
        else if (msg.command == "UPDATE")
        {
            if (msg.targetType == "CATCHER") UpdateCatcher(msg.catcherData);
        }
        else if (msg.command == "DELETE")
        {
            ClearSpawnedObjects();
        }
    }

    private void SpawnDebris(DebrisData data)
    {
        
    }

    private void SpawnCatcher(CatcherData data)
    {
        
    }
    private void UpdateCatcher(CatcherData data)
    {
        
    }

}
