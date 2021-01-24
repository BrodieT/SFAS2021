using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] AudioMixer _masterAudio = default;
    [SerializeField] Slider _masterVolume = default;
    [SerializeField] Slider _ambienceVolume = default;
    [SerializeField] Slider _sfxVolume = default;
    [Header("Graphics")]
    [SerializeField] TMP_Dropdown _graphicsQuality = default;
    [SerializeField] Toggle _fullscreenToggle = default;
    [Header("Gameplay")]
    [SerializeField] Slider _sensitivity = default;

    private void Start()
    {
        _masterVolume.onValueChanged.AddListener(delegate { SetMasterVolume(_masterVolume.value); });
        _ambienceVolume.onValueChanged.AddListener(delegate { SetAmbienceVolume(_ambienceVolume.value); });
        _sfxVolume.onValueChanged.AddListener(delegate { SetSFXVolume(_sfxVolume.value); });
        _sensitivity.onValueChanged.AddListener(delegate { SetSensitivity(_sensitivity.value); });
        _graphicsQuality.onValueChanged.AddListener(delegate { SetQuality(_graphicsQuality.value); });
        _fullscreenToggle.onValueChanged.AddListener(delegate { SetFullscreen(_fullscreenToggle.isOn); });

       
        InitValues();
    }


    private void InitValues()
    {
        _masterVolume.value = SettingsStorage._masterVolume;
        _ambienceVolume.value = SettingsStorage._ambientVolume;
        _sfxVolume.value = SettingsStorage._effectsVolume;
        _sensitivity.value = SettingsStorage._lookSensitivity;
        _fullscreenToggle.isOn = Screen.fullScreen;
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetSensitivity(float sensitivity)
    {
        SettingsStorage._lookSensitivity = sensitivity;
    }

    public void SetMasterVolume(float volume)
    {
        _masterAudio.SetFloat("master", volume);
        SettingsStorage._masterVolume = volume;
    }
    public void SetAmbienceVolume(float volume)
    {
        _masterAudio.SetFloat("ambience", volume);
        SettingsStorage._ambientVolume = volume;
    }
    public void SetSFXVolume(float volume)
    {
        _masterAudio.SetFloat("sfx", volume);
        SettingsStorage._effectsVolume = volume;
    }

}
