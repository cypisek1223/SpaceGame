using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelingPlatform : MonoBehaviour
{

    void Start()
    {
        
    }
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("On Platfotm For Fueling");
        }
    }
}
