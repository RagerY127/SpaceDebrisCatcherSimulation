using System.ComponentModel.Design.Serialization;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DebrisCreationController : MonoBehaviour
{
    private Slider _orbitFirstAxisSlider;
    private Slider _orbitSecondAxisSlider;
    private Slider _initialPositionSlider;
    private Button _cancelButton;
    private Button _createButton;

    private DebrisScriptableObject _dataSource;
    private VisualElement _modal;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _dataSource = (DebrisScriptableObject)root.dataSource;
        _modal = root.Q<VisualElement>("DebrisCreationWizard");

        _orbitFirstAxisSlider = root.Q<Slider>("OrbitFirstAxis");
        _orbitSecondAxisSlider = root.Q<Slider>("OrbitSecondAxis");
        _initialPositionSlider = root.Q<Slider>("InitialPosition");
        _cancelButton = root.Q<Button>("Cancel");
        _createButton = root.Q<Button>("Create");

        if (_createButton != null)
            _createButton.clicked += OnCreateButtonClicked;
    }

    private void OnCreateButtonClicked()
    {
        DebrisData data = new DebrisData(_dataSource.debrisName, _dataSource.orbitFirstAxis,
        _dataSource.orbitSecondAxis, _dataSource.initialPosition, _dataSource.revolutionsPerDay, _dataSource.mass, DebrisShape.Cube,
        _dataSource.height, _dataSource.length, _dataSource.width);

        SimulationManager.Instance.AddDebrisToSimulation(data);
        _modal.visible = false;
    }
}
