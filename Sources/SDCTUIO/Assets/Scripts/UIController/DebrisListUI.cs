using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class DebrisListUI : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset rowTemplate;
    private DebrisCreationController _creationController;    
    // new : catcher controller
    private CatcherCreationController _catcherCreationController;
    private Button _addDebrisButton;
    // new : catcher button
    private Button _addCatcherButton;
    private ScrollView scrollView;
    private UIDocument uiDocument;

    private VisualElement _currentlySelectedRow;
    private string _selectedDebrisId;
    private Button _deleteButton;

    private VisualElement _catcherTargetInfo;
    private Label _catcherNameLabel;
    private Label _debrisNameLabel;

    private TextField _searchField;

    void Awake()
    {
        uiDocument = GetComponent<UIDocument>();

        var sim = SimulationManager.Instance;
        sim.CatcherInfoUpdate += UpdateCatcherInfo;
        sim.DebrisRemoving += RemoveDebrisFromList;
        sim.DebrisTapped += SelectDebrisRow;
        sim.DebrisAdded += AddDebrisToList;
    }

    public void AddDebrisToList(ObjectController<DebrisData> controller)
    {
        if (uiDocument == null || uiDocument.rootVisualElement == null) return;

        if (scrollView == null)
        {
            scrollView = uiDocument.rootVisualElement.Q<ScrollView>("debris-scroll-view");
        }

        if (scrollView == null || rowTemplate == null) return;

        VisualElement row = rowTemplate.Instantiate();
        row.name = controller.ObjectData.Id; 
        row.Q<Label>("debris-name").text = controller.ObjectData.Name;
        
        row.RegisterCallback<ClickEvent>(evt => SelectRow(row, controller.ObjectData.Id));

        Button focusBtn = row.Q<Button>("focus-button");
        if (focusBtn != null)
        {
            focusBtn.RegisterCallback<ClickEvent>(evt => 
            {
                SelectRow(row, controller.ObjectData.Id, true);
                
                CameraManager.Instance.FollowDebris(controller.gameObject);
                
                evt.StopPropagation(); 
            });
        }

        scrollView.Add(row);

        SelectRow(row, controller.ObjectData.Id, true);

        // new : add catcher button
        if (_addCatcherButton != null) _addCatcherButton.SetEnabled(true);
        if (_searchField != null)
        {
            FilterList(_searchField.value);
        }
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

            // new : if no debris, setEnabled false
            if (scrollView.childCount == 0 && _addCatcherButton != null)
            {
                _addCatcherButton.SetEnabled(false);
            }
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

        // new : catcher controller
        _catcherCreationController = FindFirstObjectByType<CatcherCreationController>();

        _addDebrisButton = root.Q<Button>("createDebris");
        if (_addDebrisButton != null)
        {
            _addDebrisButton.clicked += OpenCreationWizard;
        }

        // new : open catcher screen
        _addCatcherButton = root.Q<Button>("createCatcher");
        if (_addCatcherButton != null)
        {
            _addCatcherButton.clicked += OpenCatcherWizard;
            
            // new : button catcher state
            RefreshAddCatcherButtonState();
        }

        _searchField = root.Q<TextField>("object-search-input");
        if (_searchField != null)
        {
            _searchField.RegisterValueChangedCallback(evt => FilterList(evt.newValue));
        }
        
        var searchBtn = root.Q<Button>("search-button");
        if (searchBtn != null)
        {
            searchBtn.clicked += () => FilterList(_searchField.value);
        }
    }

    public void UpdateCatcherInfo(string catcherName, string targetDebrisName)
    {
        if (_catcherTargetInfo == null) return;

        if (string.IsNullOrEmpty(catcherName))
        {
            _catcherTargetInfo.style.display = DisplayStyle.None;

            // new : button catcher state
            RefreshAddCatcherButtonState();
        }
        else
        {
            _catcherTargetInfo.style.display = DisplayStyle.Flex;
            
            if (_catcherNameLabel != null) _catcherNameLabel.text = catcherName;
            if (_debrisNameLabel != null) _debrisNameLabel.text = targetDebrisName;
            // new : button catcher state
            RefreshAddCatcherButtonState();
        }
    }

    private void OpenCreationWizard()
    {
        if (_creationController != null)
        {
            _creationController.ShowWizard();
        }
    }

    // new : open catcher wizard
    private void OpenCatcherWizard()
    {
        if (_catcherCreationController != null)
        {
            _catcherCreationController.ShowWizard();
        }
    }
    
    // new : button catcher state
    private void RefreshAddCatcherButtonState()
    {
        if (_addCatcherButton != null && scrollView != null)
        {
            _addCatcherButton.SetEnabled(scrollView.childCount > 0 && !SimulationManager.Instance.HasCatcher);
        }
    }

    // new : filters list of existing debris based on the user input
    private void FilterList(string searchTerm)
    {
        if (scrollView == null) return;

        string lowerSearch = searchTerm.ToLower().Trim();

        foreach (VisualElement row in scrollView.Children())
        {
            Label nameLabel = row.Q<Label>("debris-name");
            if (nameLabel != null)
            {
                bool matches = string.IsNullOrEmpty(lowerSearch) || 
                            nameLabel.text.ToLower().Contains(lowerSearch);
                
                row.style.display = matches ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }
    }
}
