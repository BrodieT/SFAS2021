using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent()]
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    #region Private Attributes

    //The raw movement direction based on player input
    private Vector2 _moveDirection = new Vector2();
    //Local store of the character controller component
    private CharacterController _controller = default;
    //The velocity of the player (gravity/jump)
    private Vector3 _velocity = new Vector3();
    //Tracks whether the player is touching the ground
    private bool _isGrounded = false;
    //How high the player can jump
    private float _jumpHeight = 3.0f;
    //Tracks whether the player is sprinting or not
    private bool _isSprinting = false;

    //Tracks whether the player is jumping
    private bool _isJumping = false;

    //The height of the player when standing
    private float _defaultHeight = 0.0f;
    //The height of the player when crouching
    private float _crouchHeight = 0.0f;

    //The current movement speed
    private float _currentSpeed = 0.0f;

    //Determines whether the player can move
    //This is used when mantling or vaulting to overwrite the default movement behaviours
    private bool _canMove = true;

    private WallRun _wallRunner = default;

    //Tracks whether a vault is in progress
    private bool _isVaulting = false;
    //Tracks whether a mantle is in progress
    private bool _isMantling = false;
    public bool _recieveGravity { get; set; }
    //The players gun controller used for updating animations
    private PlayerGunController _playerGunController = default;
    #endregion

    #region Assignable In Inspector Attributes

    [Header("Player Movement Settings")]
    //The normal movement speed
    [SerializeField] public float _walkSpeed = 12.0f;
    //The faster movement speed
    [SerializeField] float _runSpeed = 24.0f;
    //How much downward velocity the player is subject to due to gravity
    [SerializeField] private float _gravity = -9.8f;
    //The position of the player's feet, used to determine whether they are touching the ground
    [SerializeField] private Transform _groundCheck = default;
    //How large the ground check radius is
    [SerializeField] private float _groundDistance = 0.4f;
    //Determines what is considered ground
    [SerializeField] private LayerMask _groundMask = default;

    [Header("Advanced Player Movement Settings")]
    //Determines what objects the player can vault over
    [SerializeField] private LayerMask _vaultMask = default;
    //The forces applied when vaulting an obstacle
    [SerializeField] private float _vaultUpForce = 100.0f;
    [SerializeField] private float _vaultForwardForce = 100.0f;
    //The forces applied when mantling up a ledge
    [SerializeField] private float _mantleUpForce = 20.0f;
    [SerializeField] private float _mantleForwardForce = 5.0f;
    #endregion

    #region InputCallbacks

    //Input callback for accessing the move direction from the input
    public void Move(InputAction.CallbackContext context)
    {
        _moveDirection = context.ReadValue<Vector2>();
    }

    //Input callback for when the jump button is pressed
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && !GameUtility._isPaused)
        {
            if (_wallRunner._isWallRunning)
                _wallRunner.AddJumpForce();
            else
                _isJumping = true;

        }
    }

    //Input callback for determining if the sprint button is pressed
    public void Sprint(InputAction.CallbackContext context)
    {
        _isSprinting = context.ReadValueAsButton();   
    }

    //Input callback for when the jump button is pressed
    public void Crouch(InputAction.CallbackContext context)
    {
        if (!GameUtility._isPaused)
        {
            if (context.performed)
            {
                //Change the height of the character controller to the crouch height
                _controller.height = _crouchHeight;


                //If the player is moving faster than a run, add forward force to slide
                if (_currentSpeed > _walkSpeed)
                {
                    GetComponent<ForceReceiver>().AddForce(transform.forward, 100.0f);
                }
            }
            else
            {
                //Return the character controller height to the standing height
                _controller.height = _defaultHeight;
            }
        }

        //Adjust the ground check position according to the new height
        //_groundCheck.position = transform.position - (transform.up * (_controller.height / 2));
    }


    #endregion

    public bool GetGrounded()
    {
        return _isGrounded;
    }
    
    public Vector3 GetVelocity()
    {
        return _velocity;
    }

    public void SetVelocity(Vector3 vel)
    {
        _velocity = vel;
    }
   
   

    // Start is called before the first frame update
    void Start()
    {
        //Get the character controller component
        _controller = GetComponent<CharacterController>();
        //Store and calculate the appropriate character heights
        _defaultHeight = _controller.height;
        _crouchHeight = _defaultHeight * 0.5f;
        //Set the default speed to the walk speed
        _currentSpeed = _walkSpeed;
        _wallRunner = GetComponent<WallRun>();
        _playerGunController = GetComponent<PlayerGunController>();
        _recieveGravity = true;
    }

    //This function interpolates the current move speed based on whether the player is running or walking
    private void UpdateMoveSpeed()
    {
        //if the player is sprinting and not at top speed yet
        if(_isSprinting && _currentSpeed != _runSpeed)
        {
            //If the current speed is close enough to the run speed, assign to the run speed
            //Otherwise continue interpolation
            if (Mathf.Abs(_currentSpeed - _runSpeed) < 0.5f)
                _currentSpeed = _runSpeed;
            else
                _currentSpeed = Mathf.Lerp(_currentSpeed, _runSpeed, Time.deltaTime);
        }
        else if(_currentSpeed != _walkSpeed) //If the player is not sprinting, and still moving faster than a walk
        {
            //If the current speed is close enough to the walk speed, assign to the walk speed
            //Otherwise continue interpolation
            if (Mathf.Abs(_currentSpeed - _walkSpeed) < 1.0f)
                _currentSpeed = _walkSpeed;
            else
                _currentSpeed = Mathf.Lerp(_currentSpeed, _walkSpeed, Time.deltaTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameUtility._isPaused && GameUtility._isPlayerObjectBeingControlled)
        {
            //Lerp movement speeds
            UpdateMoveSpeed();

            //Check if the player is touching the ground
            //_isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);
            _isGrounded = Physics.CheckCapsule(_groundCheck.transform.position, _groundCheck.transform.position + (_groundCheck.transform.up * (_defaultHeight / 2)), _groundDistance, _groundMask);

            //if the player is grounded, clamp the vertical velocity
            if (_isGrounded && _velocity.y < 0)
                _velocity.y = -2.0f;

            //Handle jumping if the jump key is pressed
            if (_isJumping && !_isVaulting)
            {
                //if there is something in front of the player that can be vaulted over
                //otherwise perform normal jump if grounded
                if (DetectVaultableObject(1.0f, _vaultMask))
                {
                    _isVaulting = true;

                    if (_isGrounded)
                        VaultObject();
                }
                else if (_isGrounded)
                {
                    //If grounded, increase vertical velocity by square root of the jump height * -2 * gravity
                    _velocity.y = Mathf.Sqrt(_jumpHeight * -2.0f * _gravity);
                }
                _isJumping = false;
            }



            //Calculate the localised move direction
            Vector3 move = (transform.right * _moveDirection.x) + (transform.forward * _moveDirection.y);

            if (!_isGrounded && !_isVaulting && !_isMantling)
            {
                if (CanMantle())
                {
                    _isMantling = true;
                    //MantleLedge();
                }
            }


            if (_canMove)
            {
                _controller.Move(move * _currentSpeed * Time.deltaTime);
            }

            if (_recieveGravity)
            {
                //Apply gravity * delta time squared
                _velocity.y += _gravity * Time.deltaTime;
            }
            _controller.Move(_velocity * Time.deltaTime);

            if (_moveDirection.magnitude > 0)
            {
                if (_isSprinting)
                {
                    _playerGunController._isWalking = false;
                    _playerGunController._isRunning = true;
                }
                else
                {
                    _playerGunController._isWalking = true;
                    _playerGunController._isRunning = false;
                }
            }
            else
            {
                _playerGunController._isWalking = false;
                _playerGunController._isRunning = false;
            }
        }
    }

    private void MantleLedge()
    {
        _canMove = false;
        GetComponent<ForceReceiver>().AddForce(Vector3.up, _mantleUpForce);
        GetComponent<ForceReceiver>().AddForce(transform.forward, _mantleForwardForce);
        Invoke("SetCanMoveOn", 0.5f);
    }

    private bool CanMantle()
    {

        return DetectVaultableObject(1.0f, LayerMask.GetMask("Ground", "Vaultable"));
    }

    private void VaultObject()
    {
        _canMove = false;
        GetComponent<ForceReceiver>().AddForce(Vector3.up, _vaultUpForce);
        GetComponent<ForceReceiver>().AddForce(transform.forward, _vaultForwardForce);
        Invoke("SetCanMoveOn", 0.5f);
    }

    public void SetCanMoveOn()
    {
        _isVaulting = false;
        _isMantling = false;
        _canMove = true;
    }

    public void SetCanMoveOff()
    {
        _isVaulting = false;
        _isMantling = false;
        _canMove = false;
    }

    //This function returns true if the player is moving
    public bool IsMoving()
    {
        if (_moveDirection != Vector2.zero)
            return true;

        return false;
    }

    //This function returns whether the player is sprinting
    public bool IsSprinting()
    {
        return _isSprinting;
    }
    
    //This function will detect a surface in front of the player that can be vaulted
    public bool DetectVaultableObject(float range, LayerMask layer)
    {
        //Create a capsule about the player size when crouched
        Vector3 top = transform.position + (transform.forward * 0.25f);
        Vector3 bottom = top - (transform.up * _crouchHeight);

       //Check if there is an object in front of the player
        if( Physics.CapsuleCastAll(top, bottom, 0.25f, transform.forward, range, layer).Length >= 1)
        {
            Ray ray = new Ray((transform.position - (transform.up * (_controller.height/2))) + (transform.up * (_defaultHeight * 0.65f)), transform.forward);
            Debug.DrawRay(ray.origin, ray.direction, Color.red, 3.0f);
            //check that the player can see over the object (ie nothing would block the vault)
            if(!Physics.Raycast(ray, 3.0f))
            {
                //Ensure the player is moving into the ledge rather than away from it
                if (_moveDirection.y > 0)
                    return true;
            }
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
    }
}
