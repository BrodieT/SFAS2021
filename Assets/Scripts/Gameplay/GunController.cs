using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GunController : MonoBehaviour
{

    [HideInInspector] public int _totalAmmo = 20;
    [HideInInspector] public int _loadedAmmo = 10;
    [SerializeField] public int _maxAmmo = 10;
    [SerializeField] public GameObject _bulletPrefab = default;
    [SerializeField] public Transform _gunTip = default;
    [SerializeField] public float _gunRange = 100.0f;
    [SerializeField] public float _bulletSpeed = 5.0f;
    [SerializeField] public int _damageAmount = 10;
    [SerializeField] private AudioClip _gunshotSound = default;
    private AudioSource _audioSource = default;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public virtual Vector3 GetTarget()
    {
        return _gunTip.forward * _gunRange;
    }
    public Vector3 GetDirection()
    {
        Vector3 from = _gunTip.transform.position;
        return (GetTarget() - from).normalized;
    }

    public virtual bool CanShoot()
    {
        if (_loadedAmmo > 0)
            return true;

        return false;
    }

    public virtual void ShootGun()
    {
        if (CanShoot())
        {
            if (_audioSource == null)
                _audioSource = GetComponent<AudioSource>();

            if (_audioSource == null)
                _audioSource.PlayOneShot(_gunshotSound);
           
            GameObject bullet = Instantiate(_bulletPrefab, _gunTip.transform.position, Quaternion.identity);
            bullet.transform.LookAt(GetTarget());
            bullet.GetComponent<BulletController>().Setup(_bulletSpeed, GetDirection(), _damageAmount, gameObject);
        }
    }

   
}
