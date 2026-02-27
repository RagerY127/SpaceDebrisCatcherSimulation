using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class CatcherCreationController : MonoBehaviour
{
    private VisualElement _wizard;
    private VisualElement _modals;
    private DropdownField _debrisDropdown;
    private IntegerField _timeInput;
    private Button _cancelButton;
    private Button _createButton;

    private List<string> _availableDebrisIds = new List<string>();

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        
        _wizard = root.Q<VisualElement>("CatcherCreationWizard");
        _modals = root.Q<VisualElement>("CatcherModals");
        
        
        _debrisDropdown = root.Q<DropdownField>("DebrisDropdown");
        _timeInput = root.Q<IntegerField>("TimeInput");
        
        _cancelButton = root.Q<Button>("CancelBtn");
        _createButton = root.Q<Button>("CreateBtn");

        if (_cancelButton != null) _cancelButton.clicked += HideWizard;
        if (_createButton != null) _createButton.clicked += OnCreateButtonClicked;

        if (_wizard != null) _wizard.visible = false;
        if (_modals != null) _modals.visible = false;
    }

    public void ShowWizard()
    {

        _debrisDropdown.choices.Clear();
        _availableDebrisIds.Clear();

        var allDebris = SimulationManager.Instance.DebrisObjects;
        foreach (var kvp in allDebris)
        {
            DebrisController debrisCtrl = kvp.Value.GetComponent<DebrisController>();
            _debrisDropdown.choices.Add(debrisCtrl.ObjectData.Name);
            _availableDebrisIds.Add(debrisCtrl.ObjectData.Id);
        }
        // TODO : ALERTE, FAUT CHOISIR UN DEBRIS
        _debrisDropdown.index = -1;
        _debrisDropdown.value = "List of existing debris...";
        if (_timeInput != null) _timeInput.value = 5;

        _wizard.visible = true;
        if (_modals != null) _modals.visible = true;
    }

    private void HideWizard()
    {
        if (_wizard != null) _wizard.visible = false;
        if (_modals != null) _modals.visible = false; 
    }

    private void OnCreateButtonClicked()
    {
        if (_availableDebrisIds.Count == 0 || _debrisDropdown.index < 0) return;

        string selectedId = _availableDebrisIds[_debrisDropdown.index];
        int timeLagMinutes = _timeInput.value;

        SimulationManager.Instance.AssignCatcherToDebris(selectedId, timeLagMinutes);
        HideWizard();
    }
}