using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script controls the berserker enemy type
//It contains the same functionality as its parent enemy controller class with some additional unique functionality
//The purpose of this enemy type is to run to a random point/at the player, and either shoot at them or melee attack
public class BerserkerController : EnemyController
{
    [Header("Berserker Settings")]
    [SerializeField] private float _meleeRange = 3.0f; //How close the player must be to be attacked via melee
    [SerializeField] private float _meleeAttackCooldown = 5.0f; //how long between melee attacks
    [SerializeField] private float _berserkRange = 20.0f; //the range at which the enemy can "berserk" in - radius to find points in


    [Header("Idle Settings")]
    [SerializeField] private List<Transform> _patrolPoints = new List<Transform>(); //The list of points the enemy can patrol through when idle
    private int _currentPatrolPoint = 0; //the index of the current patrol point
    private float _meleeTimer = 0.0f;
    private Animator _animator = default;

    private enum AttackBehaviour { Move = 0, Attack = 1 }; //What behaviours the berserker enemy can display when in the attack state
    private AttackBehaviour _currentAttack = AttackBehaviour.Attack;
    private Vector3 _targetPosition = new Vector3();

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        _animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }


    public override void Patrol()
    {
        base.Patrol();

        if (Vector3.Distance(transform.position, _patrolPoints[_currentPatrolPoint].position) < 1.0f)
        {
            //Stop the walk animation
            _animator.SetBool("Idle", true); 

            _agent.SetDestination(transform.position);
            if (!IsInvoking("FindNewPatrolPoint"))
                Invoke("FindNewPatrolPoint", 3.0f);
        }
        else
        {
            //Begin the walk animation
            _animator.SetBool("Idle", false);
            EnemyMove(_patrolPoints[_currentPatrolPoint].position);
        }
    }

    private void FindNewPatrolPoint()
    {
        _currentPatrolPoint++;
        if (_currentPatrolPoint >= _patrolPoints.Count)
        {
            _currentPatrolPoint = 0;
        }
    }


    public override void Attack()
    {
        base.Attack();

        _meleeTimer -= Time.deltaTime;


        if (_currentAttack == AttackBehaviour.Attack)
        {
            //Stop all movement when attacking
            _agent.SetDestination(transform.position);

            //Face the player
            Quaternion targetRote = Quaternion.LookRotation((transform.position - _player.position).normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRote, Time.deltaTime * _turnSpeed);

            //if close enough for melee and the cooldown has elapsed perform melee attack
            if (_playerDistance <= _meleeRange && _meleeTimer <= 0.0f)
            {
                _animator.SetBool("Idle", true);
                _animator.SetBool("Shoot", false);

                _meleeTimer = _meleeAttackCooldown;
                Vector3 hurtDirection = (_player.position - transform.position).normalized;
                _player.GetComponent<ForceReceiver>().AddForce((hurtDirection + Vector3.up).normalized, 100.0f);

                _targetPosition = new Vector3(Random.Range(transform.position.x - _berserkRange, transform.position.x + _berserkRange), transform.position.y, Random.Range(transform.position.z - _berserkRange, transform.position.z + _berserkRange));

                if (Physics.Raycast(transform.position, (transform.position - _targetPosition).normalized, out RaycastHit hit, _berserkRange))
                {
                    _targetPosition = hit.point;
                }

                _currentAttack = AttackBehaviour.Move;
            } //if close enough for ranged and cooldown has elapsed perform ranged attack
            else if (_playerDistance <= _shootingRange && _rangedTimer <= 0.0f)
            {
                Debug.Log("Bang!");

                _animator.SetBool("Idle", true);
                _animator.SetBool("Shoot", true);

                _rangedTimer = _rangedAttackCooldown;

                _targetPosition = new Vector3(Random.Range(transform.position.x - _berserkRange, transform.position.x + _berserkRange), transform.position.y, Random.Range(transform.position.z - _berserkRange, transform.position.z + _berserkRange));
                _currentAttack = AttackBehaviour.Move;
            }
        }
        else
        {
            _animator.SetBool("Shoot", false);
            _animator.SetBool("Idle", false);

            EnemyMove(_targetPosition);
        }
    }

    private void OnDrawGizmosSelected()
    {

        //Draw a green sphere for each of the patrol points for this enemy  
        Gizmos.color = Color.green;
        foreach (Transform item in _patrolPoints)
        {
            Gizmos.DrawSphere(item.position, 0.5f);
        }

        //Draw a red wire sphere to represent the melee attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _meleeRange);

        //Draw a blue wire sphere to represent the ranged attack distance
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _shootingRange);

        //Draw a yellow wire sphere to represent the detection radius of this enemy
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);

        Gizmos.DrawSphere(_targetPosition, 1.0f);
    }
}
