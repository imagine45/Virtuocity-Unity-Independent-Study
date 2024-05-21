using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingBlock : MonoBehaviour
{
    private GameObject keyFrame;
    private Rigidbody2D rb;
    private Vector3 startPos;
    private Vector3 endPos;
    private bool isMoving = false;
    private bool movingForward = false;
    private float speed;

    private GameObject timer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        timer = GameObject.Find("FMODEvents");
        Timer.beatUpdated += Move;
        keyFrame = GameObject.Find("Key Frame");
        startPos = this.transform.position;
        endPos = this.transform.Find("Key Frame").position;

        // Calculate speed based on distance and time required
        speed = Vector3.Distance(startPos, endPos) / 0.2f; // 0.5f is the time duration in seconds
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            Vector3 targetPos = movingForward ? endPos : startPos;
            Vector3 direction = (targetPos - transform.position).normalized;
            rb.velocity = direction * speed;

            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                isMoving = false;
                movingForward = !movingForward;
                rb.velocity = Vector3.zero;

                Debug.Log("End");

                //If player velocity doesn't work as expected, add this block's velocity to the player's here
            }
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    private void Move()
    {
        if (timer.GetComponent<Timer>().currentBeat == 1)
        {
            isMoving = true;
            Debug.Log("moving");
        }
    }
}