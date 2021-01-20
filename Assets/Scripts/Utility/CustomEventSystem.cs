using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//This class is a custom event system which will handle sending/recieving signals from various other systems in the project
public class CustomEventSystem : MonoBehaviour
{
    public event Action DialogueChoice;

    public static event Action BeginTutorial;
    public static event Action SkipTutorial;

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
