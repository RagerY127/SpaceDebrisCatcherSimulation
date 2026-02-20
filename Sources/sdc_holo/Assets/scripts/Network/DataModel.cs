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
    public float mass;
    public string shape;
    public float revolutionsPerDay;
}

[Serializable]
public class CatcherData
{
    public string Id;
    public string targetId;
    public string targetName;
    public double currentSpeed;
    public double distanceToTarget;
    public double minutesBeforeCatch;
}