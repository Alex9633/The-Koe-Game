using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class Player2 : MonoBehaviour
{
    // for spiky player

    public float speed = 5f;
    public float rotation = 6f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        int x = (UnityEngine.Input.GetKey(KeyCode.LeftArrow) ? -1 : 0) + (UnityEngine.Input.GetKey(KeyCode.RightArrow) ? 1 : 0);
        if (x != 0)
        {
            rb.velocity = Vector3.right * x * speed;
            rb.AddTorque(-x * rotation);
        }
    }
}
