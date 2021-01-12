using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGrapple : MonoBehaviour
{

    public enum GrappleType { Zip = 0, Swing = 1} //The two modes of grappling
    private GrappleType _currentGrappleType = 0; //The currently equipped grapple type
    private Camera _playerCamera = default; //local store of player camera
    [SerializeField] private float _grappleRange = 100.0f; //the max distance that the grapple hook can reach
    [SerializeField] private float _minDistance = 10.0f; //how close the player needs to be before the grapple will disengage
    [SerializeField] private float _zipGrappleTime = 3.0f; //the time it takes for the player to zip to the destination
    [SerializeField] private float _grappleFireTime = 2.0f; //the time it takes for the grapple hook to reach the destination
    [SerializeField] private GameObject _grappleHookPrefab = default; //The prefab for the grapple hook itself

    public void Grapple(InputAction.CallbackContext context)
    {
        if (GameUtility._isPlayerObjectBeingControlled && context.performed)
        {
            DoGrapple();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _playerCamera = Game_Manager.instance._playerCamera;
    }

    private void DoGrapple()
    {
        RaycastHit hit = default;
        //Raycast where the player is aiming
        if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out hit, _grappleRange))
        {
            //ensure the targetted point is above the player on the y-axis
            if (hit.point.y > transform.position.y)
            {
                //Grapple accordingly
                if (_currentGrappleType == GrappleType.Zip)
                    StartCoroutine(ZipGrapple(hit.point));
                else
                    Debug.Log("Swing grapple not yet implemented");
            }
            else
            {
                Debug.Log("Cannot grapple downwards");
            }
        }
    }

    IEnumerator ZipGrapple(Vector3 destination)
    {
        //Spawn in the grapple hook
        GameObject hook = Instantiate(_grappleHookPrefab, transform.position, Quaternion.identity);
        hook.transform.LookAt(destination);

        //Fire the hook at the destination
        for (float i = 0; i < 1.0f; i += (Time.deltaTime / _grappleFireTime))
        {
            hook.transform.position = Vector3.Lerp(hook.transform.position, destination, i);

            if(Vector3.Distance(hook.transform.position, destination) <= _minDistance/2.0f)
            {
                //breakout when its close enough to reduce the lag time where the player cant move
                break;
            }

            yield return null;
        }

        //disable traditional player movement
        PlayerMovement.instance.SetCanMoveOff();

        //Move the player to the destination
        for (float i = 0; i < 1.0f; i += (Time.deltaTime / _zipGrappleTime))
        {
            transform.position = Vector3.Lerp(transform.position, destination, i);
            
            if (Vector3.Distance(transform.position, destination) <= _minDistance)
            {
                //Disengage grapple and cleanup when close enough
                Destroy(hook);
                PlayerMovement.instance.SetCanMoveOn();
                break;
            }
            yield return null;
        }
    }
}
