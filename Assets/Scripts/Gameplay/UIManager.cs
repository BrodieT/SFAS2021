using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using TMPro;

public class UIManager : MonoBehaviour
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
    [SerializeField] private UnityEvent _onGamePaused = default;

    public bool GetQuestMarker(out QuestMarker marker)
    {
        if(_questMarker.TryGetComponent<QuestMarker>(out marker))
        {
            return true;
        }

        marker = null;
        return false;
    }

    public GameObject GetQuestMarkerObject()
    {
        return _questMarker;
    }
   
    public void PauseGame(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!GameUtility._isPaused)
            {
                _onGamePaused?.Invoke();
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
    }

    public TMP_Text GetInteractPrompt()
    {
        return _interactPrompt.GetComponent<TMP_Text>();
    }

    private void Start()
    {
        _mainCanvas.SetActive(true);
        _reticle.SetActive(true);
        _dialogueWindow.SetActive(false);
        _hud.SetActive(false);

    }

    public GameObject GetDialogueWindow()
    {
        return _dialogueWindow;
    }

    public void HideQuestUI()
    {
        _questLog.SetActive(false);
    }

    public void HideQuestMarker()
    {
        _questMarker.SetActive(false);
    }

    public void ShowQuestUI()
    {
        _questLog.SetActive(true);
    }

    public void ShowQuestMarker()
    {
        _questMarker.SetActive(true);
    }
}
