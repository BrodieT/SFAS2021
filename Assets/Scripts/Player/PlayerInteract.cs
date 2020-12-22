using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerInteract : AutoCleanupSingleton<PlayerInteract>
{
    [SerializeField] public Camera _playerCamera = default; //Local store of the player camera
    [SerializeField] private float _interactRange = 5.0f; //The range at which players can interact with objects
    [SerializeField] private TMP_Text _interactUI = default; //The UI prompt for showing what interactable is being looked at
    [SerializeField] private LayerMask _interactLayer = default; //The layers that can be interacted with
    private RaycastHit _targetInteractable = default; //The currently selected interactable
    
    public void SkipDialogue(InputAction.CallbackContext context)
    {
        if (context.performed && !GameUtility._isPlayerObjectBeingControlled)
        {
            DialogueManager dM = default;

            if(_targetInteractable.transform.TryGetComponent<DialogueManager>(out dM))
            {
                dM._outputScreen.QuickDisplay();
            }
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (GameUtility._isPlayerObjectBeingControlled)
        {
            Interact();
        }
    }

    private void FixedUpdate()
    {
        if (GameUtility._isPlayerObjectBeingControlled)
        {
            //Raycast to determine if the player is looking at an interactable
            if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out _targetInteractable, _interactRange, _interactLayer))
            {
                //Only proceed if the interact UI is valid
                if (_interactUI != null)
                {
                    //Show the appropriate prompt based on what interactable is being looked at

                    _interactUI.gameObject.SetActive(true);

                    switch (_targetInteractable.transform.GetComponent<Interactable>().InteractableType)
                    {
                        case Interactable.InteractType.Revolver:
                            _interactUI.text = "Pick Up Revolver";
                            break;
                        case Interactable.InteractType.GrappleGun:
                            _interactUI.text = "Pick Up Grapple Gun";
                            break;
                        case Interactable.InteractType.Laptop:
                            _interactUI.text = "Use Terminal";
                            break;
                    }

                }
            }
            else
            {
                //Disable the UI prompt if no interactable is looked at
                if (_interactUI != null)
                {
                    _interactUI.gameObject.SetActive(false);
                }
            }
        }
    }

    //This function handles the interactable processes
    private void Interact()
    {
        //Only proceed if there is a target interactable to interact with
        if (_targetInteractable.transform == null)
        {
            return;
        }

        Interactable target = default;

        if (_targetInteractable.transform.TryGetComponent<Interactable>(out target))
        {


            switch (_targetInteractable.transform.GetComponent<Interactable>().InteractableType)
            {
                case Interactable.InteractType.Revolver:
                    //PickupGun(RightHand, TargetInteractable.transform.gameObject);
                    break;
                case Interactable.InteractType.GrappleGun:
                    //PickupGun(LeftHand, TargetInteractable.transform.gameObject);
                    break;
                case Interactable.InteractType.Laptop:
                    //Disable player movement
                    //GameUtility._isPlayerObjectBeingControlled = false;

                    //Disable the UI prompt
                    if (_interactUI != null)
                    {
                        _interactUI.gameObject.SetActive(false);
                    }

                    //Find the player interact position 
                    for (int i = 0; i < _targetInteractable.transform.childCount; i++)
                    {
                        if (_targetInteractable.transform.GetChild(i).CompareTag("PlayerHolder"))
                        {
                            Transform targetTransform = _targetInteractable.transform.GetChild(i).transform;
                            
                            //Find the camera holder 
                            for (int j = 0; j < _targetInteractable.transform.childCount; j++)
                            {
                                if (_targetInteractable.transform.GetChild(j).CompareTag("CameraHolder"))
                                {
                                    Transform targetCameraTransform = _targetInteractable.transform.GetChild(j).transform;

                                    //Use the autopilot to move to the correct interact transforms
                                    PlayerAutoPilot.instance.BeginAutoPilot(targetTransform.position, targetTransform.rotation, targetCameraTransform.position, targetCameraTransform.rotation);
                                    //Wait for the autopilot to finish
                                    StartCoroutine(IsTerminalReady());
                                }
                            }

                        }
                    }

                    break;
            }
        }

    }

    IEnumerator IsTerminalReady()
    {
        bool _ready = false;
        
        while(!_ready)
        {
            if(!PlayerAutoPilot.instance._isAutoPiloting)
            {
                if(_targetInteractable.transform == null)
                {
                    Debug.LogError("Terminal Interactable becoming null before player was in position");
                    _ready = true;
                }

                //Prepare the dialogue manager on the terminal to begin the story
                DialogueManager dialogue;
                GameUtility.ShowCursor();

                if (_targetInteractable.transform.TryGetComponent(out dialogue))
                {
                    dialogue.StartDisplay();
                }
                else
                {
                    Debug.LogError("No Dialogue Manager found on laptop interactable");
                }

                _ready = true;

            }
            yield return null;
        }
    }


}
