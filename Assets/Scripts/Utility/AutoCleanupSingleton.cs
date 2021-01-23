using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class is used for creating a singleton that will automatically clean itself up
//It can be applied to any monobehaviour
public class AutoCleanupSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    //Getter for the singleton instance of this type
    public static T instance
    {
        get
        {
            //If the instance is null, meaning there is not currently a component of this type
            if (_instance == null)
            {
                //Attempt to find an instance of this type
                _instance = FindObjectOfType<T>();


                //If the instance is still null
                //Create a new instance and return that instead
                if (_instance == null)
                {
                    Debug.LogWarning("Component type " + typeof(T) + " could not be found. Instantiating a new one");
                    _instance = new GameObject("Instance of " + typeof(T)).AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    public virtual void Awake()
    {
        //Prevents duplicates by destroying this version if another already exists
        if (_instance != null)
        {
            Debug.LogWarning(typeof(T) + " appears more than once on " + _instance.name + " and " + name);
            Destroy(gameObject);
        }
    }
}
