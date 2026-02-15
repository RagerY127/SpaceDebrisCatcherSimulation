using One_Sgp4;
using UnityEngine;
using TouchScript.Gestures;

public class CatcherController : MonoBehaviour
{
    public CatcherData CatcherData { get; private set; }

    // TouchScript gestures
    [SerializeField] private TapGesture TapGesture;
    
    [SerializeField] private LongPressGesture LongPressGesture;
    
    // Timer : How many seconds have been running to cath up the debris
    private double _currentCatchProgressSeconds;
    public double CurrentProgressSeconds => _currentCatchProgressSeconds;

    public DebrisData TargetDebris 
    { 
        get { return CatcherData?.TargetDebris; } 
    }
    
    // Accept DebrisController, Initial lag time
    public void AssignTargetDebris(DebrisController targetController, double initialLagMinutes = SimulationManager.DEFAULT_CATCHER_LAG_MINUTES)
    {
        this.CatcherData = new CatcherData(
            $"Catcher-{targetController.DebrisData.Name}", 
            targetController.DebrisData, 
            initialLagMinutes
        );
        
        _currentCatchProgressSeconds = 0.0f;
    }

    void Update()
    {
        if (CatcherData == null) return;

        _currentCatchProgressSeconds += Time.deltaTime * SimulationManager.SimulationSpeed;
        
        Vector3 targetPositionKm = CatcherData.GetPositionAtTime(SimulationManager.SimulationTime, _currentCatchProgressSeconds);
        
        Vector3 newLocalPosition = targetPositionKm * (float)SimulationManager.ScaleFactor;

        transform.localPosition = newLocalPosition;
        
        var debrisRawPos = CatcherData.TargetDebris.GetPositionKmAtTime(SimulationManager.SimulationTime);
        
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
        SimulationManager.Instance.SelectCatcher(this.CatcherData);
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