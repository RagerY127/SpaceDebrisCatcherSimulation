using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "DebrisScriptableObject", menuName = "Scriptable Objects/DebrisScriptableObject")]
public class DebrisScriptableObject : ScriptableObject
{
    public string debrisName = "";
    public int orbitFirstAxis = 0;
    public int orbitSecondAxis = 0;
    public int initialPosition = 0;
    public float revolutionsPerDay = 0.0f;
    public float mass = 0.0f;
    public float height = 0.0f;
    public float length = 0.0f;
    public float width = 0.0f;
    public int shapeEnumIndex = 0;

    // Propriétés string pour labels de CreationUI
    [SerializeField]
    public string orbitFirstAxisString {
        get => orbitFirstAxis.ToString() + "°";
        set => orbitFirstAxis = int.Parse(value);
    }
    [SerializeField]
    public string orbitSecondAxisString {
        get => orbitSecondAxis.ToString() + "°";
        set => orbitSecondAxis = int.Parse(value);
    }
    [SerializeField]
    public string initialPositionString
    {
        get => initialPosition.ToString() + "°";
        set => initialPosition = int.Parse(value);
    }
    [SerializeField]
    public List<string> shapeEnumList => Enum.GetNames(typeof(DebrisShape)).ToList();
    public DebrisShape shape => (DebrisShape)Enum.Parse(typeof(DebrisShape), shapeEnumList[shapeEnumIndex]);
}
