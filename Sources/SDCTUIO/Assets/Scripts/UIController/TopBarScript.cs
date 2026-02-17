using UnityEngine;
using UnityEngine.UIElements;
using TouchScript; // Wichtig!

[RequireComponent(typeof(UIDocument))]
public class UIController : MonoBehaviour
{
    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var helpOverlay = root.Q<VisualElement>("HelpOverlay");
        var openBtn = root.Q<Button>("help");
        var closeBtn = root.Q<Button>("Close");

        var tm = TouchManager.Instance as Behaviour;

        if (openBtn != null)
        {
            openBtn.clicked += () => {
                helpOverlay.RemoveFromClassList("overlay-hidden");
                if (tm != null) tm.enabled = false;
            };
        }

        if (closeBtn != null)
        {
            closeBtn.clicked += () => {
                helpOverlay.AddToClassList("overlay-hidden");
                if (tm != null) tm.enabled = true;
            };
        }
    }
}