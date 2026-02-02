using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using TMPro;
using System.Globalization;

public class BridgeClient : MonoBehaviour
{
    [Header("Paramètres de connexion")]
    //public string serverIP = "192.168.183.134";
    //public string serverIP = "192.168.183.134";
    //public string serverIP = "127.0.0.1";
    //public string serverIP = "192.168.212.137";
    public string serverIP = "172.20.10.2";
    public int port = 9999;

    [Header("Interface Utilisateur")] 
    public TMP_Text statusText;

    private TcpClient client;
    private NetworkStream stream;
    private Thread clientThread;
    private bool isRunning = false;

    [Header("Paramètres de génération")]
    public GameObject debrisPrefab;
    public GameObject catcherPrefab;
    public GameObject debrisInfo;//prefab qui affiche les données du débris
    public GameObject catcherInfo;//prefab qui affiche les données du catcher

    private List<GameObject> spawnedObjects = new List<GameObject>();
    // File thread-safe pour transférer les messages du thread réseau vers le thread principal
    private ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();

    void Start()
    {
        if (statusText != null) statusText.text = "Waiting for connection...";

        Debug.Log("[HoloLens] Tentative de connexion au PC...");
        isRunning = true;

        clientThread = new Thread(ConnectToServer);
        clientThread.IsBackground = true;
        clientThread.Start();
    }

    // ===============================
    // Thread secondaire : connexion TCP + écoute des messages
    // ===============================
    private void ConnectToServer()
    {
        try
        {
            client = new TcpClient();
            client.Connect(serverIP, port);
            stream = client.GetStream();

            Debug.Log("[HoloLens] Connexion réussie !");
            messageQueue.Enqueue("SYS_CONNECTED");

            byte[] buffer = new byte[1024];

            while (isRunning)
            {
                if (stream.CanRead)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        messageQueue.Enqueue(message);
                    }
                }
            }
        }
        catch (Exception e)
        {
            if (isRunning)
            {
                Debug.LogError($"[HoloLens] Échec de connexion : {e.Message}");
                messageQueue.Enqueue($"SYS_ERROR:{e.Message}");
            }
        }
    }

    // ===============================
    // Thread principal Unity : traitement des messages
    // ===============================
    void Update()
    {
        while (messageQueue.TryDequeue(out string message))
        {
            HandleMessage(message);
        }
    }

    // ===============================
    // Traitement des messages reçus
    // ===============================
    void HandleMessage(string msg)
    {
        Debug.Log($"[MainThread] Message traité : {msg}");

        if (statusText != null)
        {
            // 如果是系统消息，显示状态；如果是数据，显示内容
            if (msg == "SYS_CONNECTED")
            {
                statusText.text = "<color=green>Connected to PC!</color>";
            }
            else if (msg.StartsWith("SYS_ERROR"))
            {
                statusText.text = $"<color=red>{msg}</color>";
            }
            else
            {
                // 显示收到的具体信息
                statusText.text = $"Received: {msg}";
            }
        }

        // génération d'un cube
        if (msg.Trim() == "SPAWN_CUBE")
        {
            SpawnCube();
            return;
        }

        // tentative de traitement des messages JSON
        if (msg.StartsWith("SYS_"))
        {
            TryHandleJsonMessage(msg);
        }
        TryHandleJsonMessage(msg);
        
    }
    // ===============================
    // Nettoyage des objets spawnés 
    // ===============================
    void ClearSpawnedObjects()
    {
        foreach (var obj in spawnedObjects)
        {
            Destroy(obj);
        }
        spawnedObjects.Clear();
    }

    // ===============================
    // Génération du cube 
    // ===============================
    void SpawnCube()
    {
        if (debrisPrefab != null)
        {
            if (Camera.main != null)
            {
                Transform camTransform = Camera.main.transform;
                var obj = Instantiate(
                    debrisPrefab,
                    camTransform.position + camTransform.forward * 1.5f,
                    camTransform.rotation
                );
                spawnedObjects.Add(obj);
            }
            else
            {
                var obj = Instantiate(debrisPrefab, new Vector3(0, 0, 1f), Quaternion.identity);
                spawnedObjects.Add(obj);
            }

            Debug.Log("Cube apparu !");
        }
        else
        {
            Debug.LogError("Prefab non assigné dans l'inspecteur");
        }
    }
    // ===============================
    // Génération du catcher
    // ===============================
    void SpawnCatcher()
    {
        if (catcherPrefab != null)
        {
            if (Camera.main != null)
            {
                Transform camTransform = Camera.main.transform;
                var obj = Instantiate(
                    catcherPrefab,
                    camTransform.position + camTransform.forward * 1.5f,
                    camTransform.rotation
                );
                spawnedObjects.Add(obj);
            }
            else
            {
                var obj = Instantiate(catcherPrefab, new Vector3(0, 0, 1f), Quaternion.identity);
                spawnedObjects.Add(obj);
            }

            Debug.Log("Catcher apparu !");
        }
        else
        {
            Debug.LogError("Prefab non assigné dans l'inspecteur");
        }
    }
    // ===============================
    // Faire Apparaitre les données du catcher
    // ===============================

    void SpawnCatcherData(string targetName, double speed, double targetDistance)
    {
         if (catcherInfo != null)
        {
            if (Camera.main != null)
            {
                Transform camTransform = Camera.main.transform;
                GameObject pane=Instantiate(
                    catcherInfo,
                    camTransform.position - camTransform.forward * 0.2f + camTransform.right*0.5f + camTransform.up*0.2f,
                    camTransform.rotation
                );
                spawnedObjects.Add(pane);
                var script=pane.GetComponent<CatcherInfo>();
                if(script!=null)
                script.UpdateInfo(targetName, speed, targetDistance);
                else
                {
                    Debug.LogError("CatcherInfo script not found on the prefab.");
                }
            }
            else
            {
                GameObject pane=Instantiate(catcherInfo, new Vector3(0, 0, 1f), Quaternion.identity);
                spawnedObjects.Add(pane);
                var script=pane.GetComponent<CatcherInfo>();
                if(script!=null)
                script.UpdateInfo(targetName, speed, targetDistance);
                else
                {
                    Debug.LogError("CatcherInfo script not found on the prefab.");
                }
            }

            Debug.Log("Infos du catcher apparues !");
        }
        else
        {
            Debug.LogError("Prefab non assigné dans l'inspecteur");
        }
    }

    // ===============================
    // Faire Apparaitre les données du débris
    // ===============================

    void SpawnDebrisData(string debrisName, double revolution, double mass, double position)
    {
         if (debrisInfo != null)
        {
            if (Camera.main != null)
            {
                Transform camTransform = Camera.main.transform;
                GameObject pane=Instantiate(
                    debrisInfo,
                    camTransform.position - camTransform.forward * 0.2f + camTransform.right*0.5f + camTransform.up*0.2f,
                    camTransform.rotation
                );
                spawnedObjects.Add(pane);
                var script=pane.GetComponent<DebrisInfo>();
                if(script!=null)
                script.UpdateInfo(debrisName, revolution, mass, position);
                else
                {
                    Debug.LogError("DebrisInfo script not found on the prefab.");
                }
            }
            else
            {
                GameObject pane=Instantiate(debrisInfo, new Vector3(0, 0, 1f), Quaternion.identity);
                spawnedObjects.Add(pane);
                var script=pane.GetComponent<DebrisInfo>();
                if(script!=null)
                script.UpdateInfo(debrisName, revolution, mass, position);
                else
                {
                    Debug.LogError("DebrisInfo script not found on the prefab.");
                }
            }

            Debug.Log("Infos du débris apparues !");
        }
        else
        {
            Debug.LogError("Prefab non assigné dans l'inspecteur");
        }
    }

    // ===============================
    // Traitement des messages JSON (depuis le PC)
    // ===============================
    void TryHandleJsonMessage(string msg)
    {
        try
        {
            HololensMessage message = JsonUtility.FromJson<HololensMessage>(msg);

            Debug.Log("Type du message JSON = " + message.type);
            if(message.type == MessageType.DEBRIS)
            {
                ObjectDataBundle catcher=message.messageData[0].value;
                string name=(catcher.GetValue("name")==null)?"Unknown":catcher.GetValue("name");
                double revolution=double.Parse((catcher.GetValue("revolutionPerDay")==null)?"0":catcher.GetValue("revolutionPerDay"),CultureInfo.InvariantCulture);
                double mass=double.Parse((catcher.GetValue("mass")==null)?"0":catcher.GetValue("mass"),CultureInfo.InvariantCulture);
                double position=double.Parse((catcher.GetValue("position")==null)?"0":catcher.GetValue("position"),CultureInfo.InvariantCulture);
                ClearSpawnedObjects();
                SpawnCube();
                SpawnDebrisData(name, revolution, mass, position);
            }
            else if(message.type == MessageType.CATCHER)
            {
                ObjectDataBundle catcher=message.messageData[0].value;
                string name=(catcher.GetValue("targetName")==null)?"Unknown":catcher.GetValue("targetName");
                double speed=double.Parse((catcher.GetValue("speed")==null)?"0":catcher.GetValue("speed"),CultureInfo.InvariantCulture);
                double distance=double.Parse(catcher.GetValue("targetDistance")==null?"0":catcher.GetValue("targetDistance"),CultureInfo.InvariantCulture);
                
                ClearSpawnedObjects();
                SpawnCatcher();
                SpawnCube();
                SpawnCatcherData(name, speed, distance);
            }
            else{
                Debug.Log("Type de message incorrect");
                return;
            }
            var data = message.messageData;
            if (data != null)
            {
                Debug.Log("Donnée JSON [0] = " + data[0]);
            }
            
        }
        catch
        {
            // Si ce n'est pas du JSON, on le traite comme un message texte classique
            Debug.Log($"Message non JSON : {msg}");
        }
    }

    // ===============================
    // Envoi d'un message vers le PC
    // ===============================
    public void SendToPC(string message)
    {
        if (client == null || !client.Connected || stream == null) return;

        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
            Debug.Log($"[HoloLens] Message envoyé au PC : {message}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Échec de l'envoi vers le PC : {e.Message}");
        }
    }

    // ===============================
    // Nettoyage à la fermeture de l'application
    // ===============================
    void OnApplicationQuit()
    {
        isRunning = false;
        stream?.Close();
        client?.Close();

        if (clientThread != null && clientThread.IsAlive)
        {
            clientThread.Abort();
        }
    }
}
