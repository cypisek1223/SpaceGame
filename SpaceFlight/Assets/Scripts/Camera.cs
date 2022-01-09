using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
   public  Transform target;
   public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        offset = target.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position - offset;
    }
}
