using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class DebrisCreationController : MonoBehaviour
{
    private const string INITIAL_DEBRIS_NAME = "New debris";
    private const float INITIAL_DEBRIS_DISTANCE_FROM_EARTH_KM = 200f;
    private const float INITIAL_DEBRIS_MASS_KG = 1f;
    private const float INITIAL_DEBRIS_WIDTH_M = 1f;
    private const float INITIAL_DEBRIS_HEIGHT_M = 1f;
    private const float INITIAL_DEBRIS_LENGTH_M = 1f;
    //private static int _debrisCounter = 1;

    private const float MIN_DEBRIS_DISTANCE_FROM_EARTH_KM = 200f;
    private const float MAX_DEBRIS_DISTANCE_FROM_EARTH_KM = 10000f;
    private const float MIN_DIMENSION_M = 1f;
    private const float MAX_DIMENSION_M = 10000f;

    private Button _cancelButton;
    private Button _createButton;

    private DebrisScriptableObject _dataSource;
    private VisualElement _wizard;
    private VisualElement _modals;
    private GameObject previewScene;
    private PreviewSceneOrbit previewScript;

    private int errorCounter = 0;

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
        _wizard.Q<FloatField>("DistanceFromEarth").RegisterValueChangedCallback(evt => previewScript.Distance = evt.newValue);

        // check for errors in fields
        var distanceFromEarthField = _wizard.Q<FloatField>("DistanceFromEarth");
        var massField = _wizard.Q<FloatField>("Mass");
        var heightField = _wizard.Q<FloatField>("Height");
        var lengthField = _wizard.Q<FloatField>("Length");
        var widthField = _wizard.Q<FloatField>("Width");

        RegisterRangeErrorCallback(distanceFromEarthField, MIN_DEBRIS_DISTANCE_FROM_EARTH_KM, MAX_DEBRIS_DISTANCE_FROM_EARTH_KM);
        RegisterRangeErrorCallback(massField, 1, 10000);
        RegisterRangeErrorCallback(heightField, MIN_DIMENSION_M, MAX_DIMENSION_M);
        RegisterRangeErrorCallback(lengthField, MIN_DIMENSION_M, MAX_DIMENSION_M);
        RegisterRangeErrorCallback(widthField, MIN_DIMENSION_M, MAX_DIMENSION_M);
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

        //_debrisCounter++;

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

        ResetData();
    }

    private void ResetData()
    {
        _dataSource.debrisName = INITIAL_DEBRIS_NAME;
        //_dataSource.debrisName = $"{INITIAL_DEBRIS_NAME} {_debrisCounter}";
        _dataSource.orbitFirstAxis = 0;
        _dataSource.orbitSecondAxis = 0;
        _dataSource.initialPosition = 0;
        _dataSource.distanceFromEarthKm = INITIAL_DEBRIS_DISTANCE_FROM_EARTH_KM;
        _dataSource.mass = INITIAL_DEBRIS_MASS_KG;
        _dataSource.width = INITIAL_DEBRIS_WIDTH_M;
        _dataSource.height = INITIAL_DEBRIS_HEIGHT_M;
        _dataSource.length = INITIAL_DEBRIS_LENGTH_M;
    }

    private void RegisterRangeErrorCallback(FloatField field, float min, float max)
    {
        field.RegisterCallback<ChangeEvent<float>>((evt) =>
        {
            float val = field.value;
            if (val < min || val > max)
            {
                if (!field.GetClasses().Contains("base-field-error"))
                {
                    field.AddToClassList("base-field-error");
                    errorCounter++;
                    if (errorCounter > 0)
                    {
                        _createButton.SetEnabled(false);
                    }   
                }
            }
            else
            {
                if (field.GetClasses().Contains("base-field-error"))
                {
                    field.RemoveFromClassList("base-field-error");
                    errorCounter--;
                    if (errorCounter == 0)
                    {
                        _createButton.SetEnabled(true);
                    }   
                }
            }
        });
    }

}
