using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[DisallowMultipleComponent]
public class BulletController : MonoBehaviour
{
  
    private int _damageAmount { get; set; }
    private GameObject _bulletOwner = default;


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
