using One_Sgp4;
using TouchScript.Gestures;
using UnityEngine;

public class DebrisController : MonoBehaviour
{
    public Tle Tle { get; set; }
    public DebrisData DebrisData { get; private set; }
    private bool IsInitialized = false;

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
    }

    private void OnDebrisTapped(object sender, System.EventArgs e)
    {
        SimulationManager.Instance.SelectDebris(this.DebrisData.Id);
        CameraManager.Instance.FollowDebris(this.gameObject);
    }
}
