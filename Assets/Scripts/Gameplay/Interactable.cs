using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script represents all interactables in the game 
[DisallowMultipleComponent]
public class Interactable : MonoBehaviour
{
    [SerializeField] public string _interactPromptText = ""; //The prompt that will appear to the player when looking at this object

    [SerializeField] private bool _hasAnimation = false; //Determines whether an animation should be played

    public virtual void DoInteract()
    {
        //If the has animation flag is set to true, try to find an animator component and play the animation
        if (_hasAnimation)
        {
            if (transform.TryGetComponent<Animator>(out Animator _anim))
            {
                _anim.SetTrigger("Play");
            }
            else
            {
                Debug.LogWarning("No Animator Found on " + name);
            }
        }
    }
}
