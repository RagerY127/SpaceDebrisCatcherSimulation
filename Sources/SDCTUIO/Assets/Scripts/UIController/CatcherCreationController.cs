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
    private bool _isSelectionValid = false;

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

        if (_debrisDropdown != null)
        {
            _debrisDropdown.RegisterValueChangedCallback(evt => UpdateCreateButtonState());

            _debrisDropdown.style.transitionProperty = new StyleList<StylePropertyName>(new List<StylePropertyName> { new StylePropertyName("scale"), new StylePropertyName("color") });
            _debrisDropdown.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue> { new TimeValue(0.15f, TimeUnit.Second) });
        }

        if (_wizard != null) _wizard.visible = false;
        if (_modals != null) _modals.visible = false;
    }

    public void ShowWizard(string preselectedDebrisId = null)
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

        if (!string.IsNullOrEmpty(preselectedDebrisId) && _availableDebrisIds.Contains(preselectedDebrisId))
        {
            _debrisDropdown.index = _availableDebrisIds.IndexOf(preselectedDebrisId);
            
            int targetIndex = _availableDebrisIds.IndexOf(preselectedDebrisId);
            
            _debrisDropdown.index = targetIndex;
            
            _debrisDropdown.value = _debrisDropdown.choices[targetIndex];
        }
        else{
            _debrisDropdown.index = -1;
            _debrisDropdown.value = "List of existing debris...";
        }

        if (_timeInput != null) _timeInput.value = 5;

        UpdateCreateButtonState();

        _wizard.visible = true;
        if (_modals != null) _modals.visible = true;
    }

    private void UpdateCreateButtonState()
    {
        if (_createButton == null || _debrisDropdown == null) return;

        _isSelectionValid = _debrisDropdown.index >= 0 && _debrisDropdown.index < _availableDebrisIds.Count;

        _createButton.style.opacity = _isSelectionValid ? 1f : 0.2f;
    }

    private void HideWizard()
    {
        if (_wizard != null) _wizard.visible = false;
        if (_modals != null) _modals.visible = false; 
    }

    private void OnCreateButtonClicked()
    {
        if (!_isSelectionValid)
        {
            TriggerDropdownHighlight();
            return;
        }
        if (_availableDebrisIds.Count == 0 || _debrisDropdown.index < 0) return;

        string selectedId = _availableDebrisIds[_debrisDropdown.index];
        int timeLagMinutes = _timeInput.value;

        SimulationManager.Instance.AssignCatcherToDebris(selectedId, timeLagMinutes);
        HideWizard();
    }
    /// <summary>
    /// Zoom in instant to remind user to choose a debris before create
    /// </summary>
    private void TriggerDropdownHighlight()
    {
        if (_debrisDropdown == null) return;

        _debrisDropdown.style.scale = new StyleScale(new Vector3(1.05f, 1.05f, 1f));
        _debrisDropdown.style.color = new StyleColor(new Color(1f, 0.3f, 0.3f));

        _debrisDropdown.schedule.Execute(() =>
        {
            _debrisDropdown.style.scale = new StyleScale(new Vector3(1f, 1f, 1f));
            _debrisDropdown.style.color = new StyleColor(StyleKeyword.Null);
        }).StartingIn(150);
    }
}