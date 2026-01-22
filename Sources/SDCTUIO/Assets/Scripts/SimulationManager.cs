using One_Sgp4;
using UnityEngine;
using DigitalRuby.Earth;

public class SimulationManager : MonoBehaviour
{
    public static SimulationManager Instance { get; private set; }

    public const float EARTH_RADIUS_KM = 6371.0f;

    [SerializeField]
    private EarthScript _earth;
    [SerializeField]
    private float _simulationSpeed;
    private float _savedSimulationSpeed;
    private EpochTime _simulationTime;
    private float _scaleFactor;
    private bool _isRunning;

    // Static passthroughs
    public static float SimulationSpeed => Instance._simulationSpeed;
    public static EpochTime SimulationTime => Instance._simulationTime;
    public static float ScaleFactor => Instance._scaleFactor;
    public static bool IsRunning => Instance._isRunning;

    void Awake() 
    {
        Instance = this;
        _isRunning = true;
    }

    void Start()
    {
        _simulationTime = new EpochTime(System.DateTime.UtcNow);
        _scaleFactor = _earth.Radius / EARTH_RADIUS_KM;

        for (int i = 0; i < 20; i++)
        {
            DebrisManager.Instance.AddDebrisToSimulation(DebrisData.RandomDebris());
        }
    }

    void Update()
    {
        _simulationTime.addTick(Time.deltaTime * _simulationSpeed);

        // DEBUG: Remove selected debris on backspace
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            DebrisManager.Instance.RemoveSelectedDebris();
        }
    }

    public void SetSimulationSpeed(float speed)
    {
        _savedSimulationSpeed = speed;
        if (_isRunning)
        {
            _simulationSpeed = speed;   
        }
    }

    public void StopSimulation()
    {
        _savedSimulationSpeed = SimulationSpeed;
        _simulationSpeed = 0.0f;
        _isRunning = false;
    }

    public void ResumeSimulation()
    {
        _simulationSpeed = _savedSimulationSpeed;
        _isRunning = true;
    }
}
