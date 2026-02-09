using System;
using UnityEngine;

[Serializable]
public class HololensMessage
{
    public string command;
    public string targetType;
    public object data;

    private HololensMessage() { }

    public static void SendDebrisMessage(MessageCommand cmd, DebrisData data)
    {
        var msg = new HololensMessage();
        msg.command = cmd.ToString();
        msg.targetType = TargetType.DEBRIS.ToString();
        msg.data = new DebrisDTO(data);

        string json = msg.ToJson();

        BridgeServer.Instance.SendMessageToHoloLens(json);
    }


    public static void SendCatcherMessage(MessageCommand cmd, CatcherData data)
    {
        var msg = new HololensMessage();
        msg.command = cmd.ToString();
        msg.targetType = TargetType.CATCHER.ToString();
        msg.data = new CatcherDTO(data);

        string json = msg.ToJson();
        
        BridgeServer.Instance.SendMessageToHoloLens(json);
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}