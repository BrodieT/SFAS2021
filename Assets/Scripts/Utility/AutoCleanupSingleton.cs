using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class is used for creating a singleton that will automatically clean itself up
public class AutoCleanupSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<T>();

                if (_instance == null)
                {
                    Debug.LogError("Component type " + typeof(T) + " could not be found. Instantiating a new one");
                    _instance = new GameObject("Instance of " + typeof(T)).AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        //Prevents duplicates
        if (_instance != null)
        {
            Debug.LogWarning(typeof(T) + " appears more than once on " + _instance.name + " and " + name);
            Destroy(gameObject.GetComponent<T>());
        }
    }
}
