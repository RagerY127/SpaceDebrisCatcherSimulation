using One_Sgp4;
using UnityEngine;
using TouchScript.Gestures;

public class CatcherController : ObjectController<CatcherData>
{    
    private double _currentCatchProgressSeconds;
    public double CurrentProgressSeconds => _currentCatchProgressSeconds;

    public bool HasBeenSpawned { get; set; } = false;

    [Header("Capture Settings")]
    public float PlugInOffset = 0.0f;

    public DebrisData TargetDebris 
    { 
        get { return ObjectData?.TargetDebris; } 
    }
    
    public void AssignTargetDebris(DebrisController targetController, double initialLagMinutes = SimulationManager.DEFAULT_CATCHER_LAG_MINUTES)
    {
        this.ObjectData = new CatcherData(
            $"Catcher-{targetController.ObjectData.Name}", 
            targetController.ObjectData, 
            initialLagMinutes
        );
        
        _currentCatchProgressSeconds = 0.0f;
    }

    void Update()
    {
        if (ObjectData == null || TargetDebris == null) return;

        double maxProgressSeconds = ObjectData.InitialTimeLagMinutes * 60.0;

        _currentCatchProgressSeconds += Time.deltaTime * SimulationManager.SimulationSpeed;
        
        if (_currentCatchProgressSeconds > maxProgressSeconds)
        {
            _currentCatchProgressSeconds = maxProgressSeconds;
        }

        EpochTime currentCatcherTime = new EpochTime(SimulationManager.SimulationTime);
        currentCatcherTime.addMinutes(-ObjectData.InitialTimeLagMinutes);
        currentCatcherTime.addMinutes(_currentCatchProgressSeconds / 60.0);

        Vector3 currentPosKm = ObjectData.GetPositionAtTime(currentCatcherTime);
        Vector3 finalPosition = currentPosKm * (float)SimulationManager.ScaleFactor;

        EpochTime nextCatcherTime = new EpochTime(currentCatcherTime);
        nextCatcherTime.addMinutes(1.0 / 60.0);
        Vector3 nextPosKm = ObjectData.GetPositionAtTime(nextCatcherTime);
        Vector3 nextLocalPosition = nextPosKm * (float)SimulationManager.ScaleFactor;

        Vector3 forwardDir = (nextLocalPosition - finalPosition).normalized;

        if (PlugInOffset > 0f)
        {
            float catchRatio = (float)(_currentCatchProgressSeconds / maxProgressSeconds);
            float smoothOffsetMultiplier = Mathf.Pow(catchRatio, 4.0f); 
            finalPosition -= forwardDir * (PlugInOffset * smoothOffsetMultiplier);
        }

        transform.localPosition = finalPosition;

        Vector3 upDir = transform.localPosition.normalized; 
        if (forwardDir != Vector3.zero)
        {
            Quaternion baseRotation = Quaternion.LookRotation(forwardDir, upDir);
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

    private void OnCatcherTapped(object sender, System.EventArgs e)
    {
        SimulationManager.Instance.SelectCatcher(this.ObjectData);
        CameraManager.Instance.FollowDebris(this.gameObject);
    }

    private void OnCatcherLongPressed(object sender, System.EventArgs e)
    {
        if (AnneauController.Instance != null)
        {
            AnneauController.Instance.OpenMenuForCatcher(this);
        }
    }
}