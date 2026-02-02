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
}
