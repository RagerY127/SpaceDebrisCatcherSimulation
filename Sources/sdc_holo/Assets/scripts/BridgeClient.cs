using System.Collections.Concurrent; // 引入线程安全队列
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class BridgeClient : MonoBehaviour
{
    public string serverIP = "192.168.183.134";
    //public string serverIP = "192.168.183.134";
    //public string serverIP = "127.0.0.1";
    //public string serverIP = "192.168.212.137";
    public int port = 9999;

    private TcpClient client;
    private Thread clientThread;

    public GameObject myPrefab;

    private ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();

    void Start()
    {
        Debug.Log("[HoloLens] is trying to connect PC...");
        clientThread = new Thread(ConnectToServer);
        clientThread.IsBackground = true;
        clientThread.Start();
    }

    private void ConnectToServer()
    {
        try
        {
            client = new TcpClient();
            client.Connect(serverIP, port);
            Debug.Log("[HoloLens] Connect Success!");

            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            while (true)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    messageQueue.Enqueue(message);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[HoloLens] Connect Fail: {e.Message}");
        }
    }

    void Update()
    {
        while (messageQueue.TryDequeue(out string message))
        {
            HandleMessage(message);
        }
    }

    void HandleMessage(string msg)
    {
        Debug.Log($"[MainThread] traite message: {msg}");

        if (msg.Trim() == "SPAWN_CUBE")
        {
            if (myPrefab != null)
            {
                if (Camera.main != null)
                {
                    Transform camTransform = Camera.main.transform;
                    Instantiate(myPrefab, camTransform.position + camTransform.forward * 1.5f, Quaternion.identity);
                }
                else
                {
                    Instantiate(myPrefab, new Vector3(0, 0, 1f), Quaternion.identity);
                }

                Debug.Log("appear Cube!");
            }
            else
            {
                Debug.LogError(" inspector exam");
            }
        }
        else
        {
            Debug.Log($"other message  {msg} ");
        }
    }

    void OnApplicationQuit()
    {
        if (client != null) client.Close();
        if (clientThread != null) clientThread.Abort();
    }
}