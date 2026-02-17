using UnityEngine;
using UnityEngine.UIElements;

public class AnneauController : MonoBehaviour
{
    // Instance
    public static AnneauController Instance { get; private set; }

    [Header("UI settings")] 
    public UIDocument uiDocument;
    
    // Corrrection of the offset (No used for now)
    public Vector2 centerCorrection = Vector2.zero; 
    
    public float selectionDistance = 80f;

    public float dragThreshold = 20f; 
    
    private Vector2 _pointerDownPos;

    // Targets
    private DebrisController _targetDebris;
    private CatcherController _targetCatcher;

    // UI Elements
    private VisualElement _root;
    private VisualElement _menuContainer;
    private VisualElement _selectionLayer;
    
    // Validation Layers
    private VisualElement _valLayerDelete;
    private VisualElement _valLayerHolo;
    private VisualElement _valLayerFocus;
    private VisualElement _activeValLayer;

    // State tracking
    private VisualElement _curHoverBtn;
    private VisualElement _pendingOpBtn;
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
                    //HololensMessage.SendDebrisMessage(MessageCommand.DELETE, _targetDebris.DebrisData);
                    SimulationManager.Instance.RemoveDebris(_targetDebris.ObjectData.Id);
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
                    HololensMessage.SendDebrisMessage(MessageCommand.SPAWN, _targetDebris.ObjectData);
                }
                else if (_targetCatcher != null)
                {
                    MessageCommand cmd = _targetCatcher.HasBeenSpawned ? MessageCommand.UPDATE : MessageCommand.SPAWN;
                    
                    HololensMessage.SendCatcherMessage(
                        cmd, 
                        _targetCatcher.ObjectData,
                        _targetCatcher.CurrentProgressSeconds);

                    _targetCatcher.HasBeenSpawned = true;
                }
            } 
            // FOCUS button
            else if (_pendingOpBtn.name == "btnFocas") 
            { 
                if (_targetDebris != null)
                {
                    SimulationManager.Instance.SelectDebris(_targetDebris.ObjectData.Id);
                    CameraManager.Instance.FollowDebris(_targetDebris.gameObject);
                }
                else if (_targetCatcher != null)
                {
                    //CameraManager.Instance.FollowCatcher(_targetCatcher.??);
                    SimulationManager.Instance.SelectCatcher(this._targetCatcher.ObjectData);
                    CameraManager.Instance.FollowDebris(_targetCatcher.gameObject);
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
        if (uiDocument == null) return;
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


        Vector3 targetWorldPos = Vector3.zero;
        Vector2 currentScreenCorrection = centerCorrection; 

        // Control slide on the screen
        Vector2 currentScreenPos;
        if (Input.touchCount > 0) currentScreenPos = Input.GetTouch(0).position;
        else currentScreenPos = Input.mousePosition;

        Vector2 mousePanelPos = RuntimePanelUtils.ScreenToPanel(
                _menuContainer.panel, 
                new Vector2(currentScreenPos.x, Screen.height - currentScreenPos.y)
        );
        
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            _pointerDownPos = currentScreenPos;
        }
        // 

        if (_targetDebris != null) 
        {
            targetWorldPos = _targetDebris.transform.position;
        }
        else if (_targetCatcher != null) 
        {
            Transform bodyTransform = _targetCatcher.transform.Find("CatcherBody");
            targetWorldPos = bodyTransform.position;
        }

        Vector2 panelPos = RuntimePanelUtils.CameraTransformWorldToPanel(
            _menuContainer.panel, 
            targetWorldPos, 
            Camera.main
        );

        float width = _menuContainer.layout.width;
        float height = _menuContainer.layout.height;

        if (float.IsNaN(width) || width == 0) width = 300; 
        if (float.IsNaN(height) || height == 0) height = 300;

        _menuContainer.style.left = panelPos.x - (width / 2f) + currentScreenCorrection.x;
        _menuContainer.style.top = panelPos.y - (height / 2f) + currentScreenCorrection.y;

        // Input detection
        bool isTouch = Input.touchCount > 0;
        bool isMouseRelease = Input.GetMouseButtonUp(0);
        bool isTouchRelease = isTouch && Input.GetTouch(0).phase == TouchPhase.Ended;
        
        bool isReleased = isTouchRelease || isMouseRelease;
        bool isPressing = isTouch || Input.GetMouseButton(0);


        // Interaction logic
        if (isReleased) 
        {
            float dragDist = Vector2.Distance(_pointerDownPos, currentScreenPos);

            if (dragDist > dragThreshold)
            {
                return; 
            }
            if (_state == State.Sel) 
            {
                var btn = FindBtn(_selectionLayer, mousePanelPos, "btnDelete", "btnHolo", "btnFocas");
                
                if (btn != null) 
                {
                    ToValMode(btn);
                }
                else
                {
                    if (Time.time - _openTime > 1f)
                    {
                        HideMenu();
                    }
                }
            }
            else if (_state == State.Val) 
            {
                ExecuteIfConfirmed(mousePanelPos); 
            }
            return;
        }

        if (_state == State.Sel) 
        {
            HandleSel(mousePanelPos); 
        }
        else 
        {
            HandleVal(mousePanelPos);
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