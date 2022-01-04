using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESC : MonoBehaviour
{

    public Canvas Esc;
    // Start is called before the first frame update
    void Start()
    {
        Esc.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Esc.enabled = !Esc.enabled;
        }
        
    }
}
