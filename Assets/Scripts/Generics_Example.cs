using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generics_Example : MonoBehaviour {

    void Awake()
    {
        KeyValuePair<int, string> course = new KeyValuePair<int, string>(1, "Devs");
        course.Print();
    }
}

public class KeyValuePair<TKey, TValue>
{
    public TKey key;
    public TValue value;

    public KeyValuePair(TKey _key, TValue _value)
    {
        key = _key;
        value = _value;
    }

    public void Print()
    {
        Debug.Log("Key: " + key);
        Debug.Log("Value: " + value);
    }
}