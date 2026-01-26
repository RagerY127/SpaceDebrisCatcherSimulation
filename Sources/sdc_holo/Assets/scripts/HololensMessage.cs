using UnityEngine;
using System; // Obligatoire pour [Serializable]
using System.Collections.Generic;
public enum MessageType { DEBRIS, CATCHER }


[Serializable]
public class DataEntry//La classe Dictionary n'étant pas sérialisable,il faut créer la notre
{
    public string key;
    public string value; //TYPE A CHANGER!
}

/*
Classe qui contient le contenu d'un message vers l'Hololens. Un message peut être de différent types.
Pour l'utiliser il faut instancier un objet de cette classe avec un type de messsage puis lui fournir des informations via AddData()
A l'envoi:
    string StringToSend=JsonUtility.ToJson(message);

A la réception:
    HololensMessage hololensMessage = JsonUtility.FromJson<HololensMessage>(stringToSend);
*/
[Serializable]
public class HololensMessage{
    private MessageType type;
    private List<DataEntry> messageData = new List<DataEntry>();
    public HololensMessage(MessageType type){
        this.type=type;
    }

    public void AddData(string name,string data){
        messageData.Add(new DataEntry { key = name, value = data });
    }

    public List<DataEntry> GetMessageData(){return messageData;}
    public MessageType GetMessageType(){return type;}
}