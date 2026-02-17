using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class CreateDebrisController : MonoBehaviour
{
    void OnCreateDebris()
    {
        var dd = DebrisData.RandomDebris();
        SimulationManager.Instance.AddDebrisToSimulation(dd);
        SimulationManager.Instance.SelectDebris(dd.Id);
        var debris = SimulationManager.Instance.FindDebrisFromId(dd.Id);
        if (debris != null)
            CameraManager.Instance.FollowDebris(debris);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        // var createButton = root.Q<Button>("CreateDebrisButton");
        // createButton.clicked += OnCreateDebris;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
