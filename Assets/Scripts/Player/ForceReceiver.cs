using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ForceReceiver : MonoBehaviour
{
    //The rate the force value will lerp back to zero
    [SerializeField] private float _deceleration = 5.0f;
    //How heavy the player is, used when calculating the intensity
    [SerializeField] private float _mass = 3.0f;
    //The directional force vector, scaled with the mass of the player
    private Vector3 _intensity = new Vector3();
    //Local store of the character controller
    private CharacterController _controller;

    // Start is called before the first frame update
    void Start()
    {
        _intensity = Vector3.zero;
        _controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //If a large enough force has been applied then move the character controller
        if(_intensity.magnitude > 0.2f)
        {
            _controller.Move(_intensity * Time.deltaTime);
        }

        //Decelerate the intensity value
        _intensity = Vector3.Lerp(_intensity, Vector3.zero, _deceleration * Time.deltaTime);
    }

    //This function will add a force value to the character controller
    public void AddForce(Vector3 direction, float force)
    {
        _intensity += direction.normalized * force / _mass;
    }
}
