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

    public CatcherDTO(CatcherData source)
    {
        if (source == null) return;

        this.Id = source.Id;
        
        this.minutesBeforeCatch = source.InitialTimeLagMinutes;

        if (source.TargetDebris == null) return;

        var t = source.TargetDebris;
        this.targetId = t.Id;
        this.targetName = t.Name;
        this.currentSpeed = (2 * Math.PI * t.OrbitFirstAxis * t.RevolutionsPerDay) / 86400.0;
        this.distanceToTarget = this.currentSpeed * (this.minutesBeforeCatch * 60.0);
    }
}