using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a static utility function for storing game information to be accessed globally
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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public static bool _isCursorHidden = true; //Tracks whether the cursor is hidden
    public static bool _isPaused = false; //Tracks whether the game is paused

    public static bool _isPlayerObjectBeingControlled = true; //Tracks whether the player object is being controlled by the player


    public static void DestroyAllChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            UnityEngine.Object.Destroy(child.gameObject);
        }
    }

    public static string ReplaceWordInString(string original, string wordToReplace, string replacement)
    {
        if (wordToReplace.Length > original.Length)
        {
            Debug.Log("Word to be replaced is longer than the original string.");
            return original;
        }

        if (!original.Contains(wordToReplace))
        {
            Debug.Log("Could not find word to be replaced in given string");
            return original;
        }

        if (wordToReplace.Length == original.Length)
        {
            return replacement;
        }

        int wordSize = wordToReplace.Length;

        for (int i = 0; i < original.Length - wordToReplace.Length; i++)
        {
            string possibleWord = original.Substring(i, wordSize);
            if (possibleWord == wordToReplace)
            {
                string firstPart = "";
                if (i > 0)
                    firstPart = original.Substring(0, i);
                int remainder = i + wordSize;
                string secondPart = original.Substring(remainder, original.Length - (firstPart.Length + wordToReplace.Length));

                return firstPart + replacement + secondPart;

            }
        }

        return original;
    }
}
