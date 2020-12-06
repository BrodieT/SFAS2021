using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//This class is a custom event system which will handle sending/recieving signals from various other systems in the project
public class CustomEventSystem : MonoBehaviour
{
    public static CustomEventSystem Instance;
    public event Action DialogueChoice;

    public static event Action BeginTutorial;
    public static event Action SkipTutorial;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }


   public void OnDialogueChoice()
    {
        Debug.Log("Dialogue choice made");
        DialogueChoice?.Invoke();
    }


 
    public static void OnBeginTutorial()
    {
        BeginTutorial?.Invoke();
    }

    public static void OnSkipTutorial()
    {
        SkipTutorial?.Invoke();
    }
}
