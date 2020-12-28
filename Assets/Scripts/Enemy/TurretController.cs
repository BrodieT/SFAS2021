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
    [SerializeField] Transform _rotatableObject = default;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        if (_rotatableObject == null)
            _rotatableObject = transform;

        _defaultRotation = _rotatableObject.rotation.eulerAngles;
     
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
        if (Quaternion.Angle(_rotatableObject.rotation, _targetRote) < 1)
        {
            FindNewTurretRotation();
        }
        else
        {
            _rotatableObject.rotation = Quaternion.Slerp(_rotatableObject.rotation, _targetRote, Time.deltaTime * (_turretTurnSpeed / 2));
        }
    }

    public override void Search()
    {
        base.Search();
    }

    public override void Attack()
    {

        _rangedTimer -= Time.deltaTime;

        Vector3 targetPoint = PlayerMovement.instance.transform.position - transform.position;
        Quaternion targetRote = Quaternion.LookRotation(targetPoint, Vector3.up);
        _rotatableObject.rotation = Quaternion.Slerp(_rotatableObject.rotation, targetRote, Time.deltaTime * _turnSpeed);


        if (_playerDistance <= _shootingRange && _rangedTimer <= 0.0f)
        {
            GetComponent<GunController>().ShootGun();
            _rangedTimer = _rangedAttackCooldown;
        }
    }
}
