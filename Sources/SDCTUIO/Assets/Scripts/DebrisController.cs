using One_Sgp4;
using TouchScript.Gestures;
using UnityEngine;

public class DebrisController : MonoBehaviour
{
    public Tle Tle { get; set; }
    public DebrisData DebrisData { get; private set; }


    [SerializeField]
    private TapGesture TapGesture;
    // Long gesture in anneau Controller
    [SerializeField]
    public LongPressGesture LongPressGesture;

    public void AssignDebrisData(DebrisData debrisData)
    {
        this.Tle = debrisData.ToTle();
        this.DebrisData = debrisData;
    }

    public void OnEnable()
    {
        TapGesture.Tapped += OnDebrisTapped;

        LongPressGesture.LongPressed += OnDebrisLongPressed;
    }

    public void OnDisable()
    {
        TapGesture.Tapped -= OnDebrisTapped;
    }

    private void Update()
    {
        transform.position = this.DebrisData.GetPositionKmAtTime(SimulationManager.SimulationTime).ToUnityVector3() * SimulationManager.ScaleFactor;
    }

    private void OnDebrisTapped(object sender, System.EventArgs e)
    {
        SimulationManager.Instance.SelectDebris(this.DebrisData.Id);
        CameraManager.Instance.FollowDebris(this.gameObject);
    }

    private void OnDebrisLongPressed(object sender, System.EventArgs e)
    {
        AnneauController.Instance.OpenMenuForDebris(this);
    }
}
