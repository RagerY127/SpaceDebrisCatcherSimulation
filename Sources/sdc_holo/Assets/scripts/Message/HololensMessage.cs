using System;
using UnityEngine;
using System.Runtime.Serialization;

[Serializable]
public class HololensMessage
{
    public string command;
    public string targetType;
    public DebrisDTO debrisData;
    public CatcherDTO catcherData;

    private HololensMessage() { }

    public static string GetMessageCommand(string json)
    {
        var msg = JsonUtility.FromJson<HololensMessage>(json);
        if (msg != null)
        {
            return msg.command;
        }
        else return null;
    }
    public static string GetMessageTargetType(string json)
    {
        var msg = JsonUtility.FromJson<HololensMessage>(json);
        if (msg != null)
        {
            return msg.targetType;
        }
        return null;
    }
    public static DebrisDTO ReadDebrisMessage(string json)
    {
        var msg = JsonUtility.FromJson<HololensMessage>(json);
        if (msg != null && msg.targetType == TargetType.DEBRIS.ToString())
        {
            return msg.debrisData;
        }
        return null;
    }

    public static CatcherDTO ReadCatcherMessage(string json)
    {
        var msg = JsonUtility.FromJson<HololensMessage>(json);
        if (msg != null && msg.targetType == TargetType.CATCHER.ToString())
        {
            return msg.catcherData;
        }
        return null;
    }
    public static UpdateDTO ReadUpdateMessage(string json)
    {
        var msg = JsonUtility.FromJson<HololensMessage>(json);
        if (msg != null && msg.command == "UPDATE")
        {
            return JsonUtility.FromJson<UpdateDTO>(json);
        }
        return null;
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}
