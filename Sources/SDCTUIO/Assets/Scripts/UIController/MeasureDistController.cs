using UnityEngine;
using UnityEngine.UIElements;

public class MeasureDistController : MonoBehaviour
{
    // La barre de distance
    VisualElement bar;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var mwgt = root.Q<VisualElement>("MeasureWidget");
        bar = mwgt.Q<VisualElement>("MeasureBar");
    }

    void Update()
    {
        //bar.style.paddingLeft = new Length(Time.frameCount % 100f, LengthUnit.Percent);
        var ds = bar.dataSource as DistMeasureScriptableObject;
        ds.ratio = Time.frameCount % 100f;
    }
}
