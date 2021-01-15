using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChargerController : EnemyController
{
    [Header("Charger Settings")]
    [SerializeField] private float _chargeDistance = 20.0f;
    [SerializeField] private float _attackCooldown = 5.0f;
    [SerializeField] private float _chargeSpeed = 5.0f;
    [SerializeField] private float _chargeStoppingDistance = 2.0f;
    private Animator _animator = default;
    private Vector3 targetPoint = new Vector3(); //the resulting position of the projected charge
    private Quaternion targetRote = Quaternion.identity; //the desired rotation of the enemy while charging
    private bool _isCharging = false;
    private bool _canCharge = true;
    
    public override void Start()
    {
        base.Start();
        _animator = GetComponentInChildren<Animator>();
    }

    public override void Attack()
    {
        if (!_isCharging && _canCharge)
        {
            StartCharge();
        }


        DoCharge();

    }

    private void StartCharge()
    {   
        Debug.Log("Start Charge");

        //Begin the walk animation
        _animator.SetBool("Idle", false);

        //set the target point of the charge using the direction of the player and the charge distance variable
        targetPoint = transform.position + ((_player.position - transform.position).normalized * _chargeDistance);
        //set the target rotation to look in the direction of the target point
        targetRote = Quaternion.LookRotation(targetPoint, Vector3.up);
        //set the navmesh agent destination to the target point
        _agent.SetDestination(targetPoint);

        //prevent another charge from starting
        _canCharge = false;

        //update to ensure charge related functionality occurs
        _isCharging = true;
    }

    private void StopCharge()
    {    
        Debug.Log("Stop Charge");

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

    void ResetAttack()
    {
        _canCharge = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _chargeDistance);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_isCharging)
            StopCharge();
    }

    public void DoCharge()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRote, Time.deltaTime * _turnSpeed);
        _controller.Move(_agent.desiredVelocity.normalized * _chargeSpeed * Time.deltaTime);
        _agent.velocity = _controller.velocity;

        float remainingDistance = Vector3.Distance(transform.position, targetPoint);
        if (remainingDistance <= _chargeStoppingDistance && _isCharging)
        {
            StopCharge();
        }
    }
}
