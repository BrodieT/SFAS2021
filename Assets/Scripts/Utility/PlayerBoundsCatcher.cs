using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is to catch the player should they somehow fall out of the map
public class PlayerBoundsCatcher : MonoBehaviour
{
    private Vector3 _defaultPosition = default;
    private GameObject _player = default;
    [SerializeField] private float _fallThreshold = -10.0f; //The y-axis value the player would need to exceed to be considered out of bounds
    [SerializeField] private float _fallCheckFrequency = 5.0f; //How frequently the check will take place
    // Start is called before the first frame update
    void Start()
    {
        _player = Game_Manager.instance._player;
        _defaultPosition = _player.transform.position;

        StartCoroutine(FallCheck());
    }

    IEnumerator FallCheck()
    {
        while(true)
        {
            if(_player.transform.position.y <= _fallThreshold)
            {
                _player.transform.position = _defaultPosition;
            }

            yield return new WaitForSeconds(5.0f);
        }
    }
}
