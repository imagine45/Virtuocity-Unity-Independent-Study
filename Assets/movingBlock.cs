using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingBlock : MonoBehaviour
{
    private GameObject keyFrame;
    private Vector3 startPos;
    private Vector3 endPos;
    private bool isMoving = false;
    private bool movingForward = false;
    private float stepSize;

    private GameObject timer;

    //how long in seconds
    private float speed = 0.5f;

    private void Start()
    {
        timer = GameObject.Find("FMODEvents");
        Timer.beatUpdated += move;
        keyFrame = GameObject.Find("Key Frame");
        startPos = this.transform.position;
        endPos = this.transform.Find("Key Frame").position;
        //Add the move function to the Timer on-beat script
        stepSize = Vector3.Distance(startPos, endPos) / speed;
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            if (movingForward)
            {
                moveToEnd();
            }
            else if (!movingForward)
            {
                moveToStart();
            }
            if (Vector3.Distance(transform.position, endPos) < 0.1f || Vector3.Distance(transform.position, startPos) < 0.1f)
            {
                isMoving = false;
                movingForward = !movingForward;

                //If player velocity doesn't work as expected, add this block's velocity to the player's here
            }
        }
    }

    private void move()
    {
        if (timer.GetComponent<Timer>().currentBeat == 1)
        {
            isMoving = true;
            Debug.Log("moving");
        }
    }

    private void moveToEnd()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, endPos, 1f);
        //this.transform.position = new Vector3(this.transform.position.x + stepSize, this.transform.position.y + stepSize, 0);
    }

    private void moveToStart()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, startPos, 1f);
        //this.transform.position = new Vector3(this.transform.position.x - stepSize, this.transform.position.y - stepSize, 0);
    }

}
