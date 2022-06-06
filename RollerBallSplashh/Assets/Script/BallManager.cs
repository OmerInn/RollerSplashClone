using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    public Rigidbody rb;
    [Header("Movement")]
    public Vector2 firstPos;
    public Vector2 secondPos;
    public Vector2 currentPos;

    public float moveSpeed;

    public float currentGroundNumber;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Swipe();
    }

    private void Swipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            firstPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);


        }

        if (Input.GetMouseButtonUp(0))
        {
            secondPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            currentPos = new Vector2(
                secondPos.x-firstPos.x,
                secondPos.y-firstPos.y
                );
        }
        currentPos.Normalize();
        if (currentPos.y < 0 && currentPos.x > -0.5f && currentPos.x < 0.5f) 
        {
            //back
            rb.velocity = Vector3.back * moveSpeed;
        }
        else if (currentPos.y>0 && currentPos.x > -0.5f && currentPos.x < 0.5f)
        {
            //forward
            rb.velocity = Vector3.forward * moveSpeed;

        }
        else if (currentPos.x < 0 && currentPos.y > -0.5f && currentPos.y < 0.5f)
        {
            //left
            rb.velocity = Vector3.left * moveSpeed;

        }
        else if (currentPos.x > 0 && currentPos.y > -0.5f && currentPos.y < 0.5f)
        {
            //right
            rb.velocity = Vector3.right * moveSpeed;

        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            if (other.gameObject.GetComponent<MeshRenderer>().material.color != Color.red)
            {
                other.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
                Constranits();
                currentGroundNumber++;
            }
        }
    }
    private void Constranits()
    {
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
    }
}
