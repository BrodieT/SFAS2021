using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    [SerializeField] LayerMask _runnableSurfaces = default;
    [SerializeField] float _wallDetectionRange = 2.0f;
    [SerializeField, Range(0, 360)] float _cameraTiltAngle = 25.0f;
    [SerializeField, Min(0.0f)] float _tiltTime = 3.0f;
    [SerializeField] float _pushOffForce = 10.0f; //How much force will be applied when pushing off of wall
    Vector3 _pushOffDirection = new Vector3(); //The direction the push off force will be applied in (normal to the wall)
    [SerializeField] float _upwardBiasForce = 0.0f; //How much of an upward force will be applied when pushing off from wall
    [SerializeField] float _wallRunDuration = 5.0f;
    private PlayerMovement _movement = default;
    private Camera _playerCamera = default;
    public bool _isWallRunning { get; private set; }
    RaycastHit wall = default;
    private void Start()
    {
        _movement = Game_Manager.instance._player.GetComponent<PlayerMovement>();
        _playerCamera = Game_Manager.instance._playerCamera;
    }

    private void StopWallRun()
    {
        Debug.Log("Times up");
        _movement._recieveGravity = true;
    }

    private void FixedUpdate()
    {
        if (_isWallRunning)
        {
            if (_movement.IsMoving())
            {
                _movement._recieveGravity = false;
            }
            else
            {
                _movement._recieveGravity = true;
            }
        }



        if (Physics.Raycast(transform.position, transform.right, out wall, _wallDetectionRange, _runnableSurfaces) && !_movement.GetGrounded())
        {
            if (!_isWallRunning)
            {
                _pushOffDirection = wall.normal;
                _isWallRunning = true;
                _movement.SetVelocity(new Vector3(_movement.GetVelocity().x, 0.0f, _movement.GetVelocity().z));
                StopAllCoroutines();
                StartCoroutine(RotateCamera(_cameraTiltAngle, Vector3.forward, _tiltTime));
                _movement._recieveGravity = false;
                
                if (IsInvoking("StopWallRun"))
                {
                    CancelInvoke("StopWallRun");
                }

                Invoke("StopWallRun", _wallRunDuration);
            }
        }
        else if (Physics.Raycast(transform.position, -transform.right, out wall, _wallDetectionRange, _runnableSurfaces) && !_movement.GetGrounded())
        {
            if (!_isWallRunning)
            {
                _pushOffDirection = wall.normal;

                _isWallRunning = true;
                _movement.SetVelocity(new Vector3(_movement.GetVelocity().x, 0.0f, _movement.GetVelocity().z));
                StopAllCoroutines();
                StartCoroutine(RotateCamera(-_cameraTiltAngle, Vector3.forward, _tiltTime));
                _movement._recieveGravity = false;

                if (IsInvoking("StopWallRun"))
                {
                    CancelInvoke("StopWallRun");
                }
                Invoke("StopWallRun", _wallRunDuration);
            }
        }
        else
        {
            if (_isWallRunning)
            {
                _isWallRunning = false;
                StopAllCoroutines();
                StartCoroutine(RotateCamera(0.0f, Vector3.forward, _tiltTime)); 
                _movement._recieveGravity = true;
            }
        }
    }

    public void AddJumpForce()
    {
        _isWallRunning = false;
        GetComponent<ForceReceiver>().AddForce(_pushOffDirection , _pushOffForce);
        GetComponent<ForceReceiver>().AddForce(Vector3.up , _upwardBiasForce);
    }

    IEnumerator RotateCamera(float angle, Vector3 axis, float time)
    {
        for (float i = 0.0f; i < 1.0f; i += (Time.deltaTime / time))
        {
            Vector3 cameraRote = _playerCamera.transform.localEulerAngles;

            if (axis.x != 0)
                cameraRote.x = Mathf.LerpAngle(cameraRote.x, angle, i);

            if (axis.y != 0)
                cameraRote.y = Mathf.LerpAngle(cameraRote.y, angle, i);
           
            if (axis.z != 0)
                cameraRote.z = Mathf.LerpAngle(cameraRote.z, angle, i);



            _playerCamera.transform.localEulerAngles = cameraRote;

            yield return null;
        }
    }
}

