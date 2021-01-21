using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//This interact type triggers an event when it is interacted with
//This can be used for environmental changes as well as quest progressions
public class EventInteract : Interactable
{
    [SerializeField] private UnityEvent _onInteract = default;

    public override void DoInteract()
    {
        base.DoInteract();
        _onInteract?.Invoke();
    }
}
