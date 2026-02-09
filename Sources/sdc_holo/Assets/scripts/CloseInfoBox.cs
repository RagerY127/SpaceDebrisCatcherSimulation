using UnityEngine;

public class CloseInfoBox : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void close(){
        Debug.Log("Close Info Box");
        GameObject parent = this.transform.parent.gameObject;//appWindow
        Destroy(parent.transform.parent.gameObject);//objet info panel

    }
}
