using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera _mainCamera;
    void Start()
    {     
        //Store the main camera

        _mainCamera = Game_Manager.instance._playerCamera;

    }

    void LateUpdate()
    {
        //Rotate to always face the main camera
        transform.LookAt(transform.position + _mainCamera.transform.forward);
    }
}
