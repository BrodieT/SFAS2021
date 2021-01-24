using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
public class PlayerGunController : GunController
{

    private Camera _playerCamera = default;
    public bool _isAiming { get; set; }
    public bool _isWalking { get; set; }
    public bool _isRunning { get; set; }
    [SerializeField] private Animator _gunAnimator = default;
    [SerializeField] private float _reloadRate = 0.5f; //The time it takes to reload each bullet
    [Header("UI")]
    [SerializeField] private List<Image> _bulletUI = default;
    [SerializeField] private Color _emptyColour = default;
    [SerializeField] private Color _loadedColour = default;
    [SerializeField] private TMP_Text _bulletCount = default;

    public void Fire(InputAction.CallbackContext context)
    {
        if (context.performed && !GameUtility._isPaused && GameUtility._isPlayerObjectBeingControlled)
        {
            ShootGun();
        }
    }
    public void Aim(InputAction.CallbackContext context)
    {
        if (!GameUtility._isPaused && GameUtility._isPlayerObjectBeingControlled)
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
    }

    public void StoreAmmoCounter()
    {
        ProgressionTracker.instance.SetAmmoCounters(_loadedAmmo, _spareAmmo);
    }

    public void UpdateAmmoCounter()
    {
        _loadedAmmo = ProgressionTracker.instance._loadedAmmo;
        _spareAmmo = ProgressionTracker.instance._spareAmmo;
        UpdateUI();
    }

    public void Reload(InputAction.CallbackContext context)
    {
        if (context.performed && !GameUtility._isPaused && GameUtility._isPlayerObjectBeingControlled)
        {
            if(!_isReloading && _loadedAmmo < _magCapacity)
                StartCoroutine(ReloadGun());
        }
    }

    IEnumerator ReloadGun()
    {
        _isReloading = true;
        if (_spareAmmo > 0)
        {
            int amountToReload = _magCapacity - _loadedAmmo;

            if (_spareAmmo > amountToReload)
            {
                for (int i = 0; i < amountToReload; i++)
                {
                    _loadedAmmo++;
                    _spareAmmo--;
                    UpdateUI();
                    StoreAmmoCounter();
                    yield return new WaitForSeconds(_reloadRate);
                }
            }
            else
            {
                for (int i = 0; i < _spareAmmo; i++)
                {
                    _loadedAmmo++;
                    _spareAmmo--;
                    UpdateUI();
                    StoreAmmoCounter();
                    yield return new WaitForSeconds(_reloadRate);
                }
            }
        }
        _isReloading = false;
    }

    public void UpdateUI()
    {
        for (int i = 0; i < _magCapacity; i++)
        {
            if (i < _loadedAmmo)
                _bulletUI[i].color = _loadedColour;
            else
                _bulletUI[i].color = _emptyColour;
        }

        _bulletCount.text = _spareAmmo.ToString();
    }

    public override Vector3 GetTarget()
    {
        return _playerCamera.transform.position + (_playerCamera.transform.forward * 100.0f);
    }

    public override void Start()
    {
        base.Start();
        _playerCamera = Game_Manager.instance._playerCamera;
        UpdateUI();
    }

    public override void ShootGun()
    {
        if (CanShoot() && !_isRunning)
        {
            _loadedAmmo--;
            StoreAmmoCounter();

            _gunAnimator.SetTrigger("Shoot");

            base.ShootGun();
            UpdateUI();

            Invoke("ResetShootTrigger", 1.0f);
        }
    }

    public void AddAmmo(int amount)
    {
        _spareAmmo += amount;
        StoreAmmoCounter();
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
