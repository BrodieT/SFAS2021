using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is used to return the player to a pre-defined position should they enter its trigger collider
//Primarily used in the tutorial for falling off of platforms but could come in handy elsewhere
public class PlayerRespawnCheckpoint : MonoBehaviour
{
    [SerializeField] private Transform _respawnPoint = default; //The linked respawn point that the player will be returned to

    private void OnTriggerEnter(Collider other)
    {
        //If the player enters the trigger
        //Update their position and rotation to that of the respawn point
        if(other.CompareTag("Player"))
        {
            other.transform.position = _respawnPoint.position;
            other.transform.rotation = _respawnPoint.rotation;
        }
    }
}
