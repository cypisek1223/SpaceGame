using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Statek : MonoBehaviour
{
    //PARAMETRY STATKU
    private Rigidbody rb;
    public float maxVelocity = 3;
    public float rotationSpeed = 3;
    public float speed = 3;
    public float sibilizerForce = 10;
    public float stoppingForce = 1;

    private void Start()
    {

        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {


        float moveHorizontal = Input.GetAxis("Horizontal");//x
        float moveVertical = Input.GetAxis("Vertical");//y

        //if (moveVertical == 0)
        //{
        //    rb.constraints = RigidbodyConstraints.FreezePositionZ |
        //        RigidbodyConstraints.FreezeRotationX |
        //        RigidbodyConstraints.FreezeRotationY;
        //}
        //else
        //{
        //    rb.constraints = RigidbodyConstraints.FreezePositionZ |
        //    RigidbodyConstraints.FreezeRotationX |
        //    RigidbodyConstraints.FreezeRotationY |
        //    RigidbodyConstraints.FreezeRotationZ;
        //}

        ThrustForward(moveVertical);
        Rotate(transform, -moveHorizontal * rotationSpeed);


    }

    #region Steer
    private void ClapVelocity()
    {
        float x = Mathf.Clamp(rb.velocity.x, -maxVelocity, maxVelocity);
        float y = Mathf.Clamp(rb.velocity.y, -maxVelocity, maxVelocity);

        rb.velocity = new Vector2(x, y);
    }
 private void ThrustForward(float amunt)
    {
        rb.AddForce(-rb.velocity / stoppingForce);
        rb.AddForce(Vector3.up * sibilizerForce);
        Vector2 force = transform.up * amunt;
        rb.AddForce(force * speed * Time.fixedDeltaTime);
    }
    private void Rotate(Transform t, float amunt)
    {
        t.Rotate(0, 0, amunt);
    }
    #endregion

}
