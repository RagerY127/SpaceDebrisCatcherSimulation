using UnityEngine;
using UnityEngine.UIElements;


// Notice the 'partial' keyword. This tells the compiler this is part of the AnneauController class.
public partial class AnneauController 
{
    // UI Elements
    private VisualElement _root;
    private VisualElement _menuContainer;
    private VisualElement _selectionLayer;
    
    // Validation Layers
    private VisualElement _valLayerDelete;
    private VisualElement _valLayerHolo;
    private VisualElement _valLayerFocus;
    private VisualElement _activeValLayer;

    // Visual State tracking
    private VisualElement _curHoverBtn;
    private VisualElement _pendingOpBtn;

    void OnEnable()
    {
        if (uiDocument == null) return;
        _root = uiDocument.rootVisualElement;

        // Query all required UI elements
        _menuContainer = _root.Q("RadialMenu");
        _selectionLayer = _root.Q("SelectionLayer");
        _valLayerDelete = _root.Q("ValidationLayer");
        _valLayerHolo = _root.Q("ValidationLayerHolo");
        _valLayerFocus = _root.Q("ValidationLayerFocus");
        
        HideMenu();
    }

    /// <summary>
    /// Internal method to initialize and show the menu
    /// </summary>
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
    /// Hide and reset the menu visual states
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

    /// <summary>
    /// Handling hover visual effects in selection mode
    /// </summary>
    private void HandleSel(Vector2 p)
    {
        var btn = FindBtn(_selectionLayer, p, "btnDelete", "btnHolo", "btnFocas");
        UpdateHover(btn);
    }

    /// <summary>
    /// Handling hover visual effects in confirmation mode
    /// </summary>
    private void HandleVal(Vector2 p)
    {
        var btn = FindBtn(_activeValLayer, p, "btnConfirm", "btnCancel");
        UpdateHover(btn);
    }

    /// <summary>
    /// Switch UI to Validation Mode and display the specific confirmation layer
    /// </summary>
    private void ToValMode(VisualElement op)
    {
        _pendingOpBtn = op; 
        _state = State.Val; 

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
    private VisualElement FindBtn(VisualElement parent, Vector2 pos, params string[] names)
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
    /// Hover visual effect of the button (Opacity and Scale)
    /// </summary>
    private void UpdateHover(VisualElement btn)
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
}