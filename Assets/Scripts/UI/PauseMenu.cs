using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] Button _resumeButton = default;
    [SerializeField] Button _quitButton = default;


    private void Start()
    {
        _resumeButton.onClick.AddListener(delegate { Game_Manager.instance._UIManager.Resume(); });
        _quitButton.onClick.AddListener(delegate { Application.Quit(); });
    }
}
