using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    #endregion

    #region Assignable In Inspector Attributes

    //The normal movement speed
    [SerializeField] float _walkSpeed = 12.0f;
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
    //Determines what objects the player can vault over
    [SerializeField] private LayerMask _vaultMask = default;

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
        if(context.performed)
            _isJumping = true;

    }

    //Input callback for determining if the sprint button is pressed
    public void Sprint(InputAction.CallbackContext context)
    {
        _isSprinting = context.ReadValueAsButton();   
    }

    //Input callback for when the jump button is pressed
    public void Crouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _controller.height = _crouchHeight;



            if (_currentSpeed > _walkSpeed)
            {
                Debug.Log(_currentSpeed);
                Debug.Log("Slide");
                GetComponent<ForceReceiver>().AddForce(transform.forward, 100.0f);
            }
        }
        else
        {
            _controller.height = _defaultHeight;
        }
    }


    #endregion


    #region Singleton

    public static GameObject PlayerInstance;

    private void Awake()
    {
        PlayerInstance = gameObject;
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //Get the character controller component
        _controller = GetComponent<CharacterController>();
        _defaultHeight = _controller.height;
        _crouchHeight = _defaultHeight * 0.5f;
        _currentSpeed = _walkSpeed;
    }
    private void UpdateMoveSpeed()
    {
        if(_isSprinting && _currentSpeed != _runSpeed)
        {
            if (Mathf.Abs(_currentSpeed - _runSpeed) < 0.5f)
                _currentSpeed = _runSpeed;
            else
                _currentSpeed = Mathf.Lerp(_currentSpeed, _runSpeed, Time.deltaTime);
        }
        else if(_currentSpeed != _walkSpeed)
        {
            if (Mathf.Abs(_currentSpeed - _walkSpeed) < 1.0f)
                _currentSpeed = _walkSpeed;
            else
                _currentSpeed = Mathf.Lerp(_currentSpeed, _walkSpeed, Time.deltaTime);
        }
    }
   
    // Update is called once per frame
    void Update()
    {
        UpdateMoveSpeed();

        //Check if the player is touching the ground
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);

        //if the player is grounded, clamp the vertical velocity
        if (_isGrounded && _velocity.y < 0)
            _velocity.y = -2.0f;

        if (_isJumping)
        {
            if (DetectVaultableObject(1.0f, _vaultMask))
            {
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

       
        _controller.Move(move * _currentSpeed * Time.deltaTime);

        //Apply gravity * delta time squared
        _velocity.y += _gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);

        
    }

    private void MantleLedge()
    {

    }

    private void VaultObject()
    {
        Debug.Log("Vault over object");
        //if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1.0f, _vaultMask))
        //{
        //   // transform.position = hit.transform.position + Vector3.up;
        //}
        GetComponent<Animator>().SetTrigger("Vault");

      
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
                return true;
            }
        }
        return false;
    }


}
