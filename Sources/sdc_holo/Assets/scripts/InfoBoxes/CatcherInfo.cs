using UnityEngine;
using TMPro;
public class CatcherInfo : MonoBehaviour
{
    [Header("CatcherInfo fields")]
    public GameObject targetNameField;
    public GameObject speedField;
    public GameObject targetDistanceField;

    public Catcher Original{get; set;}
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
        if (targetNameField != null)
            targetNameField.GetComponent<TextMeshPro>().text = targetName;

        if (speedField != null)
            speedField.GetComponent<TextMeshPro>().text = speed.ToString("F2") + " km/h";

        if (targetDistanceField != null)
        {
            double displayDistance = targetDistance - 4.3;

            if (displayDistance < 0.01)
            {
                targetDistanceField.GetComponent<TextMeshPro>().text = "Target Reached";
            }
            else
            {
                targetDistanceField.GetComponent<TextMeshPro>().text = displayDistance.ToString("F2") + " km";
            }
        }
    }

    void OnDestroy()
    {
        if (Original != null)
        {
            Original.infoInstance = null;
        }
    }

}
