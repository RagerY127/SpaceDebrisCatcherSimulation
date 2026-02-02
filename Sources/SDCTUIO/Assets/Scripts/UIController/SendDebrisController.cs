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

            
            ObjectDataBundle debris1=new ObjectDataBundle();
            debris1.data.Add(new ObjectDataEntry(){key="name",value="debrisCube"});
            debris1.data.Add(new ObjectDataEntry(){key="revolutionPerDay",value="4.5"});
            debris1.data.Add(new ObjectDataEntry(){key="mass",value="85.9"});
            debris1.data.Add(new ObjectDataEntry(){key="position",value="27.6"});

            message.messageData.Add(new DataEntry(){key="debris1",value=debris1});
            
            /*
            ObjectDataBundle catcher=new ObjectDataBundle();
            catcher.data.Add(new ObjectDataEntry(){key="targetName",value="debris1"});
            catcher.data.Add(new ObjectDataEntry(){key="speed",value="1542.36"});
            catcher.data.Add(new ObjectDataEntry(){key="targetDistance",value="14.67"});

            message.messageData.Add(new DataEntry(){key="catcher",value=catcher});

            ObjectDataBundle debris1=new ObjectDataBundle();
            debris1.data.Add(new ObjectDataEntry(){key="name",value="debrisCube"});
            debris1.data.Add(new ObjectDataEntry(){key="revolutionPerDay",value="4.5"});
            debris1.data.Add(new ObjectDataEntry(){key="mass",value="85.9"});
            debris1.data.Add(new ObjectDataEntry(){key="position",value="27.6"});
            message.messageData.Add(new DataEntry(){key="debris1",value=debris1});*/
            
            string json=JsonUtility.ToJson(message);
            BridgeServer.Instance.SendMessageToHoloLens(json);
        }
        else
        {
            Debug.LogError("Cannot found bridge server");
        }
    }
}