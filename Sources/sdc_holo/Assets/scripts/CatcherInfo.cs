using UnityEngine;
using TMPro;
public class CatcherInfo : MonoBehaviour
{
    [Header("CatcherInfo fields")]
    public GameObject targetNameField;
    public GameObject speedField;
    public GameObject targetDistanceField;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateInfo(string targetName, double speed, double targetDistance)
    {
        targetNameField.GetComponent<TextMeshPro>().text = targetName;
        speedField.GetComponent<TextMeshPro>().text = speed.ToString("F2") + " km/h";
        targetDistanceField.GetComponent<TextMeshPro>().text = targetDistance.ToString("F2") + " km";
    }
}
