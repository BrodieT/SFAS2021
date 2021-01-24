using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class represents things the player can pick up
public class Pickup : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 1.0f; //How quickly the object will rotate
    [System.Serializable] private enum PickupType { None = 0, HP = 1, Ammo = 2, Collectible = 3};
    [SerializeField] private PickupType _pickupType = 0; //The type of pickup - used to determine functionality when picked up
    [SerializeField] private int _resourceAmount = 10; //The amount of the associated resource to be added
    // Update is called once per frame
    void Update()
    {
        //rotate each frame
        transform.Rotate(transform.up * _rotationSpeed * Time.deltaTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        //Perform the appropriate functionality if the player enter this trigger
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

            //Disappear to prevent multiple pickups
            Destroy(gameObject, 0.1f);
        }
    }
}
