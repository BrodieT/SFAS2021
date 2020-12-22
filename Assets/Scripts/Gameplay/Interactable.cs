using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [System.Serializable] public enum InteractType { None = 0, GrappleGun = 1, Revolver = 2, Door = 3, Laptop = 4, NPC = 5 }
    [SerializeField] public InteractType InteractableType = 0;
}
