using One_Sgp4;
using UnityEngine;
using TouchScript.Gestures;

public class CatcherController : ObjectController<CatcherData>
{    
    // Timer : How many seconds have been running to cath up the debris
    private double _currentCatchProgressSeconds;
    public double CurrentProgressSeconds => _currentCatchProgressSeconds;

    // WHEN TOUCHED!
    [Header("Capture Settings")]
    public float CaptureDistanceThreshold = 1.0f;
    public float PlugInOffset = 0.0f;
    private bool _isCaptured = false;
    private Vector3 _lastDebrisPos = Vector3.zero;
    private Vector3 _lockedTangent = Vector3.forward;
    
    private Vector3 _lastPosition;

    // If already sent to Hololens
    public bool HasBeenSpawned { get; set; } = false;

    public DebrisData TargetDebris 
    { 
        get { return ObjectData?.TargetDebris; } 
    }
    
    // Accept DebrisController, Initial lag time
    public void AssignTargetDebris(DebrisController targetController, double initialLagMinutes = SimulationManager.DEFAULT_CATCHER_LAG_MINUTES)
    {
        this.ObjectData = new CatcherData(
            $"Catcher-{targetController.ObjectData.Name}", 
            targetController.ObjectData, 
            initialLagMinutes
        );
        
        _currentCatchProgressSeconds = 0.0f;
        _isCaptured = false;

        Vector3 initialPosKm = ObjectData.GetPositionAtTime(SimulationManager.SimulationTime, 0.0);
        _lastPosition = initialPosKm * (float)SimulationManager.ScaleFactor;
    }

    void Update()
    {
        if (ObjectData == null || ObjectData.TargetDebris == null) return;

        var debrisRawPos = ObjectData.TargetDebris.GetPositionKmAtTime(SimulationManager.SimulationTime);
        Vector3 debrisTargetPos = new Vector3((float)debrisRawPos.X, (float)debrisRawPos.Y, (float)debrisRawPos.Z) * (float)SimulationManager.ScaleFactor;

        if (_lastDebrisPos != Vector3.zero && debrisTargetPos != _lastDebrisPos)
        {
            _lockedTangent = (debrisTargetPos - _lastDebrisPos).normalized;
        }
        _lastDebrisPos = debrisTargetPos;

        if (!_isCaptured)
        {
            _currentCatchProgressSeconds += Time.deltaTime * SimulationManager.SimulationSpeed;
            Vector3 targetPositionKm = ObjectData.GetPositionAtTime(SimulationManager.SimulationTime, _currentCatchProgressSeconds);
            transform.localPosition = targetPositionKm * (float)SimulationManager.ScaleFactor;

            if (Vector3.Distance(transform.localPosition, debrisTargetPos) <= CaptureDistanceThreshold)
            {
                _isCaptured = true;
            }
        }
        else
        {
            transform.localPosition = debrisTargetPos - _lockedTangent * PlugInOffset;
        }

        Vector3 awayFromEarth = transform.localPosition.normalized;
        
        if (_lockedTangent != Vector3.zero) 
        {
            Quaternion baseRotation = Quaternion.LookRotation(_lockedTangent, awayFromEarth);
            transform.rotation = baseRotation * Quaternion.Euler(180, 0, 0);
        }
    }

    private void OnEnable()
    {
        if (TapGesture != null) TapGesture.Tapped += OnCatcherTapped;
        if (LongPressGesture != null) LongPressGesture.LongPressed += OnCatcherLongPressed;
    }

    private void OnDisable()
    {
        if (TapGesture != null) TapGesture.Tapped -= OnCatcherTapped;
        if (LongPressGesture != null) LongPressGesture.LongPressed -= OnCatcherLongPressed;
    }

    // If clicked (Like debrisManager)
    private void OnCatcherTapped(object sender, System.EventArgs e)
    {
        SimulationManager.Instance.SelectCatcher(this.ObjectData);
        CameraManager.Instance.FollowDebris(this.gameObject);
    }

    // If long clicked (The same)
    private void OnCatcherLongPressed(object sender, System.EventArgs e)
    {
        if (AnneauController.Instance != null)
        {
            AnneauController.Instance.OpenMenuForCatcher(this);
        }
    }
}