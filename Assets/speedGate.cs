using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class speedGate : MonoBehaviour
{
    public GameObject player;
    public Animator animator;
    public float openSpeed = 1;
    private float initY;
    private float playerY;
    private float playerX;
    private float speed;
    private bool isPlayerUnder = false;

    private void Awake()
    {
        initY = transform.position.y - 3;
    }

    private void FixedUpdate()
    {
        speed = player.GetComponent<PlayerController>().getSpeed();

        playerX = player.GetComponent<PlayerController>().transform.position.x;
        playerY = player.GetComponent<PlayerController>().transform.position.y;

        if (playerY >= initY - 3 && playerX >= transform.position.x -.4 && playerX <= transform.position.x + .4)
        {
            isPlayerUnder = true;
        } else
        {
            isPlayerUnder = false;
        }

        print(isPlayerUnder);

        if (speed >= openSpeed || isPlayerUnder)
        {
            print("door open: yes" + "    is player under: " + isPlayerUnder);
            //transform.position = new Vector3(transform.position.x, initY + 3, 0);
            animator.SetBool("doorOpen", true);
        } else
        {
            print("door open: no" + "    is player under: " + isPlayerUnder);
            //transform.position = new Vector3(transform.position.x, initY, 0);
            animator.SetBool("doorOpen", false);
        }
    }
}
