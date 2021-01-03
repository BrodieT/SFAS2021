using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class QuestMarker : AutoCleanupSingleton<QuestMarker>
{
    [SerializeField] Image _questMarker = default; //The quest marker image
    private Transform _target = default; //The location the marker will point to
    private Camera _player = default; //local ref to the player camera
    private TMP_Text _distanceCount = default; //the text display for the distance the player is to the target
    [SerializeField] Vector3 _markerOffset = new Vector3(0, 1, 0);
    private void Start()
    {
        _player = PlayerInteract.instance._playerCamera;
        _distanceCount = transform.GetComponentInChildren<TMP_Text>();
    }
    
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
            Vector2 markerScreenPos = _player.WorldToScreenPoint(_target.position + _markerOffset);

            //Check if the marker is behind the player and position at the bounds if this is the case
            if (Vector3.Dot(_target.position - _player.transform.position, _player.transform.forward) < 0)
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
            _distanceCount.text = ((int)(Vector3.Distance(_player.transform.position, _target.position))).ToString();
        }
    }
}
