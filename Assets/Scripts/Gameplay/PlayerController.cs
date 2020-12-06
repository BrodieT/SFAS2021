using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{


    #region InpuCallbacks

    //The following functions are used by the player input component 

    public void Attack(InputAction.CallbackContext context)
    {
        if (CanControl)
        {

        }
    }

    public void SkipDialogue(InputAction.CallbackContext context)
    {
        if (context.performed && !CanControl)
        {
            TextDisplay.Instance.QuickDisplay();
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (CanControl)
            {
                DoJump();
            }
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (CanControl)
        {
            DoInteract();
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (CanControl)
            MoveDirection = context.ReadValue<Vector2>();
    }

    public void Look(InputAction.CallbackContext context)
    {
        if (CanControl)
            LookDirection = context.ReadValue<Vector2>();
    }


    public void Sprint(InputAction.CallbackContext context)
    {
        if (CanControl)
        {
            if (context.ReadValueAsButton())
            {
                MoveSpeed = RunSpeed;
                BobSpeed = RunningBobSpeed;
            }
            else
            {
                MoveSpeed = WalkSpeed;
                BobSpeed = WalkingBobSpeed;
            }
        }
    }
    #endregion


    #region Singleton

    public static PlayerController Instance;
    private void Awake()
    {
        Instance = this;
    }

    #endregion


    [Header("Movement")]
    [SerializeField] private float WalkSpeed = 5.0f; //The movement speed while walking
    [SerializeField] private float RunSpeed = 10.0f; //The movement speed while running
    [SerializeField] private float JumpHeight = 0.75f; //The height the player can jump
    [SerializeField] private float Gravity = 10.0f; //The amount of gravity
    [SerializeField] private float MaxVelocityChange = 10.0f;


    [SerializeField] private LayerMask GroundMask = new LayerMask(); //Determines what is considered ground
    [SerializeField] private float GroundDetectionRange = 0.5f;

    [Header("Camera")]
    [SerializeField] private float LookSensitivity = 100.0f; //The look sensitivity of the player
    [SerializeField] private bool CanCameraBob = true; //Determines whether the camera bob will be used
    [SerializeField] private float WalkingBobSpeed = 14.0f; //How quickly the camera will bob when walking
    [SerializeField] private float RunningBobSpeed = 20.0f;  //How quickly the camera will bob when running
    [SerializeField] private float BobAmount = 0.15f; //The amount the camera will bob by

    private Vector2 MoveDirection = new Vector2(); //The direction the player is moving in
    private Vector2 LookDirection = new Vector2(); //The direction the player is looking at
    private float MoveSpeed = 0.0f; //The current movement speed of the player
    private Rigidbody Rb = default; //Local store of the player's rigidbody
    [SerializeField] private Camera PlayerCamera = default; //Local store of the main camera
    private Vector3 CameraRotation = new Vector3(); //The rotation of the camera
    private float BobSpeed = 0.0f; //The current speed of the camera bob
    float BobTimer = 0; //The timer used for the camera bob, used when lerping positions
    float DefaultCamPosY = 0; //Stores the default y-position of the camera to return to when bobbing
    private Vector3 Velocity = new Vector3();
    private bool IsGrounded = false;

    private Vector3 JumpOffset = Vector3.zero;
    private Vector3 OriginalSpawnPoint = new Vector3();
    private Quaternion OriginalSpawnRote = new Quaternion();

    Transform OriginalCameraTransform;


    [SerializeField] private GameObject TutorialLevel = default;

    public bool CanControl = false;

    // Start is called before the first frame update
    void Start()
    {

        DefaultCamPosY = PlayerCamera.transform.localPosition.y;
        Rb = GetComponent<Rigidbody>();

        OriginalSpawnPoint = transform.position;
        OriginalSpawnRote = transform.rotation;

        OriginalCameraTransform = PlayerCamera.transform;

        MoveSpeed = WalkSpeed;
        BobSpeed = WalkingBobSpeed;

        Rb.freezeRotation = true;
        Rb.useGravity = false;

        GameUtility.HideCursor();

        CustomEventSystem.BeginTutorial += StartTutorial;

        TutorialLevel.SetActive(false);

        if (InteractPrompt != null)
            InteractPrompt.gameObject.SetActive(false);

    }

    private void DoJump()
    {
        if (IsGrounded)
        {
            Rb.velocity = new Vector3(Velocity.x, Mathf.Sqrt(2 * JumpHeight * Gravity), Velocity.z) + JumpOffset;
            IsGrounded = false;
        }
    }
    RaycastHit TargetInteractable = default;
    [SerializeField] private LayerMask InteractLayer = default;
    [SerializeField] private float InteractDistance = 5.0f;
    [SerializeField] private GameObject LeftHand = default;
    [SerializeField] private GameObject RightHand = default;
    [SerializeField] private TMP_Text InteractPrompt = default;

    private void FixedUpdate()
    {
        if (CanControl)
        {
            Move();

            //Apply gravity
            Rb.AddForce(new Vector3(0, -Gravity * Rb.mass, 0));

            GroundCheck();


            if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out TargetInteractable, InteractDistance, InteractLayer))
            {
                if (InteractPrompt != null)
                {
                    InteractPrompt.gameObject.SetActive(true);

                    switch (TargetInteractable.transform.GetComponent<Interactable>().InteractableType)
                    {
                        case Interactable.InteractType.Revolver:
                            InteractPrompt.text = "Pick Up Revolver";
                            break;
                        case Interactable.InteractType.GrappleGun:
                            InteractPrompt.text = "Pick Up Grapple Gun";
                            break;
                        case Interactable.InteractType.Laptop:
                            InteractPrompt.text = "Use Terminal";
                            break;
                    }
                  
                }
            }
            else
            {
                if (InteractPrompt != null)
                {
                    InteractPrompt.gameObject.SetActive(false);
                }
            }
            
        }

        

        if(transform.position.y <= -50)
        {
            transform.position = OriginalSpawnPoint;
            transform.rotation = OriginalSpawnRote;
        }
    }

    private void StartTutorial()
    {
        TutorialLevel.SetActive(true);
    }

    private void DoInteract()
    {
        if (TargetInteractable.transform == null)
            return;

        switch(TargetInteractable.transform.GetComponent<Interactable>().InteractableType)
        {
            case Interactable.InteractType.Revolver:
                PickupGun(RightHand, TargetInteractable.transform.gameObject);
                break;
            case Interactable.InteractType.GrappleGun:
                PickupGun(LeftHand, TargetInteractable.transform.gameObject);
                break;
            case Interactable.InteractType.Laptop:
                CanControl = false;

                if (InteractPrompt != null)
                {
                    InteractPrompt.gameObject.SetActive(false);
                }


                for (int i = 0; i < TargetInteractable.transform.childCount; i++)
                {
                    if(TargetInteractable.transform.GetChild(i).CompareTag("CameraHolder"))
                    {
                        StartCoroutine(TranslateCamera(TargetInteractable.transform.GetChild(i).transform, 5.0f, true));
                    }
                }
                

                break;
        }
        
    }

    public void ResetCamera()
    {
        StartCoroutine(TranslateCamera(OriginalCameraTransform, 5.0f, false));

    }

    IEnumerator TranslateCamera(Transform targetTranslation, float time, bool isTerminal)
    {
        for(float i = 0; i < 1.0f; i += (Time.deltaTime / time))
        {
            PlayerCamera.transform.position = Vector3.Lerp(PlayerCamera.transform.position, targetTranslation.position, i);
            PlayerCamera.transform.LookAt(targetTranslation.position + targetTranslation.forward);

            yield return null;
        }


        if (isTerminal && TargetInteractable.transform != null)
        {

            DialogueManager dialogue;

            if (TargetInteractable.transform.TryGetComponent(out dialogue))
            {
                dialogue.StartDisplay();
            }
            else
            {
                Debug.LogError("No Dialogue Manager found on laptop interactable");
            }
        }
    }


    [SerializeField] private GameObject GunPrefab = default;

    public void PickupGun(GameObject GunHolder, GameObject PhysicalGun)
    {
        GameObject newGun = Instantiate(GunPrefab, GunHolder.transform);
        Destroy(PhysicalGun);
    }

    private void GroundCheck()
    {

        if (Physics.OverlapSphere(transform.position - (Vector3.up / 2), GroundDetectionRange + 0.1f, GroundMask).Length > 0)
        {
            IsGrounded = true;
        }
        else
        {
            IsGrounded = false;
        }
    }

    private void LateUpdate()
    {
        if (CanControl)
        {
            Turn();
            CameraBob();
        }
    }


    //private void OnCollisionEnter(Collision collision)
    //{
    //    //The dot product of the collision contact normal and the world up vector will return
    //    //0 if the two are perpendicular, and 1 if they are the same
    //    //Using the absolute value will determine generally whether the surface is horizontal or vertical (floor or wall)
    //    if (Mathf.Abs(Vector3.Dot(collision.contacts[0].normal, Vector3.up)) < 1)
    //    {

    //    }
    //}

    void CameraBob()
    {
        if (CanCameraBob)
        {
            if (MoveDirection != Vector2.zero)
            {
                BobTimer += Time.deltaTime * WalkingBobSpeed;
                PlayerCamera.transform.localPosition = new Vector3(PlayerCamera.transform.localPosition.x, DefaultCamPosY + Mathf.Sin(BobTimer) * BobAmount, PlayerCamera.transform.localPosition.z);
            }
            else
            {
                BobTimer = 0;
                PlayerCamera.transform.localPosition = new Vector3(PlayerCamera.transform.localPosition.x, Mathf.Lerp(PlayerCamera.transform.localPosition.y, DefaultCamPosY, Time.deltaTime * WalkingBobSpeed), PlayerCamera.transform.localPosition.z);
            }
        }
    }

    void Turn()
    {
        LookDirection *= LookSensitivity * Time.deltaTime;

        //Update the x rotation of the camera based on the new Look Direction
        CameraRotation.x -= LookDirection.y;
        CameraRotation.y += LookDirection.x;
        CameraRotation.z = PlayerCamera.transform.localRotation.eulerAngles.z;

        //Clamp to prevent full 360 rotation around the x-axis
        CameraRotation.x = Mathf.Clamp(CameraRotation.x, -90, 90);

        //Update the camera's rotation
        PlayerCamera.transform.localRotation = Quaternion.AngleAxis(CameraRotation.x, Vector3.right);
        transform.localRotation = Quaternion.AngleAxis(CameraRotation.y, Vector3.up);

    }
    void Move()
    {
        // Create a Vector3 for the 3D move direction, making use of the inputs in relation to the transform
        Vector3 Move = (transform.right * MoveDirection.x) + (transform.forward * MoveDirection.y).normalized;
        Move *= MoveSpeed;

        Velocity = Rb.velocity;
        Vector3 VelocityChange = (Move - Velocity);
        VelocityChange.x = Mathf.Clamp(VelocityChange.x, -MaxVelocityChange, MaxVelocityChange);
        VelocityChange.z = Mathf.Clamp(VelocityChange.z, -MaxVelocityChange, MaxVelocityChange);
        VelocityChange.y = 0;

        Rb.AddForce(VelocityChange, ForceMode.VelocityChange);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position - (Vector3.up / 2), GroundDetectionRange + 0.1f);
        Gizmos.DrawLine(PlayerCamera.transform.position, PlayerCamera.transform.position + (PlayerCamera.transform.forward * InteractDistance));
    }
}
