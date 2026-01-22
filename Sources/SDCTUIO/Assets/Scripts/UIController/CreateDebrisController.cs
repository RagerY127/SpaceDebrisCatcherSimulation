using UnityEngine;
using UnityEngine.UIElements;

public class CreateDebrisController : MonoBehaviour
{
    void OnCreateDebris()
    {
        var dd = DebrisData.RandomDebris();
        DebrisManager.Instance.AddDebrisToSimulation(dd);
        DebrisManager.Instance.SelectDebris(dd.Id);
        var debris = DebrisManager.Instance.FindDebrisFromId(dd.Id);
        if (debris != null)
            CameraManager.Instance.FollowDebris(debris);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var createButton = root.Q<Button>("CreateDebrisButton");
        createButton.clicked += OnCreateDebris;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
