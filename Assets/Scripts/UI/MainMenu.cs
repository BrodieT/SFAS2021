using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
   public void NewGame()
    {
        SceneLoader.instance.StartNewGame();
    }

    public void ContinueGame()
    {
        SceneLoader.instance.ContinueGame();
    }

    public void Settings()
    {
        Debug.Log("Settings Menu Toggle");
    }

    public void QuitGame()
    {
        SceneLoader.instance.QuitGame();
    }
}
