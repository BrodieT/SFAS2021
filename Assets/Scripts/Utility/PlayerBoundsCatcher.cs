using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is to catch the player should they somehow fall out of the map
public class PlayerBoundsCatcher : MonoBehaviour
{
    private Vector3 _defaultPosition = default; //The default position of the player, where they will be returned to should they fall below the threshold
    private GameObject _player = default; //Local store of the player game object
    [SerializeField] private float _fallThreshold = -10.0f; //The y-axis value the player would need to exceed to be considered out of bounds
    [SerializeField] private float _fallCheckFrequency = 5.0f; //How frequently the check will take place
   
    // Start is called before the first frame update
    void Start()
    {
        //Get the player object from the game manager
        _player = Game_Manager.instance._player;
        //Store the default position on startup
        _defaultPosition = _player.transform.position;
        //Begin the fall check coroutine
        StartCoroutine(FallCheck());
    }

    //This is the coroutine for periodically checking the player's height against the threshold value
    IEnumerator FallCheck()
    {
        //Continue checking forever
        while(true)
        {
            //if the player is below the threshold value then return them to their default position
            if(_player.transform.position.y <= _fallThreshold)
            {
                _player.transform.position = _defaultPosition;
            }

            //This controls how frequently the check is made to prevent it from processing every frame
            yield return new WaitForSeconds(_fallCheckFrequency);
        }
    }
}
