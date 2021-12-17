using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelJumpScript : MonoBehaviour
{
    public Rigidbody rb;
    public WheelCollider wc;

    public float initialForceDown = 10;
    public float przyrost = 1;
    public float maxForceDown = 100;

    public float freeDamper = 10000;
    public float heldDamper = 10;

    public float freeSpring = 700000;
    public float heldSpring = 700000;

    float currentForce;
    JointSpring sj;
    
    // Start is called before the first frame update
    void Start()
    {
        //rb.mass = initialMass;
        currentForce = initialForceDown;
        sj = wc.suspensionSpring;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Jump"))
        {
            StartCoroutine(AggregateMass());
        }
        if (currentForce == initialForceDown)
        {
            sj.damper = freeDamper;
            sj.spring = freeSpring;
        }
        else
        {
            sj.damper = heldDamper;
            sj.spring = heldSpring;
        }

        wc.suspensionSpring = sj;

    }

    IEnumerator AggregateMass()
    {
        while(Input.GetButton("Jump"))
        {
            //float zwiêkszonaMasa = rb.mass + przyrost;
            //rb.mass = zwiêkszonaMasa;

            if (currentForce < maxForceDown - przyrost)
            {
                currentForce += przyrost; 
            }

            rb.AddForce(new Vector3(0f, -currentForce, 0f));
            yield return new WaitForEndOfFrame();
        }
        currentForce = initialForceDown;
        //rb.mass = initialMass;
    }
}
