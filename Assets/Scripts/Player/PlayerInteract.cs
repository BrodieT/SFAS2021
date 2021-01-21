using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    private Camera _playerCamera = default; //Local store of the player camera
    [SerializeField] private float _interactRange = 5.0f; //The range at which players can interact with objects
    [SerializeField] private LayerMask _interactLayer = default; //The layers that can be interacted with
    private TMP_Text _interactPrompt = default;
    public Interactable _targetInteractable { get; set; } //The currently selected interactable
    private PlayerAutoPilot _autopilot = default;
    private void Start()
    {
        _playerCamera = Game_Manager.instance._playerCamera;
        if (!Game_Manager.instance._player.TryGetComponent<PlayerAutoPilot>(out _autopilot))
        {
            Debug.LogWarning("No Autopilot found. Manually Adding one now.");
            PlayerAutoPilot autoPilot = Game_Manager.instance._player.AddComponent(typeof(PlayerAutoPilot)) as PlayerAutoPilot;
        }
        
        _interactPrompt = Game_Manager.instance._UIManager.GetInteractPrompt();

    }


    public void SkipDialogue(InputAction.CallbackContext context)
    {
        if (!GameUtility._isPaused)
        {
            if (context.performed && !GameUtility._isPlayerObjectBeingControlled)
            {
                if (_targetInteractable != null)
                {
                    if (_targetInteractable.transform.TryGetComponent<BranchingNarrative>(out BranchingNarrative dM))
                    {
                        dM._outputScreen.QuickDisplay();
                    }
                }
            }
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (GameUtility._isPlayerObjectBeingControlled && !GameUtility._isPaused)
        {
            if(context.performed)
                Interact();
        }
    }

    private void FixedUpdate()
    {
        if (GameUtility._isPlayerObjectBeingControlled && !GameUtility._isPaused)
        {
            //Raycast to determine if the player is looking at an interactable
            if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out RaycastHit target, _interactRange, _interactLayer))
            {
                _targetInteractable = target.transform.GetComponent<Interactable>();

                //Only proceed if the interact UI is valid
                if (_interactPrompt != null)
                {
                    //Show the appropriate prompt based on what interactable is being looked at
                    _interactPrompt.gameObject.SetActive(true);
                    _interactPrompt.text = _targetInteractable.transform.GetComponent<Interactable>()._interactPromptText;
                }
            }
            else
            {
                //Disable the UI prompt if no interactable is looked at
                if (_interactPrompt != null)
                {
                    _interactPrompt.gameObject.SetActive(false);
                }
            }
        }
    }

    //This function handles the interactable processes
    public void Interact()
    {
        //Only proceed if there is a target interactable to interact with
        if (_targetInteractable == null)
        {
            return;
        }

        _targetInteractable.DoInteract();            
        

    }

    //public void Interact(Interactable obj)
    //{
    //    //Only proceed if there is a target interactable to interact with
    //    if (obj == null)
    //    {
    //        return;
    //    }

    //    obj.DoInteract();    

    //}
}
