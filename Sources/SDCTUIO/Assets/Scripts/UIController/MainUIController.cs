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

    void Update()
    {
        // DEBUG: test if debris creation ui pops (to remove)
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            SpawnDebrisCreationModal();
        }
    }

    private void SpawnDebrisCreationModal()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var wizard = root.Q<VisualElement>("DebrisCreationWizard");
        var modals = root.Q<VisualElement>("Modals");
        wizard.visible = true;
        modals.visible = true;
    }
}
