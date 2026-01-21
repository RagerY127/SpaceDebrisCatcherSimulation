using One_Sgp4;
using UnityEngine;
using DigitalRuby.Earth;

public class SimulationManager : MonoBehaviour
{
    public const float EARTH_RADIUS_KM = 6371.0f;

    [SerializeField]
    private EarthScript Earth;
    [SerializeField]
    public float SimulationSpeed;
    private float SavedSimulationSpeed;

    public static SimulationManager Instance { get; private set; }

    public EpochTime SimulationTime { get; private set; }
    public float SimulationScaleFactor { get; private set; }
    public bool IsSimulationRunning { get; private set; } 

    void Awake() 
    {
        Instance = this;
        IsSimulationRunning = true;
    }

    void Start()
    {
        this.SimulationTime = new EpochTime(System.DateTime.UtcNow);
        this.SimulationScaleFactor = Earth.Radius / EARTH_RADIUS_KM;

        for (int i = 0; i < 20; i++)
        {
            DebrisManager.Instance.AddDebrisToSimulation(DebrisData.RandomDebris());
        }
    }

    void Update()
    {
        this.SimulationTime.addTick(Time.deltaTime * this.SimulationSpeed);
    }

    public void SetSimulationSpeed(float speed)
    {
        this.SavedSimulationSpeed = speed;
        if (this.IsSimulationRunning)
        {
            this.SimulationSpeed = speed;   
        }
    }

    public void StopSimulation()
    {
        this.SavedSimulationSpeed = this.SimulationSpeed;
        this.SimulationSpeed = 0.0f;
        this.IsSimulationRunning = false;
    }

    public void ResumeSimulation()
    {
        this.SimulationSpeed = this.SavedSimulationSpeed;
        this.IsSimulationRunning = true;
    }
}
