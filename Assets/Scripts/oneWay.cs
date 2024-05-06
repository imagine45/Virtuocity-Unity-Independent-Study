using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oneWay : MonoBehaviour
{
    private GameObject player;
    private GameObject groundCheck;
    private Vector2 playerVector;
    private Vector3 playerCoord;
    public Rigidbody2D rb;

    //The direction at which the player can pass through
    private Vector2 platformVector = new Vector2(0, 1);

    private void Start()
    {
        player = GameObject.Find("Player");
        groundCheck = GameObject.Find("Ground Check");
    }

    private void FixedUpdate()
    {
        playerVector = player.GetComponent<Rigidbody2D>().velocity;
        playerCoord = groundCheck.transform.position;

        if (Mathf.Sign(playerVector.y) == Mathf.Sign(platformVector.y) && Mathf.Abs(playerVector.y) >= 0.001)
        {
            rb.simulated = false;
        } else if (playerCoord.y >= this.transform.position.y) 
        {
            rb.simulated = true;
        }
    }
}
