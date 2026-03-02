using System;

[Serializable]
public class NetMessage
{
    public string command;
    public string targetType;
    public DebrisData debrisData;
    public CatcherData catcherData;
}

[Serializable]
public class DebrisData
{
    public string id;
    public string name;
    public double mass;
    public string shape;
    public double revolutionsPerDay;
}

[Serializable]
public class CatcherData
{
    public string Id;
    public string targetId;
    public string targetName;
    public double currentSpeed;       // 修复：必须是 double
    public double distanceToTarget;   // 修复：必须是 double
    public double minutesBeforeCatch;
}