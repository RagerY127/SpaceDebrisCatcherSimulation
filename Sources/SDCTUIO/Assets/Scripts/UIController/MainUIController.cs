using UnityEngine;
using UnityEngine.UIElements;

public class MainUIController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        root.Query<TextField>().ForEach(field => {
            field.RegisterCallback<PointerUpEvent>(e => {
                if (e.pointerType == "touch") OnScreenKeyboard.ShowTouchKeyboard(); });
        });
        root.Query<TextElement>().ForEach(field => {
            field.RegisterCallback<PointerUpEvent>(e => {
                if (e.pointerType == "touch") OnScreenKeyboard.ShowTouchKeyboard(); });
        });
    }
}
