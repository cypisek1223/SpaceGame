using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Func
{
    Linear,
    Quadratic
}
public class ObstaclesGenerator : MonoBehaviour
{
    public Func funkcja;
    //Func<float, float> func;
    Funkcja func;
    delegate float Funkcja(float x, float a=1, float b=0);

    public float speed = 1;
    public float scale = 1;
    public float y_Offset = 10;
    public float x_Offset;
    public float r = 1;
    public float timeStep = 1;
    public GameObject[] prefabs;
    private GameObject prefab;

    Rigidbody rb;
    Vector3 startPosition;
    float t;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        StartCoroutine(SpawningCoroutine());
    }
    private void OnValidate()
    {
        switch (funkcja)
        {
            case Func.Linear:
                func = liniowa;
                break;

            case Func.Quadratic:
                func = (x,a,b) => x*x;
                break;
        }
        
    }

    float liniowa(float x, float a=1, float b=0)
    {
        return x * a + b;
    }

    Vector3 œruba(float t, float r, float h=0)
    {
        return new Vector3(r * Mathf.Cos(t), 0, r * Mathf.Sin(t));
    }

    IEnumerator SpawningCoroutine()
    {
        while(true)
        {
            SpawnObstacle();
            yield return new WaitForSeconds(timeStep);
        }
    }

    // Update is called once per frame
    void SpawnObstacle()
    {
        prefab = prefabs[UnityEngine.Random.Range(0, prefabs.Length)];
        Instantiate(prefab, new Vector3(transform.position.x + x_Offset, func(transform.position.x,scale,0)+y_Offset, UnityEngine.Random.Range(0,0)), Quaternion.identity);
    }

    private void FixedUpdate()
    {
        float t = Time.time * speed;
        rb.MovePosition(new Vector3(t, func(t), 0f) + œruba(t,r));
    }
}
