using System;
using System.Collections.Generic;
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

    // LeftPane to create real Tle
    private RadioButton _radioCustom;
    private RadioButton _radioExisting;
    private VisualElement _existingSettingsContainer;
    private VisualElement _rightPane;
    private IntegerField _inputExistingID;
    private DropdownField _dropdownExistingList;
    private bool _isRealMode = false;
    private List<string> _realDebrisIds = new List<string>();

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

        // Part of leftPane, add real debris
        _radioCustom = root.Q<RadioButton>("radioCustom");
        _radioExisting = root.Q<RadioButton>("radioExisting");
        _existingSettingsContainer = root.Q<VisualElement>("ExistingSettingsContainer");
        _inputExistingID = root.Q<IntegerField>("inputExistingID");
        _dropdownExistingList = root.Q<DropdownField>("dropdownExistingList");
        _rightPane = root.Q<VisualElement>("RightPane");
        
        if (_radioCustom != null) 
            _radioCustom.RegisterValueChangedCallback(evt => { if (evt.newValue) SetCreationMode(false); });
            
        if (_radioExisting != null) 
            _radioExisting.RegisterValueChangedCallback(evt => { if (evt.newValue) SetCreationMode(true); });

        if (_dropdownExistingList != null && _inputExistingID != null)
        {
            _dropdownExistingList.RegisterValueChangedCallback(evt => {

                if (_dropdownExistingList.index >= 0 && _dropdownExistingList.index < _realDebrisIds.Count)
                {
                    string selectedIdStr = _realDebrisIds[_dropdownExistingList.index];
                    if (int.TryParse(selectedIdStr, out int idResult))
                    {
                        _inputExistingID.SetValueWithoutNotify(idResult);
                        UpdatePreviewForRealDebris(selectedIdStr);
                    }
                }
            });

            _inputExistingID.RegisterValueChangedCallback(evt => {
                string searchId = evt.newValue.ToString();
                
                if (TLEManager.Instance != null && TLEManager.Instance.AvailableRealDebris.ContainsKey(searchId))
                {
                    int targetIndex = _realDebrisIds.IndexOf(searchId);
                    if (targetIndex >= 0)
                    {
                        _dropdownExistingList.index = targetIndex;
                        _dropdownExistingList.SetValueWithoutNotify(_dropdownExistingList.choices[targetIndex]);
                        UpdatePreviewForRealDebris(searchId);
                    }
                }
                else
                {
                    _dropdownExistingList.index = -1;
                    _dropdownExistingList.SetValueWithoutNotify("Unknown ID...");
                }
            });
        }
    }

    private void SetCreationMode(bool isReal)
    {
        _isRealMode = isReal;

        if (_rightPane != null)
            _rightPane.style.display = DisplayStyle.Flex;

        if (_existingSettingsContainer != null)
            _existingSettingsContainer.style.display = isReal ? DisplayStyle.Flex : DisplayStyle.None;

        //  lock right pane to avoid user modify
        string[] controlsToLock = { "OrbitFirstAxis", "OrbitSecondAxis", "InitialPosition", "DistanceFromEarth", "Mass", "Shape", "Height", "Length", "Width", "Name" };
        foreach (string controlName in controlsToLock)
        {
            var control = _rightPane?.Q<VisualElement>(controlName);
            if (control != null) control.SetEnabled(!isReal);
        }

        if (isReal && TLEManager.Instance != null)
        {
            _dropdownExistingList.choices.Clear();
            _realDebrisIds.Clear();
            
            foreach (var kvp in TLEManager.Instance.AvailableRealDebris)
            {
                _dropdownExistingList.choices.Add(kvp.Value.Name);
                _realDebrisIds.Add(kvp.Key);
            }

            if (_dropdownExistingList.choices.Count > 0)
            {
                _dropdownExistingList.index = 0;
                _dropdownExistingList.value = _dropdownExistingList.choices[0];
                
                if (int.TryParse(_realDebrisIds[0], out int firstId))
                {
                    _inputExistingID.SetValueWithoutNotify(firstId);
                    UpdatePreviewForRealDebris(_realDebrisIds[0]);
                }
            }
        }
    }

    private void OnCreateButtonClicked()
    {
        DebrisData data;
        if (_isRealMode)
        {
            string selectedId = _inputExistingID.value.ToString();
            
            if (TLEManager.Instance == null || !TLEManager.Instance.AvailableRealDebris.TryGetValue(selectedId, out RealDebrisEntry entry))
            {
                Debug.LogError($"Couldn't found the real debris with ID : {selectedId}");
                return;
            }
            
            data = new DebrisData(entry); 
        }
        else
        {
            data = _dataSource.CreateDebrisData();
        }

        GameObject debrisObject = SimulationManager.Instance.AddDebrisToSimulation(data);
        SimulationManager.Instance.SelectDebris(data.Id);
        CameraManager.Instance.FollowDebris(debrisObject);

        DisableModal();
    }

    private void UpdatePreviewForRealDebris(string noradId)
    {
        if (TLEManager.Instance == null || !TLEManager.Instance.AvailableRealDebris.TryGetValue(noradId, out RealDebrisEntry realData))
            return;

        string line2 = realData.TleLine2;
        if (string.IsNullOrEmpty(line2) || line2.Length < 63) return;

        try
        {
            var culture = System.Globalization.CultureInfo.InvariantCulture;

            if (float.TryParse(line2.Substring(8, 8).Trim(), System.Globalization.NumberStyles.Any, culture, out float tilt))
                _wizard.Q<Slider>("OrbitFirstAxis").value = tilt;

            if (float.TryParse(line2.Substring(17, 8).Trim(), System.Globalization.NumberStyles.Any, culture, out float ascNode))
                _wizard.Q<Slider>("OrbitSecondAxis").value = ascNode;

            if (float.TryParse(line2.Substring(43, 8).Trim(), System.Globalization.NumberStyles.Any, culture, out float anomaly))
                _wizard.Q<Slider>("InitialPosition").value = anomaly;

            if (double.TryParse(line2.Substring(52, 11).Trim(), System.Globalization.NumberStyles.Any, culture, out double meanMotion) && meanMotion > 0)
            {
                double periodSeconds = 86400.0 / meanMotion;
                double mu = 398600.4418; 
                double a = Math.Pow((mu * periodSeconds * periodSeconds) / (4.0 * Math.PI * Math.PI), 1.0 / 3.0);
                
                float distance = (float)(a - SimulationManager.EARTH_RADIUS_KM);

                var distField = _wizard.Q<SDCNumericField>("DistanceFromEarth");
                if (distField != null)
                {
                    distance = Mathf.Clamp(distance, _dataSource.MIN_DEBRIS_DISTANCE_FROM_EARTH_KM, _dataSource.MAX_DEBRIS_DISTANCE_FROM_EARTH_KM);
                    distField.value = distance;
                }
            }

            var nameField = _wizard.Q<TextField>("Name");
            if (nameField != null) nameField.value = realData.Name;

        }
        catch (Exception e)
        {
            Debug.LogWarning($"Analyse TLE error : {e.Message}");
        }
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
        Label nameError = _wizard.Q<Label>("NameError");

        bool nameAlreadyExists = debrisNameList.Contains(newValue);
        bool isWhitespace = string.IsNullOrWhiteSpace(newValue);

        if (nameAlreadyExists || isWhitespace)
        {
            if (!field.ClassListContains("base-field-error"))
            {
                field.AddToClassList("base-field-error");
                OnFieldError();  
            }

            nameError.AddToClassList("error-label");
            if (nameAlreadyExists)
            {
                nameError.text = "Debris with this name already exists";
            }
            else if (isWhitespace)
            {
                nameError.text = "Debris name cannot be empty";
            }
        }
        else
        {
            if (field.ClassListContains("base-field-error"))
            {
                field.RemoveFromClassList("base-field-error");
                OnFieldCorrect();  
            }

            nameError.RemoveFromClassList("error-label");
            nameError.text = "";
        }
    }
}
