using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class MouseLook : MonoBehaviour
{ 
    //The player parent object being rotated when looking horizontally
    [SerializeField] private Transform _playerBody = default;
    //The raw look direction based on player input
    private Vector2 _lookDirection = new Vector2();
    //The sensitivity of the mouse for camera rotation 
    [SerializeField] private float _mouseSensitivity = 100.0f;

    [Header("Audio")]
    [SerializeField] List<AudioClip> _footstepSounds = new List<AudioClip>();

    //The vertical look rotation to be clamped
    private float _xRotation = 0.0f;
    //Local store of the player movement component, used for a camera bob
    private PlayerMovement _playerMovement = default;
    //The local position of the camera at the start of the game
    private Vector3 _defaultPosition = new Vector3();
    
    //Determines whether the player can camera bob
    private bool _canCameraBob = true;
    //The speed of the camera bob when walking
    private float _walkBobSpeed = 12.0f;
    //The speed of the camera bob when running
    private float _runBobSpeed = 24.0f;
    //The amount of bobbing the camera will do (how extreme are the movements)
    private float _bobAmount = 0.15f;
    
    //The timer used when lerping the camera for the bob
    private float _bobTimer = 0.0f;

    //Input callback for accessing the look direction
    public void Look(InputAction.CallbackContext context)
    {
        _lookDirection = context.ReadValue<Vector2>();
    }

   
    // Start is called before the first frame update
    void Start()
    {

        //Hide the cursor using the game utility static script
        GameUtility.HideCursor();


        //If the camera bob is supposed to occur
        if (_canCameraBob)
        {
            //Store the default position of the camera
            _defaultPosition = transform.localPosition;

            //Attempt to acquire the player movement component
            //If unsuccessful the camera bob will not proceed
            if (!_playerBody.TryGetComponent<PlayerMovement>(out _playerMovement))
            {
                Debug.LogError("Player Movement component not found, unable to perform camera bob");
                _canCameraBob = false;
            }
        }

    }

   
    // Update is called once per frame
    void Update()
    {
        if (!GameUtility._isPaused && GameUtility._isPlayerObjectBeingControlled)
        {
            //Multiply the raw look direction vector with a sensitivity parameter and delta time
            Vector2 look = _lookDirection * _mouseSensitivity * Time.deltaTime;

            //Increment the rotation around the x-axis (vertical look) & clamp values to prevent full 360
            _xRotation -= look.y;
            _xRotation = Mathf.Clamp(_xRotation, -90.0f, 90.0f);

            //Rotate only the camera object around the x-axis (vertical look) 
            transform.localRotation = Quaternion.Euler(_xRotation, 0.0f, transform.localEulerAngles.z);
            //Rotate the entire player around the y-axis (horizontal look)
            _playerBody.Rotate(Vector3.up, look.x);

            //Perform the camera bob if applicable
            DoCameraBob();
        }
    }
    
    //This function handles the camera bob 
    void DoCameraBob()
    {
        if(_canCameraBob)
        {
            //If the player is moving bob the camera
            //otherwise, return to it's default position
            if (_playerMovement.IsMoving())
            {
                if (!_isPlayingAudio)
                    StartCoroutine(PlayFootsteps());

                //using delta time, increment the bob timer with the appropriate speed depending on whether the player is sprinting or not
                //This results in a larger bob when moving faster
                if (_playerMovement.IsSprinting())
                    _bobTimer += Time.deltaTime * _runBobSpeed;
                else
                    _bobTimer += Time.deltaTime * _walkBobSpeed;
                
                //Lerp the camera position using the bob timer
                transform.localPosition = new Vector3(transform.localPosition.x, _defaultPosition.y + Mathf.Sin(_bobTimer) * _bobAmount, transform.localPosition.z);
            }
            else
            {
                //Reset the bob timer and lerp camera back to the default position
                _bobTimer = 0;
                transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, _defaultPosition.y, Time.deltaTime * _walkBobSpeed), transform.localPosition.z);
            }
        }
    }

    private bool _isPlayingAudio = false;
    IEnumerator PlayFootsteps()
    {    
        _isPlayingAudio = true;

        if (_footstepSounds.Count > 0)
        {
            AudioSource source = GetComponent<AudioSource>();

            if (source != null)
            {
                while (_playerMovement.IsMoving())
                {
                    if (_playerMovement.IsSprinting())
                        yield return new WaitForSeconds(0.25f);
                    else
                        yield return new WaitForSeconds(0.5f);

                    source.PlayOneShot(_footstepSounds[Random.Range(0, _footstepSounds.Count)]);
                }
            }
        }
        _isPlayingAudio = false;
    }


    
}
