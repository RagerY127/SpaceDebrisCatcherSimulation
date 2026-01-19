using UnityEngine;
using System; // Obligatoire pour [Serializable]
using System.Collections.Generic;
[Serializable]
public class ObjectDataEntry//La classe Dictionary n'étant pas sérialisable,il faut créer la notre
{
    [SerializeField] public string key;
    [SerializeField] public string value;
}
public class ObjectDataBundle
{
    [SerializeField] public List<ObjectDataEntry> messageData = new List<ObjectDataEntry>();
}
    