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
        //var openBtn = root.Q<Button>("help");
        var closeBtn = root.Q<Button>("Close");
        var openBtn = root.Q<Button>("open");

        var tm = TouchManager.Instance as Behaviour;

        /*if (openBtn != null)
        {
            openBtn.clicked += () => {
                helpOverlay.RemoveFromClassList("overlay-hidden");
                if (tm != null) tm.enabled = false;
            };
        }*/

        if (closeBtn != null)
        {
            closeBtn.clicked += () => {
                helpOverlay.AddToClassList("overlay-hidden");
                if (tm != null) tm.enabled = true;
            };
        }

        if (openBtn != null)
        {
            openBtn.clicked += () => {
                if (tm != null) tm.enabled = false;

                string selectedPath = WindowsFilePicker.ShowDialog("Select a TLE text file", "Text Files\0*.txt\0All Files\0*.*\0");

                if (!string.IsNullOrEmpty(selectedPath))
                {
                    TLEManager.Instance.LoadTLEDataFromExternalFile(selectedPath);
                }

                if (tm != null) tm.enabled = true;
            };
        }
    }
}