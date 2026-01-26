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
            HololensMessage message=new HololensMessage(MessageType.DEBRIS);
            string json=JsonUtility.ToJson(message);
            BridgeServer.Instance.SendMessageToHoloLens(json);
        }
        else
        {
            Debug.LogError("Cannot found bridge server");
        }
    }
}