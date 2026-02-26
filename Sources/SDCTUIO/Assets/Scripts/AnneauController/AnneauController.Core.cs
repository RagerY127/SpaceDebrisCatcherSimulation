using UnityEngine;
using UnityEngine.UIElements;

// Notice the 'partial' keyword. This tells the compiler this is part of the AnneauController class.
public partial class AnneauController : MonoBehaviour
{
    // Singleton Instance
    public static AnneauController Instance { get; private set; }

    [Header("UI settings")] 
    public UIDocument uiDocument;
    
    // Correction of the offset (Not used for now)
    public Vector2 centerCorrection = Vector2.zero; 
    public float selectionDistance = 80f;
    public float dragThreshold = 20f; 
    
    // Input state
    private Vector2 _pointerDownPos;

    // Targets
    private DebrisController _targetDebris;
    private CatcherController _targetCatcher;

    // Interaction State
    private bool _isOpen;
    private float _openTime;

    /*
        State:
        Sel: Selection, When user begins to select a button
        Val: Validation, After the selection user needs to validate this operation
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

    void Update()
    {
        // 1. Check if targets exist and menu is ready
        bool hasTarget = _targetDebris != null || _targetCatcher != null;

        if (!_isOpen || !hasTarget || _menuContainer == null) 
        {
            return;
        }

        // 2. Calculate target world position
        Vector3 targetWorldPos = Vector3.zero;
        Vector2 currentScreenCorrection = centerCorrection; 

        // 3. Handle inputs (Mouse or Touch)
        Vector2 currentScreenPos;
        if (Input.touchCount > 0) currentScreenPos = Input.GetTouch(0).position;
        else currentScreenPos = Input.mousePosition;

        Vector2 mousePanelPos = RuntimePanelUtils.ScreenToPanel(
                _menuContainer.panel, 
                new Vector2(currentScreenPos.x, Screen.height - currentScreenPos.y)
        );
        
        // Record pointer down position to detect dragging
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            _pointerDownPos = currentScreenPos;
        }

        // 4. Assign target position based on the selected object type
        if (_targetDebris != null) 
        {
            targetWorldPos = _targetDebris.transform.position;
        }
        else if (_targetCatcher != null) 
        {
            Transform bodyTransform = _targetCatcher.transform.Find("CatcherBody");
            targetWorldPos = bodyTransform.position;
        }

        // 5. Convert world position to UI panel coordinates
        Vector2 panelPos = RuntimePanelUtils.CameraTransformWorldToPanel(
            _menuContainer.panel, 
            targetWorldPos, 
            Camera.main
        );

        float width = _menuContainer.layout.width;
        float height = _menuContainer.layout.height;

        if (float.IsNaN(width) || width == 0) width = 300; 
        if (float.IsNaN(height) || height == 0) height = 300;

        // Apply position to the menu container
        _menuContainer.style.left = panelPos.x - (width / 2f) + currentScreenCorrection.x;
        _menuContainer.style.top = panelPos.y - (height / 2f) + currentScreenCorrection.y;

        // 6. Interaction logic detection
        bool isReleased = Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);

        if (isReleased) 
        {
            float dragDist = Vector2.Distance(_pointerDownPos, currentScreenPos);

            // Ignore action if it was a drag (e.g., rotating the camera)
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
                    // Close menu if user clicks outside after opening
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

        // 7. Process visual hover effects continuously
        if (_state == State.Sel) 
        {
            HandleSel(mousePanelPos); 
        }
        else 
        {
            HandleVal(mousePanelPos);
        }
    }

    // --- Public API for opening the menu ---

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
}