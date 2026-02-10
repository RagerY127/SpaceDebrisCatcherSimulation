using One_Sgp4;
using System;
using UnityEngine;

public class CatcherData
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public DebrisData TargetDebris { get; private set; }
    
    public double InitialTimeLagMinutes { get; private set; }

    public CatcherData(string name, DebrisData target, double initialTimeLagMinutes = 5.0)
    {
        this.Id = $"{target.Id}-Catcher";
        this.Name = name;
        this.TargetDebris = target;
        this.InitialTimeLagMinutes = initialTimeLagMinutes;
    }

    public Vector3 GetPositionAtTime(EpochTime simulationTime, double catchProgressSeconds)
    {
        if (TargetDebris == null) return Vector3.zero;

        EpochTime calcTime = new EpochTime(simulationTime);

        calcTime.addMinutes(-this.InitialTimeLagMinutes);
        calcTime.addMinutes(catchProgressSeconds / 60.0f);

        var rawPos = TargetDebris.GetPositionKmAtTime(calcTime);

        return new Vector3((float)rawPos.X, (float)rawPos.Y, (float)rawPos.Z);
    }
}