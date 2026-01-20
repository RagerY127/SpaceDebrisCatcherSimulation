using UnityEngine;
using UnityEngine.UIElements;

public class SimulationBarController : MonoBehaviour
{
    private Button _playPauseButton;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _playPauseButton = root.Q<Button>("start_stop");

        if (_playPauseButton != null)
        {
            _playPauseButton.AddToClassList("icon-pause");
            
            _playPauseButton.clicked += OnToggleSimulation;
        }
    }

    private void OnToggleSimulation()
    {
        if (SimulationManager.Instance.IsSimulationRunning)
        {
            _playPauseButton.RemoveFromClassList("icon-pause");
            _playPauseButton.AddToClassList("icon-play");

            SimulationManager.Instance.StopSimulation();
        }
        else
        {
            _playPauseButton.RemoveFromClassList("icon-play");
            _playPauseButton.AddToClassList("icon-pause");

            SimulationManager.Instance.ResumeSimulation();
        }
    }
}