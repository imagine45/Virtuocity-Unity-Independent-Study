using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cannonBehavior : MonoBehaviour
{
    private LayerMask playerLayer;
    private GameObject player;
    private int radius = 1;
    private Vector3 Transform;
    bool playerIn = false;

    private void Start()
    {
        player = GameObject.Find("Player");
        playerLayer = LayerMask.GetMask("Player");
        Transform = transform.position;
    }

    private void FixedUpdate()
    {
        if (isPlayerNear())
        {
            player.transform.position = new Vector3(Transform.x, Transform.y, 0);
        }
    }

    private bool isPlayerNear()
    {
        return Physics2D.OverlapCircle(transform.position, radius + 0.2f, playerLayer);
    }
}
