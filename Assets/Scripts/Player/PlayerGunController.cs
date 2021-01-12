using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGunController : GunController
{

    private Camera _playerCamera = default;
    public bool _isAiming { get; set; }
    public bool _isWalking { get; set; }
    public bool _isRunning { get; set; }
    [SerializeField] private Animator _gunAnimator = default;
    public void Fire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ShootGun();
        }
    }
    public void Aim(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isAiming = true;
        }
        else
        {
            _isAiming = false;
        }
    }

    public override Vector3 GetTarget()
    {
        return _playerCamera.transform.position + (_playerCamera.transform.forward * 100.0f);
    }

    public override void ShootGun()
    {
        if (CanShoot())
        {
            _gunAnimator.SetTrigger("Shoot");

            base.ShootGun();

            Invoke("ResetShootTrigger", 1.0f);
        }
    }
    private void Start()
    {
        _playerCamera = Game_Manager.instance._playerCamera;
    }

    private void Update()
    {
        _gunAnimator.SetBool("Aim", _isAiming);
        _gunAnimator.SetBool("Walk", _isWalking);
        _gunAnimator.SetBool("Run", _isRunning);

    }

    private void ResetShootTrigger()
    {
        _gunAnimator.ResetTrigger("Shoot");

    }
}
