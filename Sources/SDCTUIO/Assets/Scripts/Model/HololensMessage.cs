using System;
using UnityEngine;

[Serializable]
public class HololensMessage
{
    public string command;
    public string targetType;
    public DebrisDTO debrisData;
    public CatcherDTO catcherData;

    private HololensMessage() { }

    public static void SendDebrisMessage(MessageCommand cmd, DebrisData data)
    {
        var msg = new HololensMessage();
        msg.command = cmd.ToString();
        msg.targetType = TargetType.DEBRIS.ToString();
        msg.debrisData = new DebrisDTO(data);
        msg.catcherData = null;

        string json = msg.ToJson();

        BridgeServer.Instance.SendMessageToHoloLens(json);
    }


    public static void SendCatcherMessage(MessageCommand cmd, CatcherData data)
    {
        var msg = new HololensMessage();
        msg.command = cmd.ToString();
        msg.targetType = TargetType.CATCHER.ToString();
        msg.catcherData = new CatcherDTO(data);
        msg.debrisData = null;

        string json = msg.ToJson();
        
        BridgeServer.Instance.SendMessageToHoloLens(json);
    }

    public static void SendUpdateMessage(MessageCommand cmd, string id, double xPos, double yPos, double zPos, double xRot, double yRot, double zRot)
    {
        var msg = new HololensMessage();
        msg.command = cmd.ToString();
        msg.targetType = TargetType.DEBRIS.ToString();
        msg.catcherData = null;
        msg.debrisData = null;

        UpdateDTO updateData = new UpdateDTO(id, xPos, yPos, zPos, xRot, yRot, zRot);
        string json = JsonUtility.ToJson(updateData);

        BridgeServer.Instance.SendMessageToHoloLens(json);
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}