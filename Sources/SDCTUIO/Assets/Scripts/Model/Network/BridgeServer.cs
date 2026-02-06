using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class BridgeServer : MonoBehaviour
{
    public static BridgeServer Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private int port = 9999;
    private TcpListener listener;
    private TcpClient connectedClient;
    private Thread serverThread;

    void Start()
    {
        // new fils thread to listen for connection
        serverThread = new Thread(ListenForConnection);
        serverThread.IsBackground = true;
        serverThread.Start();
    }

    // LISTEN
    private void ListenForConnection()
    {
        try
        {
            // listen on any IP address at the specified port
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Debug.Log($"PC SERVER SUCCESS LISTEN ON : {port}...");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                connectedClient = client;
                Debug.Log("Pipe connect");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"error: {e.Message}");
        }
    }

    // function to send message to HoloLens
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendMessageToHoloLens("Hello HoloLens, this is PC!");
        }
    }

    /*public void OnClickSpawnButton()
    {
        SendMessageToHoloLens("SPAWN_CUBE");
    }*/

    public void SendMessageToHoloLens(string message)
    {
        if (connectedClient == null)
        {
            Debug.LogError("FAIL TO SEND");
            return;
        }
        try
        {
            NetworkStream stream = connectedClient.GetStream();
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
            Debug.Log($"PC already sent: {message}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"sent fail: {e.Message}");
        }
    }

    // Quit application
    void OnApplicationQuit()
    {
        if (listener != null) listener.Stop();
        if (serverThread != null) serverThread.Abort();
    }
}