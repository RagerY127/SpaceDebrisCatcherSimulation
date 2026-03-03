using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class DebrisCreationController : MonoBehaviour
{
    //private static int _debrisCounter = 1;

    private Button _cancelButton;
    private Button _createButton;
    private TextField _nameField;

    private DebrisScriptableObject _dataSource;
    private VisualElement _wizard;
    private VisualElement _modals;
    private GameObject previewScene;
    private PreviewSceneOrbit previewScript;

    private int errorCounter = 0;
    private HashSet<string> debrisNameList;

    void OnEnable()
    {
        debrisNameList = new();

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
        _wizard.Q<DropdownField>("Shape").RegisterValueChangedCallback(evt => previewScript.Shape = (DebrisShape)Enum.Parse(typeof(DebrisShape), evt.newValue));

        // make sure debris name is unique
        _nameField = _wizard.Q<TextField>("Name");
        _nameField.RegisterValueChangedCallback(evt => OnNameChange(_nameField, evt.newValue));

        // add error handling to ranged inputs
        _wizard.Query<SDCNumericField>().ForEach(field => {
            field.JustBecameOutOfRange += OnFieldError;
            field.JustEnteredRange += OnFieldCorrect; 
        });
    }

    private void OnCreateButtonClicked()
    {
        DebrisData data = _dataSource.CreateDebrisData();

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

        _dataSource.ResetData();
        RefreshDebrisNameList();
        OnNameChange(_nameField, _dataSource.debrisName); // to make sure initial value is seen as changed
    }

    private void OnFieldError()
    {
        errorCounter++;
        _createButton.SetEnabled(false);
    }

    private void OnFieldCorrect()
    {
        errorCounter--;
        if (errorCounter == 0)
        {
             _createButton.SetEnabled(true);
        }
    }

    private void RefreshDebrisNameList()
    {
        debrisNameList.Clear();

        foreach (GameObject debris in SimulationManager.Instance.DebrisObjects.Values) {
            debrisNameList.Add(debris.GetComponent<DebrisController>().ObjectData.Name);
        }
    }

    private void OnNameChange(TextField field, string newValue)
    {
        if (debrisNameList.Contains(newValue))
        {
            if (!field.ClassListContains("base-field-error"))
            {
                field.AddToClassList("base-field-error");
                OnFieldError();  
            }
        }
        else
        {
            if (field.ClassListContains("base-field-error"))
            {
                field.RemoveFromClassList("base-field-error");
                OnFieldCorrect();  
            }
        }
    }
}
