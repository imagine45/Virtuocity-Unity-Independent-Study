using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class speedGate : MonoBehaviour
{
    private GameObject player;
    public Animator animator;
    public float openSpeed = 1;
    private float initY;
    private float playerY;
    private float playerX;
    private float speed;
    private bool isPlayerUnder = false;

    private void Awake()
    {
        player = GameObject.Find("Player");
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

        if (speed >= openSpeed || isPlayerUnder)
        {
            animator.SetBool("doorOpen", true);
        } else
        {
            animator.SetBool("doorOpen", false);
        }
    }
}
