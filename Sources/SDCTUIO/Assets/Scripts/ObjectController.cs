using UnityEngine;
using TouchScript.Gestures;

public class ObjectController<ObjectDataT> : MonoBehaviour where ObjectDataT : ObjectData
{
    [SerializeField]
    protected TapGesture TapGesture;
    // Long gesture in anneau Controller
    [SerializeField]
    protected LongPressGesture LongPressGesture;
    public ObjectDataT ObjectData { get; protected set; }
}
