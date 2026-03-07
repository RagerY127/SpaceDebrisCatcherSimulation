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

    public DebrisDTO(DebrisData source)
    {
        if (source == null) return;
        this.id = source.Id;
        this.name = source.Name;
        this.mass = source.Mass;
        this.shape = source.Shape.ToString();
        this.revolutionsPerDay = source.RevolutionsPerDay;
    }
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

    public CatcherDTO(CatcherData source, double currentProgressSeconds)
    {
        if (source == null || source.TargetDebris == null) return;

        this.Id = source.Id;
        var t = source.TargetDebris;
        this.targetId = t.Id;
        this.targetName = t.Name;
        this.minutesBeforeCatch = source.InitialTimeLagMinutes;

        double periodSeconds = 86400.0 / t.RevolutionsPerDay;
        
        double mu = 398600.4418;
        double a = Math.Pow((mu * periodSeconds * periodSeconds) / (4.0 * Math.PI * Math.PI), 1.0 / 3.0);
        
        this.currentSpeed = (2.0 * Math.PI * a) / periodSeconds;

        double totalSecondNeeded = this.minutesBeforeCatch * 60.0;
        double remainingSeconds = totalSecondNeeded - currentProgressSeconds;
        if (remainingSeconds < 0) remainingSeconds = 0;

        this.distanceToTarget = this.currentSpeed * remainingSeconds;

    }
}
/*
[Serializable]
public class UpdateDTO
{
    public string id;
    public double xPos,yPos,zPos;
    public double xRot,yRot,zRot;
    public UpdateDTO(string id, double xPos, double yPos, double zPos, double xRot, double yRot, double zRot)
    {
        this.id = id;
        this.xPos = xPos;
        this.yPos = yPos;
        this.zPos = zPos;
        this.xRot = xRot;
        this.yRot = yRot;
        this.zRot = zRot;
    }
}*/