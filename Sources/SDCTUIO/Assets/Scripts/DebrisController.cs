using One_Sgp4;
using TouchScript.Gestures;
using UnityEngine;

public class DebrisController : MonoBehaviour
{
    private Tle Tle { get; set; }
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

        Sgp4Data sgp4DebrisData = SatFunctions.getSatPositionAtTime(
            this.Tle,
            SimulationManager.Instance.SimulationTime,
            Sgp4.wgsConstant.WGS_84
        );

        // ATTENTION: Unity utilise un systeme de coordonnees Y-up tandis que SGP4 utilise Z-up
        Point3d realPositionKm = sgp4DebrisData.getPositionData();
        transform.position = new Vector3(
            (float)realPositionKm.x,
            (float)realPositionKm.z,
            (float)realPositionKm.y
        ) * SimulationManager.Instance.SimulationScaleFactor;
    }

    private void OnDebrisTapped(object sender, System.EventArgs e)
    {
        // DebrisManager.Instance.SelectDebris(this.gameObject);
        // Debug.Log("Debris tapped: " + this.DebrisData.Name);
    }
}
