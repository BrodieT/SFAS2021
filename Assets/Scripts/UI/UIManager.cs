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
    [SerializeField] public DiscoveryUI _discoveryUI = default;

    [Header("Game Menus")]
    [SerializeField] private GameObject _pauseMenu = default;
    [SerializeField] private UnityEvent _onGamePaused = default;

    [Header("Debug")]
    [SerializeField] private bool _debugMode = false;
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
                Pause();
            }
            else
            {
                Resume();
            }
        }
    }

    public void Resume()
    {
        GameUtility._isPaused = false;
        Time.timeScale = 1;
        _pauseMenu.SetActive(false);
        GameUtility.HideCursor();
    }
    public void Pause()
    {
        _onGamePaused?.Invoke();
        GameUtility._isPaused = true;
        Time.timeScale = 0;
        _pauseMenu.SetActive(true);
        GameUtility.ShowCursor();
    }

    public TMP_Text GetInteractPrompt()
    {
        return _interactPrompt.GetComponent<TMP_Text>();
    }

    private void Start()
    {
        if (_debugMode)
        {
            _mainCanvas.SetActive(true);
            _reticle.SetActive(true);
            _dialogueWindow.SetActive(false);
            _hud.SetActive(false);
        }
        else
        {
            _mainCanvas.SetActive(true);
            _reticle.SetActive(true);
            _dialogueWindow.SetActive(false);
            _hud.SetActive(true);
        }
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
