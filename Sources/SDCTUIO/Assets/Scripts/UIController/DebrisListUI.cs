using UnityEngine;
using UnityEngine.UIElements;

public class DebrisListUI : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset rowTemplate;
    private ScrollView scrollView;
    private UIDocument uiDocument;

    private VisualElement _currentlySelectedRow;
    private string _selectedDebrisId;
    private Button _deleteButton;

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
    }


}