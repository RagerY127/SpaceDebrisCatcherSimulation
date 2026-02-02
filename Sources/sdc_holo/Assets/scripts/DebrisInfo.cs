using UnityEngine;
using TMPro;

public class DebrisInfo : MonoBehaviour
{
    [Header("DebrisInfo fields")]
    public GameObject NameField;
    public GameObject revolutionField;
    public GameObject massField;
    public GameObject positionField;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateInfo(string debrisName, double revolution, double mass, double position)
    {
        NameField.GetComponent<TextMeshPro>().text = debrisName;
        revolutionField.GetComponent<TextMeshPro>().text = revolution.ToString("F2") + "/day";
        massField.GetComponent<TextMeshPro>().text = mass.ToString("F2") + " kg";
        positionField.GetComponent<TextMeshPro>().text = position.ToString("F2") + "°";
    }
}
