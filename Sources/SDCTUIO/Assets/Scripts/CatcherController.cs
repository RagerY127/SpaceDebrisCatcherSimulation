using One_Sgp4;
using UnityEngine;
using TouchScript.Gestures;

public class CatcherController : MonoBehaviour
{
    private DebrisData _debrisData;

    // getter for anneauController to read
    public DebrisData TargetDebris 
    { 
        get { return _debrisData; } 
    }
    public CatcherData CatcherData { get; private set; }

    [SerializeField]
    private TapGesture TapGesture;

    [SerializeField]
    private LongPressGesture LongPressGesture;

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
