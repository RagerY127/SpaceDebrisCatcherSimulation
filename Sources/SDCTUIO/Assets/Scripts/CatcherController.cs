using One_Sgp4;
using UnityEngine;

public class CatcherController : MonoBehaviour
{
    private DebrisData _debrisData;
    private double _timeOffsetSeconds;

    public void AssignTargetDebris(GameObject debris)
    {
        _debrisData = debris.GetComponent<DebrisController>().DebrisData;
        _timeOffsetSeconds = 0.0f;
    }

    void Update()
    {
        EpochTime newTime = new EpochTime(SimulationManager.SimulationTime);
        newTime.addMinutes(-5);
        newTime.addMinutes(_timeOffsetSeconds / 60.0f);
        _timeOffsetSeconds += Time.deltaTime * SimulationManager.SimulationSpeed;

        Vector3 previousPosition = transform.localPosition;
        transform.localPosition = _debrisData.GetPositionKmAtTime(newTime).ToUnityVector3() * SimulationManager.ScaleFactor;
        transform.LookAt(transform.localPosition - (transform.localPosition - previousPosition));
    }
}
