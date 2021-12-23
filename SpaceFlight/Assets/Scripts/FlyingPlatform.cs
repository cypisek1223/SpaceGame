using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingPlatform : MonoBehaviour
{
    public Transform StartPost, Post1, Post2;
    public float szybkosc;
    Vector3 NextPost;

    public GameObject Player;
    void Start()
    {
        NextPost = StartPost.position;
    }
    void Update()
    {
        if (transform.position == Post1.position)
        {
            Debug.Log("Post1");
            NextPost = Post2.position;
        }
        if (transform.position == Post2.position)
        {
            Debug.Log("Post2");
            NextPost = Post1.position;
        }
        transform.position = Vector3.MoveTowards(transform.position, NextPost, szybkosc * Time.deltaTime);
    }
   private void DrowLine()
    {
        Gizmos.DrawLine(Post1.position, Post2.position);
    }
    //public void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "Player")
    //    {
    //        Player.transform.parent = transform.parent;
    //        Debug.Log("On Platform");
    //    }
    //    else
    //    {
    //        Player.transform.parent = null;
    //    }
    //}
}

