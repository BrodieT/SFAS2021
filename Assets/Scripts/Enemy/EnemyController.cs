using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;


//This class represents all enemies in the game and features all common functionality across enemy types

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    public float _playerDistance { get; set; }    //How far away the player is from the enemy
    public float _rangedTimer { get; set; }    //The countdown timer between ranged attacks
    public enum EnemyState { Idle = 0, Suspicious = 1, Hostile = 2} //The different behaviour states of the enemy
    private EnemyState _currentState = 0;     //Tracks the current state of the enemy
    private RaycastHit _target = default;  //The target of the raycast used to determine line of sight to the player
    [HideInInspector] public NavMeshAgent _agent = default; //local cache of navmesh agent component
    [HideInInspector] public CharacterController _controller = default; //local cache of character controller component
    private Vector3 _velocity = new Vector3(); //The movement velocity of the enemy
    private bool _isGrounded = false; //Tracks if the enemy is grounded
    private float _searchTimer = 0.0f; //Tracks how long the enemy has been searching for in the suspicious state 
    [HideInInspector] public Transform _player = default; //Local cache of the player transform

    [Header("Enemy Movement Parameters")]
    [SerializeField] private float _gravity = -9.8f;    //How much downward velocity the player is subject to due to gravity
    [SerializeField] private Transform _groundCheck = default;    //The position of the player's feet, used to determine whether they are touching the ground
    [SerializeField] private float _groundDistance = 0.4f;    //How large the ground check radius is
    [SerializeField] private LayerMask _groundMask = default;    //Determines what is considered ground
    [SerializeField] private float _moveSpeed = 2.0f; //The speed of normal enemy movement
    [SerializeField] public float _turnSpeed = 3.0f; //How quickly the enemy can turn in the direction of travel/attack
    
    [Header("Enemy Combat Parameters")]
    [SerializeField] public float _shootingRange = 10.0f; //How far the enemy can shoot
    [SerializeField] public float _detectionRange = 20.0f; //how far the enemy can detect the player from
    [SerializeField] public float _rangedAttackCooldown = 2.0f; //How many seconds between ranged attacks
    [SerializeField] private LayerMask _playerMask = default; //Determines what is considered the  player

    [Header("Enemy Search Parameters")]
    [SerializeField] private float _timeSearching = 5.0f; //How long the enemy will search for when suspicious

    // Start is called before the first frame update
    public virtual void Start()
    {
        //Find the agent and controller components
        _agent = GetComponent<NavMeshAgent>();
        _controller = GetComponent<CharacterController>();

        //As with the player, movement will be handled manually using the character controller
        //therefore we dont want the nav mesh agent to update anything automatically
        _agent.updatePosition = false;
        _agent.updateRotation = false;
       
        //Get the player transform from the game manager
        _player = Game_Manager.instance._player.transform;
    }

    //This function determines if the enemy can see the player
    public bool DetectPlayer()
    {
        //If the player is in range and the raycast can hit them (ie nothing blocking line of sight) return true
        if(_playerDistance < _detectionRange && Physics.Raycast(transform.position + transform.forward, (_player.position - transform.position).normalized, out _target, _detectionRange, _playerMask))
        {
            if(_target.transform.gameObject == _player.gameObject)
                return true;
        }

        return false;
    }

    //Virtual empty patrol function to be overriden where appropriate by child enemies
    public virtual void Patrol()
    {
    }
      
    //Virtual search function used only to decrement search timer. Children will override this and add additional functionality where appropriate
    public virtual void Search()
    {
        _searchTimer -= Time.deltaTime;       
    }

    //Virtual attack function used only to decremtn ranged timer. Children will override this and add additional functionality where appropriate
    public virtual void Attack()
    {
        _rangedTimer -= Time.deltaTime;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        //Check if the enemy is touching the ground
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);

        //if the enemy is grounded, clamp the vertical velocity
        if (_isGrounded && _velocity.y < 0)
            _velocity.y = -2.0f;
           
        //Update the distance between the player and the enemy
        _playerDistance = Vector3.Distance(transform.position, _player.position);

      


        //FSM for determining the rest of the enemy behaviours
        switch(_currentState)
        {
            case EnemyState.Idle:
                //Move to the hostile state if the enemy can detect the player
                if (DetectPlayer())
                {
                    _currentState = EnemyState.Hostile;
                    break;
                }

                //When idle call the patrol function
                Patrol();
                break;
            case EnemyState.Suspicious:
                //Move to the hostile state if the enemy can detect the player
                if (DetectPlayer())
                {
                    _currentState = EnemyState.Hostile;
                    break;
                }

                //If the search timer has elapsed return to idle
                if (_searchTimer < 0.0f)
                {
                    _currentState = EnemyState.Idle;
                    break;
                }      
                
                //Call the search function when suspicious
                Search();
                break;
            case EnemyState.Hostile:
                //If the player can no longer be seen move to the suspicious state & reset the search timer
                if (!DetectPlayer())
                {
                    _searchTimer = _timeSearching;
                    _currentState = EnemyState.Suspicious;
                    break;
                }

                //Call the attack function when hostile
                Attack();
                break;
        }

        //Apply gravity * delta time squared
        _velocity.y += _gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }
    
    //This function handles the enemy movement
    public void EnemyMove(Vector3 destination)
    {
        //Update the rotation of the enemy to face the direction of travel
        Quaternion targetRote = Quaternion.LookRotation((transform.position - destination).normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRote, Time.deltaTime * _turnSpeed);

        //Update the navmesh agent and character controller accordingly to move in the desired direction
        _agent.SetDestination(destination);
        _controller.Move(_agent.desiredVelocity.normalized * _moveSpeed * Time.deltaTime);
        _agent.velocity = _controller.velocity;
    }
}
