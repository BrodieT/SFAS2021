using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//This script updates the quest marker UI to point in the direction of the objective
public class QuestMarker : MonoBehaviour
{
    [SerializeField] Image _questMarker = default; //The quest marker image
    private Transform _target = default; //The location the marker will point to
    private Camera _playerCamera = default; //Local ref to the player camera
    private TMP_Text _distanceCount = default; //The text display for the distance the player is to the target
    [SerializeField] Vector3 _markerOffset = new Vector3(0, 1, 0); //How much the marker will be offset by (to prevent it covering up the objective)

    private void Start()
    {
        //Get the player camera from the game manager
        _playerCamera = Game_Manager.instance._playerCamera;
        //Get the text for displaying the distance between the player and objective
        _distanceCount = transform.GetComponentInChildren<TMP_Text>();
    }
    
    //This function is used to update the objective position the marker will be directing the player to
    public void UpdateQuestTarget(Transform t)
    {
        _target = t;
    }

    // Update is called once per frame
    void Update()
    {
        if (_target != null)
        {
            //Calculate the bounds of the screen
            float minX = _questMarker.GetPixelAdjustedRect().width / 2;
            float maxX = Screen.width - minX;

            float minY = _questMarker.GetPixelAdjustedRect().height / 2;
            float maxY = Screen.height - minY;

            //Calculate where the marker should be in screen space
            Vector2 markerScreenPos = _playerCamera.WorldToScreenPoint(_target.position + _markerOffset);

            //Check if the marker is behind the player and position at the bounds if this is the case
            if (Vector3.Dot(_target.position - _playerCamera.transform.position, _playerCamera.transform.forward) < 0)
            {
                //The target is behind the player
                if (markerScreenPos.x < Screen.width / 2)
                    markerScreenPos.x = maxX;
                else
                    markerScreenPos.x = minX;
            }

            //clamp the marker screen position within the bounds of the screen
            markerScreenPos.x = Mathf.Clamp(markerScreenPos.x, minX, maxX);
            markerScreenPos.y = Mathf.Clamp(markerScreenPos.y, minY, maxY);

            //Update the marker position
            _questMarker.transform.position = markerScreenPos;

            //Calculate and update the marker distance accordingly 
            _distanceCount.text = ((int)(Vector3.Distance(_playerCamera.transform.position, _target.position))).ToString();
        }
    }
}
