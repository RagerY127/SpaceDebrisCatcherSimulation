using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "DebrisScriptableObject", menuName = "Scriptable Objects/DebrisScriptableObject")]
public class DebrisScriptableObject : ScriptableObject
{
    private const string INITIAL_DEBRIS_NAME = "New debris";
    private const float INITIAL_DEBRIS_DISTANCE_FROM_EARTH_KM = 200f;
    private const float INITIAL_DEBRIS_MASS_KG = 1f;
    private const float INITIAL_DEBRIS_DIMENSION_M = 1f;

    public float MIN_DEBRIS_DISTANCE_FROM_EARTH_KM = 160f;
    public float MAX_DEBRIS_DISTANCE_FROM_EARTH_KM = 2000f;
    public float MIN_MASS_KG = 0.1f;
    public float MAX_MASS_KG = 1000f;
    public float MIN_DIMENSION_M = 0.1f;
    public float MAX_DIMENSION_M = 10f;

    public string debrisName = INITIAL_DEBRIS_NAME;
    public int orbitFirstAxis = 0;
    public int orbitSecondAxis = 0;
    public int initialPosition = 0;
    public float distanceFromEarthKm = INITIAL_DEBRIS_DISTANCE_FROM_EARTH_KM;
    public float mass = INITIAL_DEBRIS_MASS_KG;
    public float height = INITIAL_DEBRIS_DIMENSION_M;
    public float length = INITIAL_DEBRIS_DIMENSION_M;
    public float width = INITIAL_DEBRIS_DIMENSION_M;
    public int shapeEnumIndex = 0;

    // Propriétés string pour labels de CreationUI
    [SerializeField]
    public string distanceFromEarthLabel {
        get => $"Distance from Earth (from {MIN_DEBRIS_DISTANCE_FROM_EARTH_KM}km to {MAX_DEBRIS_DISTANCE_FROM_EARTH_KM}km)";
    }

    [SerializeField]
    public string massLabel
    {
        get => $"Mass (from {MIN_MASS_KG}kg to {MAX_MASS_KG}kg)";
    }

    [SerializeField]
    public string dimensionLabel
    {
        get => $"Dimensions (from {MIN_DIMENSION_M}m to {MAX_DIMENSION_M}m)";
    }

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

    public void ResetData()
    {
        debrisName = INITIAL_DEBRIS_NAME;
        orbitFirstAxis = 0;
        orbitSecondAxis = 0;
        initialPosition = 0;
        distanceFromEarthKm = INITIAL_DEBRIS_DISTANCE_FROM_EARTH_KM;
        mass = INITIAL_DEBRIS_MASS_KG;
        height = INITIAL_DEBRIS_DIMENSION_M;
        width = INITIAL_DEBRIS_DIMENSION_M;
        length = INITIAL_DEBRIS_DIMENSION_M;
        shapeEnumIndex = 0;
    }

    public DebrisData CreateDebrisData()
    {
        return new DebrisData(debrisName, orbitFirstAxis, orbitSecondAxis, initialPosition,
            distanceFromEarthKm, mass, shape, height, length, width);
    }
}
