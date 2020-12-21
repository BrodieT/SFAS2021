using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameUtility
{
    //This function shows the cursor
    public static void ShowCursor()
    {
        _isCursorHidden = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    //This function hides the cursor
    public static void HideCursor()
    {
        _isCursorHidden = true;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    public static bool _isCursorHidden = true; //Tracks whether the cursor is hidden
    public static bool _isPaused = false; //Tracks whether the game is paused

    public static bool _isPlayerObjectBeingControlled = true;


    //These bools are used to track what gear the player picks up at the start of the game
    //This will be used for achievements to track whether the player is completing a pacifist run, or no gear run
    public static bool PickedUpRevolver = false;
    public static bool PickedUpGrapple = false;
}
