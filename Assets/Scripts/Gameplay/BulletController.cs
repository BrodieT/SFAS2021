using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class handles the bullet functionality
[RequireComponent(typeof(Rigidbody))]
[DisallowMultipleComponent]
public class BulletController : MonoBehaviour
{
  
    private int _damageAmount { get; set; } //the damage to  be inflicted
    private GameObject _bulletOwner = default; //the object firing this bullet


    //Initialise the bullet and ignore collisions with its owner
    public void Setup(float speed, Vector3 direction, int dmgAmount, GameObject owner)
    {
        GetComponent<Rigidbody>().velocity = direction * speed;
        _damageAmount = dmgAmount;
        _bulletOwner = owner;
        Physics.IgnoreCollision(GetComponent<Collider>(), owner.GetComponent<Collider>());
        Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Ignore"));
    }

    private void OnTriggerEnter(Collider other)
    {
        //If something else gets hit by the bullet destroy itself
        //inflicting damage if its a character
        if (other.gameObject != _bulletOwner)
        {
            if (other.TryGetComponent<CharacterStats>(out CharacterStats stats))
            {
                stats.TakeDamage(_damageAmount);
            }

            Destroy(gameObject);
        }
    }
}
