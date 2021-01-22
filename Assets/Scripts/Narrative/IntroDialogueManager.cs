using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroDialogueManager : TerminalManager
{
    public override void Start()
    {
        if (!ProgressionTracker.instance.HasSceneBeenLoadedBefore(SceneLoader.instance.GetCurrentScene()))
        {
            ProgressionTracker.instance.AddNewLoadedScene(SceneLoader.instance.GetCurrentScene());

            base.Start();

            PlayerInteract interact = Game_Manager.instance._player.GetComponent<PlayerInteract>();
            interact._targetInteractable = GetComponent<Interactable>();
            interact.Interact();
        }
        else
        {
            base.Start();

            FinishDisplay();
        }
    }

    public override void UpdateInput()
    {
        if (_story._isCompleted == true)
        {
            //GetComponentInParent<Animator>().SetTrigger("Play");
            this.enabled = false;
        }

        base.UpdateInput();
    }
}
