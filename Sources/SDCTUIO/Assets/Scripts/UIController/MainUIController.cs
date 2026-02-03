using UnityEngine;
using UnityEngine.UIElements;

public class MainUIController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        root.Query<TextField>().ForEach(field =>
        {
           field.RegisterCallback<FocusEvent>(e => { OnScreenKeyboard.ShowTouchKeyboard(); });
           field.RegisterCallback<ClickEvent>(e => { OnScreenKeyboard.ShowTouchKeyboard(); });
        });
    }
}
