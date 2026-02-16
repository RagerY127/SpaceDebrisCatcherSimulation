using UnityEngine;
using UnityEngine.UIElements;

public class DebrisCreationController : MonoBehaviour
{
    private Button _cancelButton;
    private Button _createButton;

    private DebrisScriptableObject _dataSource;
    private VisualElement _wizard;
    private VisualElement _modals;
    private GameObject previewScene;
    private PreviewSceneOrbit previewScript;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _wizard = root.Q<VisualElement>("DebrisCreationWizard");
        _modals = root.Q<VisualElement>("Modals");
        _dataSource = (DebrisScriptableObject)_wizard.dataSource;

        _cancelButton = root.Q<Button>("Cancel");
        _createButton = root.Q<Button>("Create");

        if (_cancelButton != null)
            _cancelButton.clicked += OnCancelButtonClicked;

        if (_createButton != null)
            _createButton.clicked += OnCreateButtonClicked;

        // création de la scène
        previewScene = (GameObject) Instantiate(Resources.Load("Prefabs/PreviewScene"));
        previewScript = previewScene.GetComponent<PreviewSceneOrbit>();
        _wizard.Q<Slider>("OrbitFirstAxis").RegisterValueChangedCallback(evt => previewScript.TiltAngle = evt.newValue);
        _wizard.Q<Slider>("OrbitSecondAxis").RegisterValueChangedCallback(evt => previewScript.AscendingNodeAngle = evt.newValue);
        _wizard.Q<Slider>("InitialPosition").RegisterValueChangedCallback(evt => previewScript.PositionAngle = evt.newValue);
        _wizard.Q<IntegerField>("DistanceFromEarth").RegisterValueChangedCallback(evt => previewScript.Distance = evt.newValue);
    }

    private void OnCreateButtonClicked()
    {
        // TODO: verify if input is valid
        DebrisData data = new DebrisData(_dataSource.debrisName, _dataSource.orbitFirstAxis,
        _dataSource.orbitSecondAxis, _dataSource.initialPosition, _dataSource.distanceFromEarthKm, _dataSource.mass, _dataSource.shape,
        _dataSource.height, _dataSource.length, _dataSource.width);

        GameObject debrisObject = SimulationManager.Instance.AddDebrisToSimulation(data);
        SimulationManager.Instance.SelectDebris(data.Id);
        CameraManager.Instance.FollowDebris(debrisObject);

        DisableModal();
    }

    private void OnCancelButtonClicked()
    {
        DisableModal();
    }

    private void DisableModal()
    {
        _wizard.visible = false;
        _modals.visible = false;
    }

    public void ShowWizard()
    {
        if (_wizard != null) _wizard.visible = true;
        if (_modals != null) _modals.visible = true;
        
    }
}
