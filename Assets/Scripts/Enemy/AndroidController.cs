using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidController : EnemyController
{
    [Header("Android Enemy Parameters")]
    [SerializeField] private float _meleeRange = 3.0f;
    [SerializeField] private float _meleeAttackCooldown = 5.0f;

    [SerializeField] private List<Transform> _patrolPoints = new List<Transform>();
    private int _currentPatrolPoint = 0;
    private float _meleeTimer = 0.0f;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
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
            if (!IsInvoking("FindNewPatrolPoint"))
                Invoke("FindNewPatrolPoint", 3.0f);
        }
        else
        {
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


        if (_playerDistance <= _meleeRange && _meleeTimer <= 0.0f)
        {
            _meleeTimer = _meleeAttackCooldown;
            Vector3 hurtDirection = (PlayerMovement.PlayerInstance.transform.position - transform.position).normalized;
            PlayerMovement.PlayerInstance.GetComponent<ForceReceiver>().AddForce((hurtDirection + Vector3.up).normalized, 100.0f);
        }
        else if (_playerDistance <= _shootingRange && _rangedTimer <= 0.0f)
        {
            Debug.Log("Bang!");
            _rangedTimer = _rangedAttackCooldown;
        }
        else
        {
            EnemyMove(PlayerMovement.PlayerInstance.transform.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _meleeRange);

        foreach (Transform item in _patrolPoints)
        {
            Gizmos.DrawSphere(item.position, 0.5f);
        }
    }
}
