using UnityEngine;
using System; // Obligatoire pour [Serializable]
using System.Collections.Generic;
[Serializable]
public class ObjectDataEntry//La classe Dictionary n'étant pas sérialisable,il faut créer la notre
{
    [SerializeField] public string key;
    [SerializeField] public string value;
}
[Serializable]
public class ObjectDataBundle
{
    [SerializeField] public List<ObjectDataEntry> data = new List<ObjectDataEntry>();
     public string GetValue(string key)
    {
        foreach (var entry in data)
        {
            if (entry.key == key)
            {
                return entry.value;
            }
        }
        return null; // retourne null si la clé n'est pas trouvée
    }
}
