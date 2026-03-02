using One_Sgp4;
using UnityEngine;
using DigitalRuby.Earth;
using System.Collections.Generic;

public class SimulationManager : MonoBehaviour
{
    public static SimulationManager Instance { get; private set; }

    public const float EARTH_RADIUS_KM = 6371.0f;

    public const double DEFAULT_CATCHER_LAG_MINUTES = 5.0;

    // Simulation data
    [SerializeField]
    private EarthScript _earth;
    [SerializeField]
    private float _simulationSpeed;
    private float _savedSimulationSpeed;
    private EpochTime _simulationTime;
    private float _scaleFactor;
    private bool _isRunning;

    // Debris managing data
    [SerializeField]
    private int _orbitPointCount;
    [SerializeField]
    private LineRenderer _lineRenderer;
    private Dictionary<string, GameObject> _debrisObjects; 
    public Dictionary<string, GameObject> DebrisObjects => _debrisObjects;
    public GameObject DebrisPrefab;
    private GameObject _selectedDebris;

    // Catcher managing data
    public GameObject CatcherPrefab;
    private GameObject _catcher;
    private GameObject _targetDebris;

    // If existe already a catcher
    public bool HasCatcher => _catcher != null;

    // Static passthroughs
    public static float SimulationSpeed => Instance._simulationSpeed;
    public static EpochTime SimulationTime => Instance._simulationTime;
    public static float ScaleFactor => Instance._scaleFactor;
    public static bool IsRunning => Instance._isRunning;

    public EarthScript Earth => _earth;

    public delegate void ControllerChangeHandler<T> (ObjectController<T> instance) where T : ObjectData;
    public event ControllerChangeHandler<DebrisData> DebrisAdded;
    public delegate void IdChangeHandler(string id);
    public event IdChangeHandler DebrisRemoving;
    public event IdChangeHandler DebrisTapped;
    public delegate void CatcherInfoUpdateHandler(string catcherName, string debrisName);
    public event CatcherInfoUpdateHandler CatcherInfoUpdate;

    void Awake()
    {
        Instance = this;
        _isRunning = true;
        _debrisObjects = new();
        _catcher = null;
        _targetDebris = null;
    }

    void Start()
    {
        _simulationTime = new EpochTime(System.DateTime.UtcNow);
        _scaleFactor = _earth.Radius / EARTH_RADIUS_KM;

         //DebrisData test = DebrisData.RandomDebris();
         //AddDebrisToSimulation(test);
         //AssignCatcherToDebris(test.Id);

        // for (int i = 0; i < 5; i++)
        // {
        //     AddDebrisToSimulation(DebrisData.RandomDebris());
        // }
    }

    void Update()
    {
        _simulationTime.addTick(Time.deltaTime * _simulationSpeed);

        // DEBUG: Remove selected debris on backspace
        // if (Input.GetKeyDown(KeyCode.Backspace))
        // {
        //     RemoveDebris(_selectedDebris.GetComponent<DebrisController>().DebrisData.Id);
        // }
    }

    /// <summary>
    /// Sets the simulation's speed multiplier.
    /// </summary>
    /// <param name="speed"></param>
    public void SetSimulationSpeed(float speed)
    {
        _savedSimulationSpeed = speed;
        if (_isRunning)
        {
            _simulationSpeed = speed;   
        }
    }

    /// <summary>
    /// Stops the simulation.
    /// </summary>
    public void StopSimulation()
    {
        _savedSimulationSpeed = SimulationSpeed;
        _simulationSpeed = 0.0f;
        _isRunning = false;
    }

    /// <summary>
    /// Resumes the simulation.
    /// </summary>
    public void ResumeSimulation()
    {
        _simulationSpeed = _savedSimulationSpeed;
        _isRunning = true;
    }

    /// <summary>
    /// Adds a debris object to the simulation.
    /// </summary>
    /// <param name="debrisData">The data of the debris to be added.</param>
    public GameObject AddDebrisToSimulation(DebrisData debrisData)
    {
        GameObject debrisObject = Instantiate(this.DebrisPrefab, this.transform);
        DebrisController debrisController = debrisObject.GetComponent<DebrisController>();
        debrisController.AssignDebrisData(debrisData);

        _debrisObjects[debrisData.Id] = debrisObject;

        // Change prefab data from the debris data (TODO make better)
        switch (debrisData.Shape)
        {
            case DebrisShape.Cube:
                debrisObject.GetComponent<MeshFilter>().mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
                break;
            case DebrisShape.Cylinder:
                debrisObject.GetComponent<MeshFilter>().mesh = Resources.GetBuiltinResource<Mesh>("Cylinder.fbx");
                break;
        }
        DebrisAdded?.Invoke(debrisController);

        return debrisObject;
    }

    /// <summary>
    /// Removes a debris from the scene.
    /// </summary>
    /// <param name="debrisId">The ID of the debris to be removed.</param>
    public void RemoveDebris(string debrisId)
    {
        // TODO: affichier une alerte pour dire un debris a ete supprime

        if (_debrisObjects.ContainsKey(debrisId))
        {
            GameObject debrisObject = _debrisObjects[debrisId];
            if (_selectedDebris == debrisObject)
            {
                DeselectDebris();
            }

            if (_targetDebris == debrisObject)
            {
                DestroyCatcher(); 
            }
            // Event dispatch avant que le débris soit supprimé
            DebrisRemoving?.Invoke(debrisId);

            _debrisObjects.Remove(debrisId);
            Destroy(debrisObject);
        }
    }

    /// <summary>
    /// Selects a debris to follow with the camera and draws its orbit.
    /// </summary>
    /// <param name="debrisId">The ID of the debris to be selected.</param>
    public void SelectDebris(string debrisId)
    {
        if (!_debrisObjects.ContainsKey(debrisId))
        {
            return;
        }

        _selectedDebris = _debrisObjects[debrisId];
        DrawDebrisOrbit(debrisId);
    }

    /// <summary>
    /// Deselects the debris.
    /// </summary>
    public void DeselectDebris()
    {
        _selectedDebris = null;
        _lineRenderer.positionCount = 0;
    }

    /// <summary>
    /// Finds the debris GameObject from a given ID.
    /// </summary>
    /// <param name="debrisId">The ID of the debris.</param>
    /// <returns></returns>
    public GameObject FindDebrisFromId(string debrisId)
    {
        return _debrisObjects.GetValueOrDefault(debrisId, null);
    }

    /// <summary>
    /// Draws the orbit of a debris.
    /// </summary>
    /// <param name="debrisId">The ID of the debris.</param>
    public void DrawDebrisOrbit(string debrisId)
    {
        if (!_debrisObjects.ContainsKey(debrisId))
        {
            return;
        }

        DebrisController debrisController = _debrisObjects[debrisId].GetComponent<DebrisController>();

        _lineRenderer.positionCount = _orbitPointCount;
        _lineRenderer.loop = true;

        Vector3[] orbitPoints = new Vector3[_orbitPointCount];

        float meanMotion = debrisController.ObjectData.RevolutionsPerDay;
        float periodMinutes = 60f * 24f / meanMotion;

        EpochTime startTime = new EpochTime(SimulationTime);
        for (int i = 0; i < _orbitPointCount; i++)
        {
            double timeOffsetMinutes = periodMinutes / _orbitPointCount * i;

            EpochTime time = new EpochTime(startTime);
            time.addMinutes(timeOffsetMinutes);

            orbitPoints[i] = debrisController.ObjectData.GetPositionKmAtTime(time).ToUnityVector3() * ScaleFactor;
        }

        _lineRenderer.SetPositions(orbitPoints);
    }

    /// <summary>
    /// Selects the catcher. Since the catcher follows a debris orbit, we draw the target debris's orbit.
    /// </summary>
    /// <param name="catcherData">The data of the catcher to be selected.</param>
    public void SelectCatcher(CatcherData catcherData)
    {
        if (catcherData == null || catcherData.TargetDebris == null)
        {
            return;
        }

        DrawDebrisOrbit(catcherData.TargetDebris.Id);
        
        // _selectedCatcherData = catcherData; 
    }

    public void AssignCatcherToDebris(string debrisId, double timeLagMinutes = DEFAULT_CATCHER_LAG_MINUTES)
    {
        if (!_debrisObjects.ContainsKey(debrisId))
        {
            return;
        }

        DestroyCatcher();

        _targetDebris = _debrisObjects[debrisId];

        _catcher = Instantiate(this.CatcherPrefab, this.transform);
        CatcherController controller = _catcher.GetComponent<CatcherController>();
        DebrisController targetDebrisController = _targetDebris.GetComponent<DebrisController>();
        controller.AssignTargetDebris(targetDebrisController,timeLagMinutes);

        CatcherInfoUpdate?.Invoke("Catcher", targetDebrisController.ObjectData.Name);
    }
    
    public void DestroyCatcher()
    {
        if (_catcher)
        {
            Destroy(_catcher);
            _catcher = null;
        }

        _targetDebris = null;
        
        CatcherInfoUpdate?.Invoke(null, null);
    }
}
