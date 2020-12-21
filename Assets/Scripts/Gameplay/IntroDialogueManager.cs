using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroDialogueManager : DialogueManager
{
    private void Awake()
    {
        StartDisplay();
    }

    public override void UpdateInput()
    {
        if (_story._isCompleted == true)
        {
            GetComponentInParent<Animator>().SetTrigger("Play");
            this.enabled = false;
        }

        base.UpdateInput();

        
    }
}
