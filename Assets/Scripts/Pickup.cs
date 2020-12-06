using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 1.0f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(transform.up * _rotationSpeed * Time.deltaTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Pickup");
           
        }
    }
}
