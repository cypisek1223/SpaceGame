using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelScript : MonoBehaviour
{
    public WheelCollider wheelCollider;
    public GameObject wheel;
    public bool frontWheel;


    
    public float toruqeMultiplier = 1f;
    public float maxSpeed = 50; // [km/h]

    public Rigidbody wheelRb;
    public float currentSpeed;

    private void Update()
    {
        Vector3 currentWheelPosition;
        Quaternion currentWheelRotation;
        wheelCollider.GetWorldPose(out currentWheelPosition, out currentWheelRotation);
        wheel.transform.position = currentWheelPosition;
        wheel.transform.rotation = currentWheelRotation;

        HandleMotion();

        currentSpeed = wheelRb.velocity.magnitude / 3.6f; // [km/h]
    }

    void HandleMotion()
    {
        float acc = Input.GetAxis("Horizontal");

        if (currentSpeed < maxSpeed)
        {
            wheelCollider.motorTorque = acc * toruqeMultiplier; 
        }
    }

}
