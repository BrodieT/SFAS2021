using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class is used to handle gun based functionality such as spawning of bullets and ammo management 
[DisallowMultipleComponent]
public class GunController : MonoBehaviour
{

    [SerializeField] public int _spareAmmo = 18; //The number of bullets that are not loaded into the gun
    [SerializeField] public int _loadedAmmo = 6; //The number of bullets currently loaded into the gun
    [SerializeField] public int _magCapacity = 6; //The maximum number of bullets that can be loaded into the gun
    [SerializeField] public GameObject _bulletPrefab = default; //The bullet to be spawned
    [SerializeField] public Transform _gunTip = default; //Where the bullet will be spawned at
    [SerializeField] public float _gunRange = 100.0f; //how far the gun can shoot
    [SerializeField] public float _bulletSpeed = 5.0f; //how fast the bullet will travel
    [SerializeField] public int _damageAmount = 10; //how much damage this gun can inflict
    [SerializeField] private AudioClip _gunshotSound = default; //the sound played when firing the gun
    private AudioSource _audioSource = default; //where the sound will be played from
    public bool _isReloading = false; //determines if a reload process is occuring

    public virtual void Start()
    {
        //Find the audio source on the gun
        _audioSource = GetComponent<AudioSource>();
    }

    //This function calculates the destination of the bullet
    public virtual Vector3 GetTarget()
    {
        return _gunTip.forward * _gunRange;
    }

    //This function gets the direction of the bullet
    public Vector3 GetDirection()
    {
        Vector3 from = _gunTip.transform.position;
        return (GetTarget() - from).normalized;
    }

    //This function determines if the gun is ready and able to shoot
    public virtual bool CanShoot()
    {
        if (_loadedAmmo > 0 && !_isReloading)
            return true;

        return false;
    }

    //This function fires the gun
    public virtual void ShootGun()
    {
        if (CanShoot())
        {
            //Find the audio source if not already found
            if (_audioSource == null)
                _audioSource = GetComponent<AudioSource>();

            //if an audio source is available play the gunshot
            if (_audioSource != null)
                _audioSource.PlayOneShot(_gunshotSound);
           
            //Spawn the bullet, rotate to face the correct direction and fire it
            GameObject bullet = Instantiate(_bulletPrefab, _gunTip.transform.position, Quaternion.identity);
            bullet.transform.LookAt(GetTarget());
            bullet.GetComponent<BulletController>().Setup(_bulletSpeed, GetDirection(), _damageAmount, gameObject);
        }
    }

   
}
