using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 1.0f;
    [System.Serializable] private enum PickupType { None = 0, HP = 1, Ammo = 2, Collectible = 3};
    [SerializeField] private PickupType _pickupType = 0;
    [SerializeField] private int _resourceAmount = 10;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(transform.up * _rotationSpeed * Time.deltaTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            switch(_pickupType)
            {
                case PickupType.HP:
                    Game_Manager.instance._player.GetComponent<CharacterStats>().RestoreHP(_resourceAmount);
                    break;
                case PickupType.Ammo:
                    Game_Manager.instance._player.GetComponent<PlayerGunController>().AddAmmo(_resourceAmount);
                    break;
                case PickupType.Collectible:
                    Game_Manager.instance._UIManager._discoveryUI.Discover(new DiscoveryUI.Discovery("Collectible"));
                    ProgressionTracker.instance.FoundCollectible();
                    break;
                default:
                    break;            
            }

            Destroy(gameObject, 0.1f);
        }
    }
}
