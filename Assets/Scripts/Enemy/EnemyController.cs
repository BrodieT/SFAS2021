using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
public class EnemyController : MonoBehaviour
{
    //Tracks the current state and behaviours of the enemy
    public enum EnemyState { Idle = 0, Suspicious = 1, Hostile = 2}
    private EnemyState _currentState = 0;

    public float _playerDistance { get; set; }


   

    private RaycastHit _target = default;
    public float _rangedTimer { get; set; }

    private NavMeshAgent _agent = default;
    private CharacterController _controller = default;
  

    private Vector3 _velocity = new Vector3();

    private bool _isGrounded = false;

    [Header("Enemy Movement Parameters")]
    //How much downward velocity the player is subject to due to gravity
    [SerializeField] private float _gravity = -9.8f;
    //The position of the player's feet, used to determine whether they are touching the ground
    [SerializeField] private Transform _groundCheck = default;
    //How large the ground check radius is
    [SerializeField] private float _groundDistance = 0.4f;
    //Determines what is considered ground
    [SerializeField] private LayerMask _groundMask = default;
    [SerializeField] private float _moveSpeed = 2.0f;
    [SerializeField] public float _turnSpeed = 3.0f;
    [Header("Enemy Combat Parameters")]
    [SerializeField] public float _shootingRange = 10.0f;
    [SerializeField] public float _detectionRange = 20.0f;
    [SerializeField] public float _rangedAttackCooldown = 2.0f;
    [SerializeField] private LayerMask _playerMask = default;
    [Header("Enemy Search Parameters")]
    private float _searchTimer = 0.0f;
    [SerializeField] private float _timeSearching = 5.0f;

   

    // Start is called before the first frame update
    public virtual void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _controller = GetComponent<CharacterController>();

        _agent.updatePosition = false;
        _agent.updateRotation = false;


        

    }

    public bool DetectPlayer()
    {
        if(_playerDistance < _detectionRange && Physics.Raycast(transform.position + transform.forward, (PlayerMovement.instance.transform.position - transform.position).normalized, out _target, _detectionRange, _playerMask))
        {
            if(_target.transform.gameObject == PlayerMovement.instance.gameObject)
                return true;
        }

        return false;
    }

   

   

    public virtual void Patrol()
    {
    }

  

    public virtual void Search()
    {
        _searchTimer -= Time.deltaTime;       
    }

    public virtual void Attack()
    {
        _rangedTimer -= Time.deltaTime;

        Vector3 targetPoint = PlayerMovement.instance.transform.position - transform.position;
        Quaternion targetRote = Quaternion.LookRotation(targetPoint, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRote, Time.deltaTime * _turnSpeed);

    }

    // Update is called once per frame
    public virtual void Update()
    {
        //Check if the enemy is touching the ground
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);

        //if the player is grounded, clamp the vertical velocity
        if (_isGrounded && _velocity.y < 0)
            _velocity.y = -2.0f;
           
        
        _playerDistance = Vector3.Distance(transform.position, PlayerMovement.instance.transform.position);


        if (DetectPlayer())
        { 
            _currentState = EnemyState.Hostile;
        }



        switch(_currentState)
        {
            case EnemyState.Idle:
                if (DetectPlayer())
                {
                    _currentState = EnemyState.Hostile;
                    break;
                }

                Patrol();
                break;
            case EnemyState.Suspicious:
                if (_searchTimer < 0.0f)
                {
                    _currentState = EnemyState.Idle;
                    break;
                }      
                
                Search();

                break;
            case EnemyState.Hostile:

                if (!DetectPlayer())
                {
                    _searchTimer = _timeSearching;
                    _currentState = EnemyState.Suspicious;
                    break;
                }
                Attack();
                break;
        }

        //Apply gravity * delta time squared
        _velocity.y += _gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);


    }
    
  
    public void EnemyMove(Vector3 destination)
    {
        Vector3 targetPoint = destination - transform.position;
        Quaternion targetRote = Quaternion.LookRotation(targetPoint, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRote, Time.deltaTime * _turnSpeed);

        _agent.SetDestination(destination);
        _controller.Move(_agent.desiredVelocity.normalized * _moveSpeed * Time.deltaTime);
        _agent.velocity = _controller.velocity;
    }

   


}
