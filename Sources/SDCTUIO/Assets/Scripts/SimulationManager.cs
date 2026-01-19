using System;
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

    void Awake() 
    {
        Instance = this;
    }

    void Start()
    {
        this.SimulationTime = new EpochTime(DateTime.UtcNow);
        this.SimulationScaleFactor = Earth.Radius / EARTH_RADIUS_KM;

        DebrisData testDebris = DebrisData.TestDebrisData(45.0f, 45.0f);
        DebrisManager.Instance.AddDebrisToSimulation(DebrisData.TestDebrisData());
        DebrisManager.Instance.AddDebrisToSimulation(DebrisData.TestDebrisData(90.0f, 0.0f));
        DebrisManager.Instance.AddDebrisToSimulation(testDebris);
        DebrisManager.Instance.SelectDebris(testDebris.Id);
    }

    void Update()
    {
        this.SimulationTime.addTick(Time.deltaTime * this.SimulationSpeed);

        /// DEBUG
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (this.SimulationSpeed > 0.0f)
            {
                this.StopSimulation();
            }
            else
            {
                this.ResumeSimulation();
            }
        }
    }

    public void StopSimulation()
    {
        this.SavedSimulationSpeed = this.SimulationSpeed;
        this.SimulationSpeed = 0.0f;
    }

    public void ResumeSimulation()
    {
        this.SimulationSpeed = this.SavedSimulationSpeed;
    }
}
