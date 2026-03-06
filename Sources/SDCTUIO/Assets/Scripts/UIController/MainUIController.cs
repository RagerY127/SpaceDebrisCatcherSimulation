using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MainUIController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var textFields = root.Query<TextField>().ToList();
        var integerFields = root.Query<IntegerField>().ToList();
        var floatFields = root.Query<FloatField>().ToList();

        textFields.ForEach(field => RegisterFieldForTouchKeyboard(field));
        integerFields.ForEach(field => RegisterFieldForTouchKeyboard(field));
        floatFields.ForEach(field => RegisterFieldForTouchKeyboard(field));

        // kill keyboard on enter (return) for any element
        root.Query<TextElement>().ForEach(field =>
        {
            field.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Return)
                {
                    OnScreenKeyboard.KillTouchKeyboard();
                }
            });
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

    private void SpawnTouchKeyboard(PointerUpEvent e)
    {
        if (e.pointerType == "touch") // touch
        {
            OnScreenKeyboard.ShowTouchKeyboard();
        }
    }

    private void RegisterFieldForTouchKeyboard(VisualElement field)
    {
        field.RegisterCallback<PointerUpEvent>(SpawnTouchKeyboard);
        field.Query<TextElement>().ForEach(elem => {
            elem.RegisterCallback<PointerUpEvent>(SpawnTouchKeyboard);
        });
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
