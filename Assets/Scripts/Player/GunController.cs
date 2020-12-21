using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunController : MonoBehaviour
{

    private int _totalAmmo = 20;
    private int _loadedAmmo = 10;
    [SerializeField] private int _maxAmmo = 10;
    [SerializeField] private GameObject BulletPrefab = default;
    [SerializeField] private Animator _gunAnimator = default;
    [SerializeField] private Transform _gunTip = default;
   public bool _isAiming { get; set; }
    public bool _isWalking { get; set; }
    public bool _isRunning { get; set; }

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

    public void Fire(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            _gunAnimator.SetTrigger("Shoot");
            Invoke("ResetShootTrigger", 1.0f);
        }
    }

        public void Aim(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            _isAiming = true;
        }
        else
        {
            _isAiming = false;
        }
    }
}
