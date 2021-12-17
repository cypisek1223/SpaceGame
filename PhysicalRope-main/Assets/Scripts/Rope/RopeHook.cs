using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeHook : MonoBehaviour
{
    public LayerMask pickup_layer;

    Rigidbody pickup;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Pickup") & !pickup)//(other.gameObject.layer == pickup_layer & !pickup)
        {
            print("Hook-pickup collision");
            pickup = other.gameObject.GetComponent<Rigidbody>();
            //(other.gameObject.AddComponent(typeof(FixedJoint)) as FixedJoint).connectedBody = rb;
            gameObject.AddComponent<FixedJoint>().connectedBody = pickup;
            pickup.transform.parent = transform;
            //pickup.transform.localScale = Vector3.one;
        }
    }
}
