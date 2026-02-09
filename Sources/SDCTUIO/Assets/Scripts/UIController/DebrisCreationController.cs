using System.ComponentModel.Design.Serialization;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DebrisCreationController : MonoBehaviour
{
    private Button _cancelButton;
    private Button _createButton;

    private DebrisScriptableObject _dataSource;
    private VisualElement _wizard;
    private VisualElement _modals;

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
    }

    private void OnCreateButtonClicked()
    {
        // TODO: verify if input is valid
        DebrisData data = new DebrisData(_dataSource.debrisName, _dataSource.orbitFirstAxis,
        _dataSource.orbitSecondAxis, _dataSource.initialPosition, _dataSource.distanceFromEarthKm, _dataSource.mass, _dataSource.shape,
        _dataSource.height, _dataSource.length, _dataSource.width);

        SimulationManager.Instance.AddDebrisToSimulation(data);
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
}
