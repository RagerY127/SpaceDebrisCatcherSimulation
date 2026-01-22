using One_Sgp4;
using TouchScript.Gestures;
using UnityEngine;

public class DebrisController : MonoBehaviour
{
    public Tle Tle { get; set; }
    public DebrisData DebrisData { get; private set; }
    private bool IsInitialized = false;
    private Vector3 RotationAxis;

    [SerializeField]
    private TapGesture TapGesture;

    public void AssignDebrisData(DebrisData debrisData)
    {
        this.Tle = debrisData.ToTle();
        this.DebrisData = debrisData;
        this.IsInitialized = true;
    }

    public void OnEnable()
    {
        TapGesture.Tapped += OnDebrisTapped;
        RotationAxis = Random.insideUnitSphere;
        RotationAxis.Normalize();
    }

    public void OnDisable()
    {
        TapGesture.Tapped -= OnDebrisTapped;
    }

    private void Update()
    {
        if (!this.IsInitialized)
        {
            return;
        }

        transform.position = this.DebrisData.GetPositionKmAtTime(SimulationManager.SimulationTime).ToUnityVector3() * SimulationManager.ScaleFactor;
        transform.Rotate(RotationAxis, Time.deltaTime * SimulationManager.SimulationSpeed);
    }

    private void OnDebrisTapped(object sender, System.EventArgs e)
    {
        DebrisManager.Instance.SelectDebris(this.DebrisData.Id);
        CameraManager.Instance.FollowDebris(this.gameObject);
    }
}
