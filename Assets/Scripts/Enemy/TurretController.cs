using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script controls the turret enemy type.
//It contains the same functionality as its parent enemy controller class with some additional unique functionality
//The purpose of this enemy type is to remain in the same spot, rotating to shoot at the player from its fixed position
public class TurretController : EnemyController
{
    private Vector3 _defaultRotation = new Vector3(); //The starting rotation of the turret
    private Quaternion _targetRote = new Quaternion(); //The destination rotation of the turret
    private List<Quaternion> _turretPatrolRotes = new List<Quaternion>(); //A list of the "patrol points" of the turret (quaternions as only rotation is possible)

    [Header("Turret Enemy Parameters")]
    [SerializeField] private float _turretViewAngle = 90.0f; //How wide an angle the turret can rotate when idle
    [SerializeField] private float _turretTurnSpeed = 1.0f; //How quickly the turret can turn to reach its target rotation
    [SerializeField] Transform _rotatableObject = default; //The child of the whole turret object that can be rotated to only turn the "head"

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        //if unassigned, rotate the entire object
        if (_rotatableObject == null)
            _rotatableObject = transform;

        //Store the starting rotation of the rotatable object
        _defaultRotation = _rotatableObject.rotation.eulerAngles;
     
        //Calculate the two rotation patrol points
        Quaternion roteA = Quaternion.Euler(_defaultRotation + (Vector3.up * _turretViewAngle));
        Quaternion roteB = Quaternion.Euler(_defaultRotation - (Vector3.up * _turretViewAngle));
        _turretPatrolRotes.Add(roteA);
        _turretPatrolRotes.Add(roteB);
        //Set the current target rotation to the first calcualted point
        _targetRote = _turretPatrolRotes[0];
    }

    //This function determines which of the two patrol points is closest, and sets the target to the other
    private void FindNewTurretRotation()
    {
        if (_targetRote == _turretPatrolRotes[0])
            _targetRote = _turretPatrolRotes[1];
        else
            _targetRote = _turretPatrolRotes[0];
    }

    //This function overrides the parent patrol function, to rotate the turret head towards the target rotation when idle
    public override void Patrol()
    {
        //Switch targets when close to destination
        if (Quaternion.Angle(_rotatableObject.rotation, _targetRote) < 1)
        {
            FindNewTurretRotation();
        }
        else
        {
            //interpolate the rotation towards the target
            _rotatableObject.rotation = Quaternion.Slerp(_rotatableObject.rotation, _targetRote, Time.deltaTime * (_turretTurnSpeed / 2));
        }
    }

    //This function overrides the parent attack function to face the player and shoot at them when in range
    public override void Attack()
    {
        //Call the base function to decrement ranged timer
        base.Attack();

        //Face the player
        Quaternion targetRote = Quaternion.LookRotation((_player.position - transform.position).normalized, Vector3.up);
        _rotatableObject.rotation = Quaternion.Slerp(_rotatableObject.rotation, targetRote, Time.deltaTime * _turnSpeed);

        //If in range and ready to fire, shoot gun and restart cooldown
        if (_playerDistance <= _shootingRange && _rangedTimer <= 0.0f)
        {
            GetComponent<GunController>().ShootGun();
            _rangedTimer = _rangedAttackCooldown;
        }
    }

    private void OnDrawGizmosSelected()
    {
       

        //Draw a red wire sphere to represent the charge attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _shootingRange);

        //Draw a yellow wire sphere to represent the detection radius of this enemy
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);

    }

}
