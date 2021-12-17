using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JetForce : MonoBehaviour
{
    public Vector3 force;
    Rigidbody rb;
    public Transform target;
    public float dotProd;
    public Rigidbody[] rbs;

    LineRenderer lr;
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody>();
        rbs = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in rbs)
            print(rb.gameObject);

        print(rbs.Sum(x => x.mass));


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 jetToTarget = target.position - transform.position;
        dotProd = Vector3.Dot(transform.forward, jetToTarget);
        Debug.DrawLine(Vector3.zero, transform.forward, Color.green);
        Debug.DrawLine(Vector3.zero, jetToTarget, Color.red);
        Debug.DrawLine(transform.position, target.position);
        //print(rb.mass + rbs.Sum(x=>x.mass));
        rb.AddForce(force * (rbs.Length ), ForceMode.Acceleration);// + rbs.Sum(rb => rb.mass)));// * rbs.Sum(rb=>rb.mass) );    
        RenderLine();
    }

    void RenderLine()
    {
        lr.positionCount = rbs.Length;
        lr.SetPositions(rbs.Select(x => x.position).ToArray());
        //lr.SetPosition(rbs.Length-1, rbs[rbs.Length - 1].position);
    }
}
