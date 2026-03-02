// scripts/BridgeClient.cs
using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using TMPro;

public class BridgeClient : MonoBehaviour
{
    [Header("Paramètres de connexion")]
    public string serverIP = "172.20.10.2";
    public int port = 9999;

    [Header("Interface Utilisateur")] 
    public TMP_Text statusText;

    private TcpClient client;
    private NetworkStream stream;
    private Thread clientThread;
    private bool isRunning = false;

    private ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();

    void Start()
    {
        if (statusText != null) statusText.text = "Waiting for connection...";
        isRunning = true;
        clientThread = new Thread(ConnectToServer) { IsBackground = true };
        clientThread.Start();
    }

    private void ConnectToServer()
    {
        try
        {
            client = new TcpClient();
            client.Connect(serverIP, port);
            stream = client.GetStream();
            messageQueue.Enqueue("SYS_CONNECTED");

            byte[] buffer = new byte[4096];
            while (isRunning && stream != null)
            {
                if (stream.DataAvailable)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        messageQueue.Enqueue(message);
                    }
                }
                Thread.Sleep(10);
            }
        }
        catch (Exception e)
        {
            if (isRunning) messageQueue.Enqueue($"SYS_ERROR:{e.Message}");
        }
    }

    void Update()
    {
        while (messageQueue.TryDequeue(out string message))
        {
            if (message == "SYS_CONNECTED" && statusText != null)
                statusText.text = "<color=green>Connected to PC!</color>";
            else if (message.StartsWith("SYS_ERROR") && statusText != null)
                statusText.text = $"<color=red>{message}</color>";
            else
            {
                ObjectManager.Instance.ProcessJsonMessage(message);
            }
        }
    }

    void OnApplicationQuit()
    {
        isRunning = false;
        stream?.Close();
        client?.Close();
        if (clientThread != null && clientThread.IsAlive) clientThread.Abort();
    }
}