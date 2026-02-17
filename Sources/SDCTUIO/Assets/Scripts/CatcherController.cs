using One_Sgp4;
using UnityEngine;
using TouchScript.Gestures;

public class CatcherController : ObjectController<CatcherData>
{    
    // Timer : How many seconds have been running to cath up the debris
    private double _currentCatchProgressSeconds;
    public double CurrentProgressSeconds => _currentCatchProgressSeconds;

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
    }

    void Update()
    {
        if (ObjectData == null) return;

        _currentCatchProgressSeconds += Time.deltaTime * SimulationManager.SimulationSpeed;
        
        Vector3 targetPositionKm = ObjectData.GetPositionAtTime(SimulationManager.SimulationTime, _currentCatchProgressSeconds);
        
        Vector3 newLocalPosition = targetPositionKm * (float)SimulationManager.ScaleFactor;

        transform.localPosition = newLocalPosition;
        
        var debrisRawPos = ObjectData.TargetDebris.GetPositionKmAtTime(SimulationManager.SimulationTime);
        
        Vector3 debrisTargetPos = new Vector3((float)debrisRawPos.X, (float)debrisRawPos.Y, (float)debrisRawPos.Z) * (float)SimulationManager.ScaleFactor;

        transform.LookAt(transform.localPosition + (transform.localPosition - debrisTargetPos));
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