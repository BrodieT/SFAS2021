using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class rotates an object to always face the camera
//It is particularly useful for some worldspace UI (such as enemy healthbars) 
//to ensure they are always the correct way round
public class Billboard : MonoBehaviour
{
    private Camera _mainCamera;    //Local store of the main camera the object will rotate to face

    void Start()
    {     
        //Store the main camera from the game manager
        _mainCamera = Game_Manager.instance._playerCamera;
    }

    void LateUpdate()
    {
        //Rotate to always face the main camera
        //Performed in late update to ensure any camera processing is carried out first
        transform.LookAt(transform.position + _mainCamera.transform.forward);
    }
}
