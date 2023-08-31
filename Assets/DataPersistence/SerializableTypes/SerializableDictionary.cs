using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// class for making dictionaries serializable/Deserializble during load/save

[System.Serializable]
public class SerializableDictionary<TKey,TValue>: Dictionary<TKey,TValue>,ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    //Save The dictionary to lists

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear(); 
        foreach(KeyValuePair<TKey,TValue> kvp in this)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    //LOAD the dictionary from lists

    public void OnAfterDeserialize()
    {
        this.Clear();
        if (keys.Count != values.Count)
        {
            Debug.LogError("Tried to deserialize a SerializableDictionary but the amount of keys(" + keys.Count + ") does not match the number of values (" + values.Count + ")");
        }
        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }
}
