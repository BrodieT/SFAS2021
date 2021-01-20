﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : AutoCleanupSingleton<UIManager>
{
    [Header ("Game UI")]
    [SerializeField] private GameObject _mainCanvas = default;
    [SerializeField] private GameObject _interactPrompt = default;
    [SerializeField] private GameObject _questLog = default;
    [SerializeField] private GameObject _questMarker = default;
    [SerializeField] private GameObject _reticle = default;
    [SerializeField] private GameObject _dialogueWindow = default;
    [SerializeField] private GameObject _hud = default;
   
    [Header("Game Menus")]
    [SerializeField] private GameObject _pauseMenu = default;


    public bool _isFullyUnlockedUI { get; private set; }

    //This function unlocks all UI elements after the tutorial has been completed
    public void FullyUnlockUI()
    {
        _questLog.SetActive(true);
        _questMarker.SetActive(true);
        _reticle.SetActive(true);
        _hud.SetActive(true);
        _isFullyUnlockedUI = true;
    }

    //This function is called on startup to show only the basic UI elements during the tutorial section of the game
    private void ShowLockedUI()
    {
        _mainCanvas.SetActive(true);
        _interactPrompt.SetActive(false);
        _questLog.SetActive(true);
        _questMarker.SetActive(true);
        _reticle.SetActive(true);
        _dialogueWindow.SetActive(false);
        _hud.SetActive(false);

        _isFullyUnlockedUI = false;

    }

    public bool GetQuestMarker(out QuestMarker marker)
    {
        if(_questMarker.activeSelf)
        {
            marker = _questMarker.GetComponent<QuestMarker>();
            return true;
        }

        marker = null;
        return false;
    }

    public void PauseGame()
    {
        if (!GameUtility._isPaused)
        {
            GameUtility._isPaused = true;
            Time.timeScale = 0;
            _pauseMenu.SetActive(true);
            GameUtility.ShowCursor();
        }
        else
        {
            GameUtility._isPaused = false;
            Time.timeScale = 1;
            _pauseMenu.SetActive(false);
            GameUtility.HideCursor();
        }
    }

    private void Start()
    {
        ShowLockedUI();
    }

    public GameObject GetDialogueWindow()
    {
        return _dialogueWindow;
    }
}
