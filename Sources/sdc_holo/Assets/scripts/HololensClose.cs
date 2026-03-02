using UnityEngine;

public class HololensClose : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //update button position and angle based on camera position and angle
        if (Camera.main != null)
        {
            Transform camTransform = Camera.main.transform;
            transform.position = camTransform.position + camTransform.forward * 0.6f + camTransform.up * 0.2f - camTransform.right * 0.5f;
            transform.rotation = camTransform.rotation;
        }
    }
    public void onClick()
    {
        if (ObjectManager.Instance != null)
        {
            ObjectManager.Instance.ClearSpawnedObjects();
        }
    }
}
