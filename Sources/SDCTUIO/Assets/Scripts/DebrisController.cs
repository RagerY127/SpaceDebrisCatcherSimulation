using One_Sgp4;
using TouchScript.Gestures;
using UnityEngine;

public class DebrisController : ObjectController<DebrisData>
{
    public Tle Tle { get; set; }

    public void AssignDebrisData(DebrisData debrisData)
    {
        this.Tle = debrisData.ToTle();
        this.ObjectData = debrisData;
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
        transform.position = this.ObjectData.GetPositionKmAtTime(SimulationManager.SimulationTime).ToUnityVector3() * SimulationManager.ScaleFactor;
    }

    private void OnDebrisTapped(object sender, System.EventArgs e)
    {
        SimulationManager.Instance.SelectDebris(this.ObjectData.Id);
        CameraManager.Instance.FollowDebris(this.gameObject);
    }

    private void OnDebrisLongPressed(object sender, System.EventArgs e)
    {
        AnneauController.Instance.OpenMenuForDebris(this);
    }
}
