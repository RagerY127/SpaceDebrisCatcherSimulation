using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class SimulationStateController : MonoBehaviour
{
    private Button _playPauseButton;
    private bool _isPlaying = false;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _playPauseButton = root.Q<Button>("start_stop");

        if (_playPauseButton != null)
        {
            _playPauseButton.AddToClassList("icon-play");
            
            _playPauseButton.clicked += OnToggleSimulation;
        }
    }

    private void OnToggleSimulation()
    {
        _isPlaying = !_isPlaying;

        if (_isPlaying)
        {
            _playPauseButton.RemoveFromClassList("icon-play");
            _playPauseButton.AddToClassList("icon-pause");
            Time.timeScale = 1f;
        }
        else
        {
            _playPauseButton.RemoveFromClassList("icon-pause");
            _playPauseButton.AddToClassList("icon-play");
            Time.timeScale = 0f;
        }

        Debug.Log(_isPlaying ? "Simulation lancée" : "Simulation pausée");
    }
}