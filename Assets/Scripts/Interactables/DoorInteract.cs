﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//This script is used for an interactable with door functionality
public class DoorInteract : Interactable
{
    [SerializeField] private bool _isLoadDoor = false; //Determines whether interacting with this door will switch scenes
    [SerializeField] private SceneLoader.SceneName _linkedScene = SceneLoader.SceneName.None; //The scene that will be switched to if this is a load door

    [Header("Additional Functionality")]
    public UnityEvent _onDoorOpen = default;

    [Header("Audio")]
    [SerializeField] private bool _playAudio = true;
    [SerializeField] AudioClip _openSound = default;
    [SerializeField] AudioClip _closeSound = default;

    private bool _isOpen = false; //Tracks whether the door is open or not

    public void OnValidate()
    {
        //if not a load door just set the linked scene to none for neatness
        if(!_isLoadDoor)
        {
            _linkedScene = SceneLoader.SceneName.None;
        }
    }
    
    public override void  DoInteract()
    {
        //If this is a load door, switch scenes
        //otherwise look for an animator to open the door
        if(_isLoadDoor)
        {
            _onDoorOpen?.Invoke();
            SceneLoader.instance.LoadLevel(_linkedScene);
        }
        else
        {
            if(transform.TryGetComponent<Animator>(out Animator _anim))
            {
                _isOpen = !_isOpen;
                
                if(_isOpen)
                    _onDoorOpen?.Invoke();

                if (_playAudio)
                {
                    if (transform.TryGetComponent<AudioSource>(out AudioSource source))
                    {
                        if (_isOpen)                      
                            source.PlayOneShot(_openSound);                        
                        else                        
                            source.PlayOneShot(_closeSound);                        
                    }
                }

                _anim.SetBool("Open", _isOpen);               
            }

            if (_isOpen)
                _interactPromptText = GameUtility.ReplaceWordInString(_interactPromptText, "Open", "Close");
            else
                _interactPromptText = GameUtility.ReplaceWordInString(_interactPromptText, "Close", "Open");
        }

       



    }
}
