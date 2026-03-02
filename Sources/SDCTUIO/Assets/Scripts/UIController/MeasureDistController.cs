using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MeasureDistController : MonoBehaviour
{
    // La barre de distance
    VisualElement bar;
    private Label measureText;
    private float lastWidth = -1f;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var mwgt = root.Q<VisualElement>("MeasureWidget");
        bar = mwgt.Q<VisualElement>("MeasureBar");

        measureText = mwgt.Q<Label>("MeasureText");
    }

    void Update()
    {
        //bar.style.paddingLeft = new Length(Time.frameCount % 100f, LengthUnit.Percent);
        //var ds = bar.dataSource as DistMeasureScriptableObject;
        //ds.ratio = Time.frameCount % 100f;
       
        if (bar == null || measureText == null || Camera.main == null || SimulationManager.Instance == null) return;

        float distToEarth = Vector3.Distance(Camera.main.transform.position, SimulationManager.Instance.Earth.transform.position);

        float screenWorldWidth = 2.0f * distToEarth * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);

        float kmPerPixel = (screenWorldWidth / Screen.width) / SimulationManager.ScaleFactor;

        float targetKm = 100f * kmPerPixel;

        float magnitude = Mathf.Pow(10, Mathf.Floor(Mathf.Log10(targetKm)));
        float normalized = targetKm / magnitude;
        
        float niceMultiplier = 1f;
        if (normalized >= 5f) niceMultiplier = 5f;
        else if (normalized >= 2f) niceMultiplier = 2f;
        
        float niceKm = niceMultiplier * magnitude;

        float actualPixels = niceKm / kmPerPixel;

        if (Mathf.Abs(lastWidth - actualPixels) > 1.0f)
        {
            lastWidth = actualPixels;

            bar.style.width = actualPixels;

            if (niceKm >= 1f)
            {
                measureText.text = $"{niceKm} km";
            }
            else
            {
                measureText.text = $"{(niceKm * 1000f)} m";
            }
        }
    }
}
