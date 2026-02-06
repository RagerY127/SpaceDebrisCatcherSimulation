using UnityEngine;
using UnityEngine.UIElements;

public class AnneauController : MonoBehaviour
{
    public static AnneauController Instance { get; private set; }

    [Header("UI settings")]
    public UIDocument uiDocument;
    public Vector2 menuOffset = new Vector2(-165f, -135f);
    public float selectionDistance = 80f;
    public float hoverDurationToActivate = 1.0f;

    private DebrisController _targetDebris;
    private VisualElement _root;
    private VisualElement _menuContainer;
    
    private VisualElement _selectionLayer;
    private VisualElement _valLayerDelete;
    private VisualElement _valLayerHolo;
    private VisualElement _valLayerFocus;
    private VisualElement _activeValLayer;

    private VisualElement _curHoverBtn;
    private VisualElement _pendingOpBtn;

    private float _hoverTimer;
    private bool _isOpen;
    
    /*
        State:

        Sel: Selection, When user begins a select a bouton
        Val: Validation, After the selection user need to validate this operation
    */
    private enum State 
    { 
        Sel,
        Val
    }
    private State _state = State.Sel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Instance = null;
        }
    }

    void OnEnable()
    {
        _root = uiDocument.rootVisualElement;

        _menuContainer = _root.Q("RadialMenu");
        _selectionLayer = _root.Q("SelectionLayer");
        _valLayerDelete = _root.Q("ValidationLayer");
        _valLayerHolo = _root.Q("ValidationLayerHolo");
        _valLayerFocus = _root.Q("ValidationLayerFocus");
        
        HideMenu();
    }

    void Update()
    {
        if (!_isOpen || !_targetDebris || _menuContainer == null) 
        {
            return;
        }

        Vector2 wp = RuntimePanelUtils.CameraTransformWorldToPanel(
            _menuContainer.panel, 
            _targetDebris.transform.position, 
            Camera.main
        );
        
        _menuContainer.style.left = wp.x + menuOffset.x; 
        _menuContainer.style.top = wp.y + menuOffset.y;

        // The states of finger or mouse
        bool isTouch = Input.touchCount > 0;
        bool isMouseRelease = Input.GetMouseButtonUp(0);
        bool isTouchRelease = isTouch && Input.GetTouch(0).phase == TouchPhase.Ended;
        
        // Is released 
        bool isReleased = isTouchRelease || isMouseRelease;
        // Is pressed finger OR mouse
        bool isPressing = isTouch || Input.GetMouseButton(0);


        // Get position
        Vector2 screenPos;
        if (isTouch)
        {
            screenPos = Input.GetTouch(0).position;
        }
        else
        {
            screenPos = Input.mousePosition;
        }

        Vector2 panelPos = RuntimePanelUtils.ScreenToPanel(
            _menuContainer.panel, 
            new Vector2(screenPos.x, Screen.height - screenPos.y)
        );

        if (!isPressing && isReleased) 
        {
            if (_state == State.Val) 
            {
                // Execute after the validation of the button selected
                ExecuteIfConfirmed(panelPos); 
            }
            else 
            {
                HideMenu();
            }
            return;
        }

        if (_state == State.Sel) 
        {
            HandleSel(panelPos); 
        }
        else 
        {
            HandleVal(panelPos);
        }
    }

    void HandleSel(Vector2 p)
    {
        var btn = FindBtn(_selectionLayer, p, "btnDelete", "btnHolo", "btnFocas");
        
        UpdateHover(btn);

        if (btn != null && btn == _curHoverBtn)
        {
            _hoverTimer += Time.deltaTime;
            
            if (_hoverTimer >= hoverDurationToActivate) 
            {
                ToValMode(btn);
            }
        } 
        else 
        {
            _hoverTimer = 0f;
        }
    }

    void HandleVal(Vector2 p)
    {
        var btn = FindBtn(_activeValLayer, p, "btnConfirm", "btnCancel");
        UpdateHover(btn);
    }

    void ToValMode(VisualElement op)
    {
        _pendingOpBtn = op; 
        _state = State.Val; 
        _hoverTimer = 0f;

        if (_selectionLayer != null) 
        {
            _selectionLayer.style.display = DisplayStyle.None;
        }

        switch (op.name)
        {
            case "btnDelete":
                _activeValLayer = _valLayerDelete;
                break;
            case "btnHolo":
                _activeValLayer = _valLayerHolo;
                break;
            case "btnFocas":
                _activeValLayer = _valLayerFocus;
                break;
            default:
                _activeValLayer = null;
                break;
        }

        if (_activeValLayer != null) 
        {
            _activeValLayer.style.display = DisplayStyle.Flex;
        }
    }

    /*
        Operations of these buttons
    */
    void ExecuteIfConfirmed(Vector2 p)
    {
        var btn = FindBtn(_activeValLayer, p, "btnConfirm", "btnCancel");
        
        if (btn != null && btn.name == "btnConfirm" && _pendingOpBtn != null) 
        {
            if (_pendingOpBtn.name == "btnDelete")
            { 
                SimulationManager.Instance.RemoveDebris(_targetDebris.DebrisData.Id);

            } else if (_pendingOpBtn.name == "btnHolo") 
            { 
                /* DebrisManager.Instance.Remove(_targetDebris); */ 
                
            } else if (_pendingOpBtn.name == "btnFocas") 
            { 
                SimulationManager.Instance.SelectDebris(_targetDebris.DebrisData.Id);
                CameraManager.Instance.FollowDebris(_targetDebris.gameObject);

            }
        }
        
        HideMenu();
    }

    VisualElement FindBtn(VisualElement parent, Vector2 pos, params string[] names)
    {
        if (parent == null) return null;
        
        VisualElement result = null; 
        float minDistance = float.MaxValue;

        foreach (var name in names) 
        {
            var element = parent.Q(name); 
            if (element == null) continue;

            float dist = Vector2.Distance(pos, element.worldBound.center);
            
            if (dist < selectionDistance && dist < minDistance) 
            { 
                minDistance = dist; 
                result = element; 
            }
        }
        return result;
    }

    void UpdateHover(VisualElement btn)
    {
        _curHoverBtn = btn; 

        var container = _state == State.Sel ? _selectionLayer : _activeValLayer;
        if (container == null) return;

        foreach (var child in container.Children())
        {
            bool isTarget = (child == btn);
            child.style.opacity = (btn == null || isTarget) ? 1f : 0.5f;
            child.style.scale = isTarget ? Vector3.one * 1.2f : Vector3.one;
        }
    }

    public void OpenMenuForDebris(DebrisController d)
    {
        _targetDebris = d; 
        _isOpen = true; 
        _state = State.Sel; 
        _hoverTimer = 0; 
        _pendingOpBtn = null; 

        _curHoverBtn = new VisualElement(); 

        if (_menuContainer == null) return;

        _menuContainer.style.display = DisplayStyle.Flex;
        if (_selectionLayer != null) _selectionLayer.style.display = DisplayStyle.Flex;
        
        if (_valLayerDelete != null) _valLayerDelete.style.display = DisplayStyle.None;
        if (_valLayerHolo != null)   _valLayerHolo.style.display = DisplayStyle.None;
        if (_valLayerFocus != null)  _valLayerFocus.style.display = DisplayStyle.None;

        UpdateHover(null);
    }

    public void HideMenu() 
    { 
        _isOpen = false; 
        _targetDebris = null; 
        
        if (_menuContainer != null) 
        {
            _menuContainer.style.display = DisplayStyle.None;
        }
    }
}