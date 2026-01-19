using One_Sgp4;
using TouchScript.Gestures;
using UnityEngine;
using System.Numerics;

public class DebrisController : MonoBehaviour
{
    public Tle Tle { get; set; }
    public DebrisData DebrisData { get; private set; }
    private bool IsInitialized = false;

    public void AssignDebrisData(DebrisData debrisData)
    {
        this.Tle = debrisData.ToTle();
        this.DebrisData = debrisData;
        this.IsInitialized = true;
    }

    public void OnEnable()
    {
        // GetComponent<TapGesture>().Tapped += OnDebrisTapped;
    }

    public void OnDisable()
    {
        // GetComponent<TapGesture>().Tapped -= OnDebrisTapped;
    }

    private void Update()
    {
        if (!this.IsInitialized)
        {
            return;
        }

        transform.position = this.DebrisData.GetPositionKmAtTime(SimulationManager.Instance.SimulationTime).ToUnityVector3() * SimulationManager.Instance.SimulationScaleFactor;
    }

    private void OnDebrisTapped(object sender, System.EventArgs e)
    {
        // DebrisManager.Instance.SelectDebris(this.gameObject);
        // Debug.Log("Debris tapped: " + this.DebrisData.Name);
    }
}
