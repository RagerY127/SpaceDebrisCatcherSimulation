using UnityEngine;
using UnityEngine.UIElements;

public class DebrisListUI : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset rowTemplate;
    private DebrisCreationController _creationController;    
    private Button _addDebrisButton;
    private ScrollView scrollView;
    private UIDocument uiDocument;

    private VisualElement _currentlySelectedRow;
    private string _selectedDebrisId;
    private Button _deleteButton;

    private VisualElement _catcherTargetInfo;
    private Label _catcherNameLabel;
    private Label _debrisNameLabel;

    void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
    }

    public void AddDebrisToList(DebrisController controller)
    {
        if (uiDocument == null || uiDocument.rootVisualElement == null) return;

        if (scrollView == null)
        {
            scrollView = uiDocument.rootVisualElement.Q<ScrollView>("debris-scroll-view");
        }

        if (scrollView == null || rowTemplate == null) return;

        VisualElement row = rowTemplate.Instantiate();
        row.name = controller.DebrisData.Id; 
        row.Q<Label>("debris-name").text = controller.DebrisData.Name;
        
        row.RegisterCallback<ClickEvent>(evt => SelectRow(row, controller.DebrisData.Id));

        Button focusBtn = row.Q<Button>("focus-button");
        if (focusBtn != null)
        {
            focusBtn.RegisterCallback<ClickEvent>(evt => 
            {
                SelectRow(row, controller.DebrisData.Id, true);
                
                CameraManager.Instance.FollowDebris(controller.gameObject);
                
                evt.StopPropagation(); 
            });
        }

        scrollView.Add(row);

        SelectRow(row, controller.DebrisData.Id, true);
    }

    public void SelectDebrisRow(string debrisId)
    {
        VisualElement row = scrollView.Q<VisualElement>(debrisId);
        if (row != null)
        {
            SelectRow(row, debrisId, true);
        }
    }

    public void RemoveDebrisFromList(string debrisId)
    {
        if (scrollView == null) return;

        VisualElement rowToDelete = scrollView.Q<VisualElement>(debrisId);
        if (rowToDelete != null)
        {
            if (_selectedDebrisId == debrisId)
            {
                _selectedDebrisId = null;
                _currentlySelectedRow = null;
                _deleteButton?.SetEnabled(false);
            }
            scrollView.Remove(rowToDelete);
        }
    }

    private void SelectRow(VisualElement row, string id, bool forceSelect = false)
    {
        if (_selectedDebrisId == id && !forceSelect)
        {
            DeselectCurrentRow();
            return;
        }

        _currentlySelectedRow?.RemoveFromClassList("selected-row");
        _currentlySelectedRow = row;
        _selectedDebrisId = id;
        _currentlySelectedRow.AddToClassList("selected-row");

        _deleteButton?.SetEnabled(true);

        SimulationManager.Instance.SelectDebris(id);
    }

    private void DeselectCurrentRow()
    {
        _currentlySelectedRow?.RemoveFromClassList("selected-row");
        _currentlySelectedRow = null;
        _selectedDebrisId = null;
        _deleteButton?.SetEnabled(false);
        SimulationManager.Instance.DeselectDebris();
        CameraManager.Instance.UnfollowDebris();
    }

    public void DeleteSelected()
    {
        if (!string.IsNullOrEmpty(_selectedDebrisId))
        {
            SimulationManager.Instance.RemoveDebris(_selectedDebrisId);
        }
    }

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        if (root == null) return;

        _deleteButton = root.Q<Button>("delete");
        if (_deleteButton != null)
        {
            _deleteButton.clicked += DeleteSelected;
            _deleteButton.SetEnabled(false); 
        }
        
        scrollView = root.Q<ScrollView>("debris-scroll-view");
        _catcherTargetInfo = root.Q<VisualElement>("catcher-target-info");
        _catcherNameLabel = root.Q<Label>("catcher-name");
        _debrisNameLabel = root.Q<Label>("debris-name");

        //hide in the beginning
        if (_catcherTargetInfo != null) 
        {
            _catcherTargetInfo.style.display = DisplayStyle.None;
        }
        _creationController = FindFirstObjectByType<DebrisCreationController>();
        _addDebrisButton = root.Q<Button>("createDebris");
        if (_addDebrisButton != null)
        {
            _addDebrisButton.clicked += OpenCreationWizard;
        }
    }

    public void UpdateCatcherInfo(string catcherName, string targetDebrisName)
    {
        if (_catcherTargetInfo == null) return;

        if (string.IsNullOrEmpty(catcherName))
        {
            _catcherTargetInfo.style.display = DisplayStyle.None;
        }
        else
        {
            _catcherTargetInfo.style.display = DisplayStyle.Flex;
            
            if (_catcherNameLabel != null) _catcherNameLabel.text = catcherName;
            if (_debrisNameLabel != null) _debrisNameLabel.text = targetDebrisName;
        }
    }

    private void OpenCreationWizard()
    {
        if (_creationController != null)
        {
            _creationController.ShowWizard();
        }
    }


}