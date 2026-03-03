using One_Sgp4;
using UnityEngine;

public class CatcherData : ObjectData
{
    public DebrisData TargetDebris { get; private set; }
    
    public double InitialTimeLagMinutes { get; private set; }

    public CatcherData(string name, DebrisData target, double initialTimeLagMinutes = 5.0)
    {
        this.Id = $"{target.Id}-Catcher";
        this.Name = name;
        this.TargetDebris = target;
        this.InitialTimeLagMinutes = initialTimeLagMinutes;
    }

    public Vector3 GetPositionAtTime(EpochTime exactCatcherTime)
    {
        if (TargetDebris == null) return Vector3.zero;

        var rawPos = TargetDebris.GetPositionKmAtTime(exactCatcherTime);

        return new Vector3((float)rawPos.X, (float)rawPos.Y, (float)rawPos.Z);
    }
}