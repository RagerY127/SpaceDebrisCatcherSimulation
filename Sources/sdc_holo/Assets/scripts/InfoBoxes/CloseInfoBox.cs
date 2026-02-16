using UnityEngine;

public class CloseInfoBox : MonoBehaviour
{
    public void close()
    {
        Debug.Log("Close Info Box - Universal");

        DebrisInfo debrisRoot = GetComponentInParent<DebrisInfo>();
        if (debrisRoot != null)
        {
            debrisRoot.gameObject.SetActive(false);
            return; 
        }

        CatcherInfo catcherRoot = GetComponentInParent<CatcherInfo>();
        if (catcherRoot != null)
        {
            catcherRoot.gameObject.SetActive(false);
            return; 
        }


        if (transform.parent != null && transform.parent.parent != null)
        {
            transform.parent.parent.gameObject.SetActive(false);
        }
        else if (transform.parent != null)
        {
            transform.parent.gameObject.SetActive(false);
        }
    }
}