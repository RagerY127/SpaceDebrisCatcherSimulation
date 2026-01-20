using System;
using UnityEngine;
using UnityEngine.UIElements;

public class SimulationBarController : MonoBehaviour
{
    private Button _playPauseButton;
    private Button _speedDownButton;
    private Button _speedUpButton;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _playPauseButton = root.Q<Button>("start_stop");
        _speedDownButton = root.Q<Button>("reverse");
        _speedUpButton = root.Q<Button>("forward");

        if (_playPauseButton != null)
        {
            _playPauseButton.AddToClassList("icon-pause");
            
            _playPauseButton.clicked += OnToggleSimulation;
        }

        if (_speedDownButton != null)
        {
            _speedDownButton.clicked += OnSpeedDown;
        }

        if (_speedUpButton != null)
        {
            _speedUpButton.clicked += OnSpeedUp;
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

    private void OnSpeedDown()
    {
        print("Slow down simulation");
    }

    private void OnSpeedUp()
    {
        print("Speed up simulation");
    }
}