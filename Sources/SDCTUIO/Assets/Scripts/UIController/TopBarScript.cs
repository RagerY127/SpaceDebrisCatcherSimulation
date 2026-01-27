using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var helpOverlay = root.Q<VisualElement>("HelpOverlay");

        var openBtn = root.Q<Button>("help");

        var closeBtn = root.Q<Button>("Close");

        if (openBtn != null)
            openBtn.clicked += () => helpOverlay.RemoveFromClassList("overlay-hidden");

        if (closeBtn != null)
            closeBtn.clicked += () => helpOverlay.AddToClassList("overlay-hidden");
    }
}
