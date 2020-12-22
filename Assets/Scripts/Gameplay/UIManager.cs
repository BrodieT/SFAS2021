using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : AutoCleanupSingleton<UIManager>
{
    [SerializeField] private GameObject _gameUI = default;
    [SerializeField] private GameObject _settingsMenu = default;
    [SerializeField] private GameObject _pauseMenu = default;
    [SerializeField] private GameObject _dialogueWindow = default;

    private void Start()
    {
        _dialogueWindow.SetActive(false);
    }

    public GameObject GetDialogueWindow()
    {
        return _dialogueWindow;
    }
}
