using System;
using UnityEngine;
using UnityEngine.UIElements;

public class SimulationBarController : MonoBehaviour
{
    private Button _playPauseButton;
    private Button _speedDownButton;
    private Button _speedUpButton;
    private Label _speedLabel;

    private readonly float[] _speedSteps = { 1f, 10f, 100f, 500f, 1000f };
    private int _currentSpeedIndex = 0;
    

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _playPauseButton = root.Q<Button>("start_stop");
        _speedDownButton = root.Q<Button>("reverse");
        _speedUpButton = root.Q<Button>("forward");
        _speedLabel = root.Q<Label>("speed");

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
        ApplySpeed();
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
        if (_currentSpeedIndex > 0)
        {
            _currentSpeedIndex--;
            ApplySpeed();
        }
        print("Slow down simulation");
    }

    private void OnSpeedUp()
    {
        if (_currentSpeedIndex < _speedSteps.Length - 1)
        {
            _currentSpeedIndex++;
            ApplySpeed();
        }
        print("Speed up simulation");
    }

    private void ApplySpeed()
    {
        float newSpeed = _speedSteps[_currentSpeedIndex];
        SimulationManager.Instance.SetSimulationSpeed(newSpeed);
        
        if (_speedLabel != null)
        {
            _speedLabel.text = _speedSteps[_currentSpeedIndex].ToString() + "x";
        }

        if (_currentSpeedIndex == _speedSteps.Length -1 )
        {
            _speedUpButton?.AddToClassList("button--disabled");
            if (_speedUpButton != null) _speedUpButton.pickingMode = PickingMode.Ignore;
        }
        else if (_currentSpeedIndex == 0)
        {
            _speedDownButton?.AddToClassList("button--disabled");
            if (_speedDownButton != null) _speedDownButton.pickingMode = PickingMode.Ignore;
        }
        else
        {
            _speedDownButton?.RemoveFromClassList("button--disabled");
            if (_speedDownButton != null) _speedDownButton.pickingMode = PickingMode.Position;
            _speedUpButton?.RemoveFromClassList("button--disabled");
            if (_speedUpButton != null) _speedUpButton.pickingMode = PickingMode.Position;
        }
    }
}