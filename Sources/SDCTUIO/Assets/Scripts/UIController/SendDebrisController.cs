using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class SendDebrisController : MonoBehaviour
{
    [SerializeField] private string messageToSend = "SPAWN_CUBE"; 

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        
        // var createButton = root.Q<Button>("sendDebrisBoutton");

        // if (createButton != null)
        // {
        //     createButton.clicked += OnSendDebris;
        // }
        // else
        // {
        //     Debug.LogError("Cannot find botton, exam the name of UI Builder");
        // }
    }

    void OnSendDebris()
    {
        if (BridgeServer.Instance != null)
        {
            Debug.Log("The button is clicked, demande the server...");
            var debrisData = new DebrisData("nom", 7000, 7000, 0, 15, 100, DebrisShape.Cube, 1, 1, 1);
            string id= debrisData.Id;
            HololensMessage.SendDebrisMessage(MessageCommand.SPAWN, debrisData);
            HololensMessage.SendUpdateMessage(MessageCommand.UPDATE, id, 0, 0, 0, 0, 0, 0);
            //HololensMessage.SendCatcherMessage(MessageCommand.SPAWN, new CatcherData("Catcher1", new DebrisData("Debris1", 7000, 7000, 0, 15, 100, DebrisShape.Cube, 1, 1, 1)));
        }
        else
        {
            Debug.LogError("Cannot found bridge server");
        }
    }
}