using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : EnemyController
{
    private Vector3 _defaultRotation = new Vector3();
    [Header("Turret Enemy Parameters")]
    [SerializeField] private float _turretViewAngle = 90.0f;
    private Quaternion _targetRote = new Quaternion();
    private List<Quaternion> _turretPatrolRotes = new List<Quaternion>();
    [SerializeField] private float _turretTurnSpeed = 1.0f;


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        _defaultRotation = transform.rotation.eulerAngles;
     
        Quaternion roteA = Quaternion.Euler(_defaultRotation + (Vector3.up * _turretViewAngle));
        Quaternion roteB = Quaternion.Euler(_defaultRotation - (Vector3.up * _turretViewAngle));
        _turretPatrolRotes.Add(roteA);
        _turretPatrolRotes.Add(roteB);
        _targetRote = _turretPatrolRotes[0];
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    private void FindNewTurretRotation()
    {
        if (_targetRote == _turretPatrolRotes[0])
            _targetRote = _turretPatrolRotes[1];
        else
            _targetRote = _turretPatrolRotes[0];
    }

    public override void Patrol()
    {
        base.Patrol();

        if (Quaternion.Angle(transform.rotation, _targetRote) < 1)
        {
            FindNewTurretRotation();
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _targetRote, Time.deltaTime * _turretTurnSpeed);
        }
    }
    
    public override void Attack()
    {
        base.Attack();

        if (_playerDistance <= _shootingRange && _rangedTimer <= 0.0f)
        {
            Debug.Log("Bang!");
            _rangedTimer = _rangedAttackCooldown;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
    }
}
