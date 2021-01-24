using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script controls the charger enemy type
//It contains the same functionality as its parent enemy controller class with some additional unique functionality
//The purpose of this enemy type is to charge directly at the player and cause damage when ramming into them
public class ChargerController : EnemyController
{
    [Header("Charge Settings")]
    [SerializeField] private float _chargeDistance = 20.0f; //The maximum distance the enemy will charge for
    [SerializeField] private float _attackCooldown = 5.0f; //The cooldown between charges where the enemy is vulnerable
    [SerializeField] private float _chargeSpeed = 5.0f; //The movement speed when charging (different from normal move speed)
    [SerializeField] private LayerMask _chargeDetectionMask = default; //Layermask to exclude the player when determining if a charge destination is reachable
    [SerializeField] private float _knockbackForce = 100.0f; //The force applied to the player if they are hit
    [SerializeField] private int _attackDamage = 20; //The damage inflicted by a successful charge

    [Header("Idle Settings")]
    [SerializeField] private List<Transform> _patrolPoints = new List<Transform>(); //A list of points the enemy can patrol through when idle
    [SerializeField] private float _patrolWaitTime = 3.0f; //How long the enemy will wait at a patrol point before moving on

    private int _currentPatrolPoint = 0; //The currently targetted patrol point
    private Animator _animator = default; //Local cache of the animator
    private Vector3 targetPoint = new Vector3(); //the resulting position of the projected charge
    private Quaternion targetRote = Quaternion.identity; //the desired rotation of the enemy while charging
    private bool _isCharging = false; //Tracks if a charge is in progress
    private bool _canCharge = true; //tracks if a new charge can be initiated
    
    public override void Start()
    {
        base.Start();
        //Locate the animator component on the enemy graphic
        _animator = GetComponentInChildren<Animator>();
    }

    //This function overrides the parent class' attack function and handles the charge attack
    public override void Attack()
    {
        //if the cooldown has elapsed and not currently charging, then initiate a new charge
        if (!_isCharging && _canCharge)
        {
            StartCharge();
        }

        //Move the enemy according to the charge destination
        DoCharge();
    }

    //This function overrides the parent class' patrol function and handles the moving between patrol points when idle
    public override void Patrol()
    {
        base.Patrol();

        if (_patrolPoints.Count == 0)
            return;

        //If the enemy is close enough to the destination
        if (Vector3.Distance(transform.position, _patrolPoints[_currentPatrolPoint].position) < _agent.stoppingDistance)
        {
            //Stop the walk animation
            _animator.SetBool("Idle", true);
            //stop the agent from moving away from the character
            _agent.SetDestination(transform.position);

            //Wait then find a new patrol point from the list
            if (!IsInvoking("FindNewPatrolPoint"))
                Invoke("FindNewPatrolPoint", _patrolWaitTime);
        }
        else
        {
            //Begin the walk animation
            _animator.SetBool("Idle", false);
            //Continue to move towards the destination if not close enough
            EnemyMove(_patrolPoints[_currentPatrolPoint].position);
        }
    }

    //This function is invoked when the enemy is at a patrol point and is responsible for selecting a new target
    private void FindNewPatrolPoint()
    {
        //Increment the current patrol index
        _currentPatrolPoint++;
        //If out of bounds, loop back to start
        if (_currentPatrolPoint >= _patrolPoints.Count)
        {
            _currentPatrolPoint = 0;
        }
    }

    //This function initiates a charge attack
    private void StartCharge()
    {   
        //Begin the walk animation
        _animator.SetBool("Idle", false);


        //Ensure that the target position is on the same y-position
        Vector3 playerPos = _player.position;
        playerPos.y = transform.position.y;

        //set the target point of the charge using the direction of the player and the charge distance variable
        targetPoint = transform.position + ((playerPos - transform.position).normalized * _chargeDistance);
        
        //Check that the target point is reachable by charging in a straight line
        //if not override the target pos with the farthest reachable point in that direction
        if(Physics.Raycast(transform.position, (targetPoint - transform.position).normalized, out RaycastHit hit, _chargeDistance, _chargeDetectionMask))
        {
            Debug.Log("overriding charge target");
            targetPoint = hit.point;
        }

        //set the target rotation to look in the direction of the target point
        targetRote = Quaternion.LookRotation((targetPoint - transform.position).normalized, Vector3.up);
        //set the navmesh agent destination to the target point
        _agent.SetDestination(targetPoint);

        //prevent another charge from starting
        _canCharge = false;

        //update to ensure charge related functionality occurs
        _isCharging = true;
    }

    //This function ends a charge when the enemy reaches the destination
    private void StopCharge()
    {    
        //Stop the walking animation
        _animator.SetBool("Idle", true);

        //set the destination to the current position to stop movement
        targetPoint = transform.position;
        _agent.SetDestination(transform.position);

        //end the current charge
        _isCharging = false;

        //If the reset attack 
        if (!IsInvoking("ResetAttack"))
            Invoke("ResetAttack", _attackCooldown);
    }

    //This function is invoked after the charge cooldown has elapsed and allows a new charge to begin
    void ResetAttack()
    {
        _canCharge = true;
    }

    private void OnDrawGizmosSelected()
    {
        //Draw a green sphere for each of the patrol points for this enemy  
        Gizmos.color = Color.green;
        if (_patrolPoints != null && _patrolPoints.Count > 0)
        {
            foreach (Transform item in _patrolPoints)
            {
                Gizmos.DrawSphere(item.position, 0.5f);
            }
        }

        //Draw a red wire sphere to represent the charge attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _chargeDistance);

        //Draw a yellow wire sphere to represent the detection radius of this enemy
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);

        //Draw a black sphere for the charge destination
        Gizmos.color = Color.black;
        if(targetPoint != null)
            Gizmos.DrawSphere(targetPoint, 1.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if the enemy hits something while charging then stop
        if(_isCharging)
            StopCharge();

        //if the object that is hit can recive forces
        if (other.TryGetComponent<ForceReceiver>(out ForceReceiver reciever))
        {
            //Apply the knockback force
            reciever.AddForce((other.transform.position - transform.position).normalized, _knockbackForce);
            reciever.AddForce(Vector3.up, _knockbackForce/4);

            //if the object can also recieve damage then apply the attack damage
            if (other.TryGetComponent<CharacterStats>(out CharacterStats stats))
            {
                stats.TakeDamage(_attackDamage);
            }
        }



    }

    //this function is responsible for updating the enemy during a charge
    public void DoCharge()
    {
        //Rotate to face the direction of travel
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRote, Time.deltaTime * _turnSpeed);

        //Move the character controller accordingly & update the nav mesh agent
        _controller.Move(_agent.desiredVelocity.normalized * _chargeSpeed * Time.deltaTime);
        _agent.velocity = _controller.velocity;

        //If the enemy is close enough to the destination during a charge, then stop
        if (Vector3.Distance(transform.position, targetPoint) <= _agent.stoppingDistance && _isCharging)
        {
            StopCharge();
        }
    }
}
