using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAutoPilot : AutoCleanupSingleton<PlayerAutoPilot>
{
    NavMeshAgent _myNavMeshAgent = default;
    float _radius = 0.0f;
    float _height = 0.0f;
    float _speed = 0.0f;
    Camera _playerCamera = default;
    private Vector3 _defaultCamPos = default; //The default camera transform
    private Quaternion _defaultCamRote = default; //The default camera transform

    public bool _isAutoPiloting { get; private set; }

    private void Start()
    {
        _height = GetComponent<CharacterController>().height;
        _radius = GetComponent<CharacterController>().radius;
        _speed  = PlayerMovement.instance._walkSpeed;
        _playerCamera = PlayerInteract.instance._playerCamera;
        _isAutoPiloting = false;
        _defaultCamPos = _playerCamera.transform.localPosition;
        _defaultCamRote = _playerCamera.transform.localRotation;

    }

    //Begin autopilot (excluding camera)
    public void BeginAutoPilot(Vector3 targetPosition, Quaternion targetRotation)
    {
        //Check that no operation is already underway
        if (_isAutoPiloting)
        {
            Debug.LogError("Unable to auto-pilot player. Auto-pilot already in progress");
            return;
        }

        _isAutoPiloting = true;


        //Stop player from controlling their character
        GameUtility._isPlayerObjectBeingControlled = false;

        //Add a NavMeshAgent to handle the autopilot movement
        _myNavMeshAgent = transform.gameObject.AddComponent<NavMeshAgent>();

        //Adjust the bounds and speed of the newly created navmesh agent to correspond
        //with the existing character controller
        _myNavMeshAgent.height = _height;
        _myNavMeshAgent.radius = _radius;
        _myNavMeshAgent.speed  = _speed;
        _myNavMeshAgent.baseOffset = _height / 2;

        //Start the coroutine to being the autopilot 
        StartCoroutine(AutoPilot(targetPosition, targetRotation));
    }

    //Begin the autopilot process (including camera)
    public void BeginAutoPilot(Vector3 targetPosition, Quaternion targetRotation, Vector3 targetCamPos, Quaternion targetCamRote)
    {
        //Check that no operation is already underway
        if (_isAutoPiloting)
        {
            Debug.LogError("Unable to auto-pilot player. Auto-pilot already in progress");
            return;
        }

        _isAutoPiloting = true;

        //Stop player from controlling their character
        GameUtility._isPlayerObjectBeingControlled = false;

        //Add a NavMeshAgent to handle the autopilot movement
        _myNavMeshAgent = transform.gameObject.AddComponent<NavMeshAgent>();

        //Adjust the bounds and speed of the newly created navmesh agent to correspond
        //with the existing character controller
        _myNavMeshAgent.height = _height;
        _myNavMeshAgent.radius = _radius;
        _myNavMeshAgent.speed = _speed;
        _myNavMeshAgent.baseOffset = _height / 2;

        //Start the coroutine to being the autopilot 
        StartCoroutine(AutoPilot(targetPosition, targetRotation, targetCamPos, targetCamRote));
    }

    //Autopilot for player (excluding camera piloting)
    IEnumerator AutoPilot(Vector3 pos, Quaternion rote)
    {
        //Set the destination of the autopilot agent
        _myNavMeshAgent.SetDestination(pos);
        
        //Wait until the destination is closer
        while(Vector3.Distance(transform.position, pos) > 1.0f)
        {
            Debug.Log("Navigating");
            yield return null;
        }

        CleanupNavMeshAgent();
        //Auto-adjust position
        transform.position = pos;

        //Rotate the player to face the correct direction
        for (float i = 0; i < 1.0f; i += (Time.deltaTime / 1.0f))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rote, i);
            yield return null;
        }

        _isAutoPiloting = false;
    }

    //Autopilot for player (including camera piloting)
    IEnumerator AutoPilot(Vector3 pos, Quaternion rote, Vector3 camPos, Quaternion camRote)
    {
        //Set the destination of the autopilot agent
        _myNavMeshAgent.SetDestination(pos);

        //Wait until destination is closer
        while (Vector3.Distance(transform.position, pos) > 1.0f)
        {
            Debug.Log("Navigating");

            yield return null;
        }

        CleanupNavMeshAgent();

        //Auto-Adjust position
        transform.position = pos;

        //Rotate the player to face the correct direction
        for (float i = 0; i < 1.0f; i += (Time.deltaTime / 1.0f))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rote, i);
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        //Autopilot camera controls
        for (float i = 0; i < 1.0f; i += (Time.deltaTime / 3.0f))
        {
            _playerCamera.transform.position = Vector3.Lerp(_playerCamera.transform.position, camPos, i);
            _playerCamera.transform.rotation = Quaternion.Slerp(_playerCamera.transform.rotation, camRote, i);

            yield return null;
        }

        _isAutoPiloting = false;
    }

    private void CleanupNavMeshAgent()
    {
        //Cleanup of NavMeshAgent
        Destroy(_myNavMeshAgent);
        _myNavMeshAgent = null;
    }

    public void ResetCamera()
    {
        StopAllCoroutines();
        CleanupNavMeshAgent();
        _isAutoPiloting = false;
        StartCoroutine(ResetCameraTranslation());
    }

    IEnumerator ResetCameraTranslation()
    {
        //Autopilot camera controls
        for (float i = 0; i < 1.0f; i += (Time.deltaTime / 3.0f))
        {
            _playerCamera.transform.localPosition = Vector3.Lerp(_playerCamera.transform.localPosition, _defaultCamPos, i);
            _playerCamera.transform.localRotation = Quaternion.Slerp(_playerCamera.transform.localRotation, _defaultCamRote, i);

            yield return null;
        }
    }
}
