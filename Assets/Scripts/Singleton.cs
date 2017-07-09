using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

    private static T instace;

    public static T Instace
    {
        get
        {
            if (instace == null)
            {
                instace = FindObjectOfType<T>();
            }
            else if (instace != FindObjectOfType<T>())
            {
                Destroy(FindObjectOfType<T>());
            }
            DontDestroyOnLoad(FindObjectOfType<T>());

            return instace;
        }
    }
}
