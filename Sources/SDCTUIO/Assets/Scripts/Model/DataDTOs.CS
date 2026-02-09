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
    public string id;
    public string targetId;
    public string targetName;
    public double currentSpeed;
    public double distanceToTarget;
    public double minutesBeforeCatch;

    public CatcherDTO(CatcherData source)
    {
        if (source == null) return;
        
        this.currentSpeed = source.GetSpeed();
        this.distanceToTarget = source.GetTargetDistance();
        this.minutesBeforeCatch = source.MinutesBeforeCatch;

        if (source.Target != null)
        {
            this.targetId = source.Target.Id;
            this.targetName = source.Target.Name;
        }
    }
}