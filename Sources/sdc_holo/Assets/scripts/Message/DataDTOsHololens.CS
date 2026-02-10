using System;
using UnityEngine;

[Serializable]
public class DebrisDTO
{
    public string id;
    public string name;
    public float mass;
    public string shape;
    public float revolutionsPerDay;
}

[Serializable]
public class CatcherDTO
{
    public string Id;
    public string targetId;
    public string targetName;
    public double currentSpeed;
    public double distanceToTarget;
    public double minutesBeforeCatch;
}

[Serializable]
public class UpdateDTO
{
    public string id;
    public double xPos,yPos,zPos;
    public double xRot,yRot,zRot;
}