using UnityEngine;
using UnityEngine.UIElements;

public class SendDebrisController : MonoBehaviour
{
    [SerializeField] private string messageToSend = "SPAWN_CUBE"; 

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        
        var createButton = root.Q<Button>("sendDebrisBoutton");

        if (createButton != null)
        {
            createButton.clicked += OnSendDebris;
        }
        else
        {
            Debug.LogError("Cannot find botton, exam the name of UI Builder");
        }
    }

    void OnSendDebris()
    {
        if (BridgeServer.Instance != null)
        {
            Debug.Log("The button is clicked, demande the server...");

            //HololensMessage.SendDebrisMessage(MessageCommand.SPAWN, new DebrisData("nom", 7000, 7000, 0, 15, 100, DebrisShape.Cube, 1, 1, 1));
            HololensMessage.SendCatcherMessage(MessageCommand.SPAWN, new CatcherData(12, new DebrisData("nom", 7000, 7000, 0, 15, 100, DebrisShape.Cube, 1, 1, 1)));
        }
        else
        {
            Debug.LogError("Cannot found bridge server");
        }
    }
}