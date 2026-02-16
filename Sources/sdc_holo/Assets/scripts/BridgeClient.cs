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

                statusText.text = $"Received: {msg}";
            }
        }

        // génération d'un cube
        if (msg.Trim() == "SPAWN_CUBE")
        {
            ObjectManager.SpawnDebris("cube", "cube", 0, 1, 0);
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
    // Traitement des messages JSON (depuis le PC)
    // ===============================
    void TryHandleJsonMessage(string msg)
    {
        try
        {
            string command=HololensMessage.GetMessageCommand(msg);
            Debug.Log("Command du message JSON = " + command);
            switch (command)
            {
                case "DELETE":
                    ObjectManager.ClearSpawnedObjects();
                    return;
                case "SPAWN":
                    string type=HololensMessage.GetMessageTargetType(msg);
                        Debug.Log("Type du message JSON = " + type);
                        
                        if(type == "DEBRIS")
                        {
                            DebrisDTO debrisDTO=HololensMessage.ReadDebrisMessage(msg);
                            string id=(debrisDTO.id==null)?"":debrisDTO.id;
                            string name=(debrisDTO.name==null)?"Unknown":debrisDTO.name;
                            double revolution=debrisDTO.revolutionsPerDay;
                            double mass=debrisDTO.mass;
                            double position=0;
                            ObjectManager.SpawnDebris(id,name, revolution, mass, position);
                        }
                        else if(type == "CATCHER")
                        {

                            CatcherDTO catcherDTO=HololensMessage.ReadCatcherMessage(msg);
                            string id=(catcherDTO.Id==null)?"":catcherDTO.Id;
                            string name=(catcherDTO.targetName==null)?"Unknown":catcherDTO.targetName;
                            double speed=catcherDTO.currentSpeed;
                            double distance=catcherDTO.distanceToTarget;
                            ObjectManager.SpawnCatcher(id, name, speed, distance);

                        }
                        else{
                            Debug.Log("Type de message incorrect");
                            return;
                        }
                    return;
                case "UPDATE":
                    Debug.Log("Message de mise à jour reçu");
                    UpdateDTO updateDTO=HololensMessage.ReadUpdateMessage(msg);
                    if(updateDTO!=null)
                    {
                        string id=updateDTO.id;
                        double xPos=updateDTO.xPos;
                        double yPos=updateDTO.yPos;
                        double zPos=updateDTO.zPos;
                        double xRot=updateDTO.xRot;
                        double yRot=updateDTO.yRot;
                        double zRot=updateDTO.zRot;
                        ObjectManager.UpdateObject(id, new Vector3((float)xPos, (float)yPos, (float)zPos),
                         new Vector3((float)xRot, (float)yRot, (float)zRot));
                    }
                    return;
                default:
                    Debug.Log("Command de message JSON inconnue");
                    break;
            }
            
            var data = msg;
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
