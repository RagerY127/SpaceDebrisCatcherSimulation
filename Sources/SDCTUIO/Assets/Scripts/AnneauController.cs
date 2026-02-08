using UnityEngine;
using UnityEngine.UIElements;

public class AnneauController : MonoBehaviour
{
    //Instance
    public static AnneauController Instance { get; private set; }

    // The relevent position of anneau, could be adjust in Unity
    [Header("UI settings")]
    public UIDocument uiDocument;
    public Vector2 menuOffset = new Vector2(-165f, -135f);
    public float selectionDistance = 80f;

    // This debris
    private DebrisController _targetDebris;

    // This catcher
    private CatcherController _targetCatcher;

    // UI Elements
    private VisualElement _root;
    private VisualElement _menuContainer;
    
    private VisualElement _selectionLayer;
    private VisualElement _valLayerDelete;
    private VisualElement _valLayerHolo;
    private VisualElement _valLayerFocus;
    private VisualElement _activeValLayer;

    private VisualElement _curHoverBtn;
    private VisualElement _pendingOpBtn;

    //private float _hoverTimer;
    private bool _isOpen;

    private float _openTime;

    
    /// <summary>
    /// !!!!!!!!!!!!!!!!!!!!!!Operations of these buttons!!!!!!!!!!!!!!!!!!!!
    /// !!!!!!!!!!!!!!!!!!!!!!ADD LOGIC OF BUTTONS ON THE ANNEAU HERE!!!!!!!!
    /// </summary>
    /// <param name="p"></param>
    void ExecuteIfConfirmed(Vector2 p)
    {
        var btn = FindBtn(_activeValLayer, p, "btnConfirm", "btnCancel");
        
        if (btn != null && btn.name == "btnConfirm" && _pendingOpBtn != null) 
        {
            // DELETE object button
            if (_pendingOpBtn.name == "btnDelete")
            {
                if (_targetDebris != null)
                {
                    HololensMessage.SendDebrisMessage(MessageCommand.DELETE, _targetDebris.DebrisData);
                    SimulationManager.Instance.RemoveDebris(_targetDebris.DebrisData.Id);
                }
                else if (_targetCatcher != null)
                {
                    //Destroy(_targetCatcher.gameObject); 
                }

            } 
            // SEND TO HOLOLENS button
            else if (_pendingOpBtn.name == "btnHolo") 
            {
                if (_targetDebris != null)
                {
                    // La logique pour envoyer debris et catcher, on envoye un d'entre les deux est-ce qu'on voit aussi l'autre dans Hololens?
                    HololensMessage.SendDebrisMessage(MessageCommand.SPAWN, _targetDebris.DebrisData);
                    Debug.Log("11111");
                }
                else if (_targetCatcher != null)
                {
                    HololensMessage.SendCatcherMessage(MessageCommand.SPAWN, _targetCatcher.CatcherData);
                }
            } 
            // FOCUS button
            else if (_pendingOpBtn.name == "btnFocas") 
            { 
                if (_targetDebris != null)
                {
                    SimulationManager.Instance.SelectDebris(_targetDebris.DebrisData.Id);
                    CameraManager.Instance.FollowDebris(_targetDebris.gameObject);
                }
                else if (_targetCatcher != null)
                {
                    //CameraManager.Instance.FollowCatcher(_targetCatcher.??);
                    //CameraManager.Instance.FollowDebris(_targetCatcher.gameObject);
                }
            }
        }
        
        HideMenu();
    }
    
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

        // If chose catcher or debris
        bool hasTarget = _targetDebris != null || _targetCatcher != null;

        if (!_isOpen || !hasTarget || _menuContainer == null) 
        {
            return;
        }

        // Depends on if we choose DEBRIS or CATCHER
        Vector3 targetPosition = Vector3.zero;
        if (_targetDebris != null) 
            targetPosition = _targetDebris.transform.position;
        else if (_targetCatcher != null) 
            targetPosition = _targetCatcher.transform.position;

        // Position follow logic
        Vector2 wp = RuntimePanelUtils.CameraTransformWorldToPanel(
            _menuContainer.panel, 
            targetPosition, 
            Camera.main
        );
        
        _menuContainer.style.left = wp.x + menuOffset.x; 
        _menuContainer.style.top = wp.y + menuOffset.y;

        // Input detection
        bool isTouch = Input.touchCount > 0;
        bool isMouseRelease = Input.GetMouseButtonUp(0);
        bool isTouchRelease = isTouch && Input.GetTouch(0).phase == TouchPhase.Ended;
        
        bool isReleased = isTouchRelease || isMouseRelease;
        bool isPressing = isTouch || Input.GetMouseButton(0);

        Vector2 screenPos;
        if (isTouch) screenPos = Input.GetTouch(0).position;
        else screenPos = Input.mousePosition;

        Vector2 panelPos = RuntimePanelUtils.ScreenToPanel(
            _menuContainer.panel, 
            new Vector2(screenPos.x, Screen.height - screenPos.y)
        );

        // Interaction logic
        if (!isPressing && isReleased) 
        {
            if (_state == State.Sel) 
            {
                var btn = FindBtn(_selectionLayer, panelPos, "btnDelete", "btnHolo", "btnFocas");
                
                if (btn != null) 
                {
                    ToValMode(btn);
                }
                else
                {
                    if (Time.time - _openTime > 0.5f)
                    {
                        HideMenu();
                    }
                }
            }
            else if (_state == State.Val) 
            {
                ExecuteIfConfirmed(panelPos); 
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

    /// <summary>
    /// Handling hover in selection mode
    /// </summary>
    /// <param name="p"></param>
    void HandleSel(Vector2 p)
    {
        var btn = FindBtn(_selectionLayer, p, "btnDelete", "btnHolo", "btnFocas");
        
        UpdateHover(btn);
    }

    /// <summary>
    /// Hover in confirmation mode
    /// </summary>
    /// <param name="p"></param>
    void HandleVal(Vector2 p)
    {
        var btn = FindBtn(_activeValLayer, p, "btnConfirm", "btnCancel");
        UpdateHover(btn);
    }

    /// <summary>
    /// Switch to Validation Mode
    /// </summary>
    /// <param name="op"></param>
    void ToValMode(VisualElement op)
    {
        _pendingOpBtn = op; 
        _state = State.Val; 
        //_hoverTimer = 0f;

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

    /// <summary>
    /// Helper method: Find the button closest to the mouse cursor in the parent container.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="pos"></param>
    /// <param name="names"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Hover visual effect of the update button
    /// </summary>
    /// <param name="btn"></param>
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
        _targetCatcher = null;
        OpenMenuInternal();
    }

    public void OpenMenuForCatcher(CatcherController c)
    {
        _targetCatcher = c;
        _targetDebris = null;
        OpenMenuInternal();
    }
    private void OpenMenuInternal()
    {
        _isOpen = true; 
        _openTime = Time.time;
        _state = State.Sel; 
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

    /// <summary>
    /// Hide and reset the menu
    /// </summary>
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