using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class killPlayer : MonoBehaviour
{

    private GameObject player;
    bool killsPlayer = false;

    private void Start()
    {
        player = GameObject.Find("Player");
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (killsPlayer)
            {
                player.GetComponent<PlayerController>().kill();
            }
        }
    }

    private void Update()
    {
        if (player.GetComponent<Rigidbody2D>().velocity.y > 0.1f)
        {
            killsPlayer = false;
        }
        else
        {
            killsPlayer = true;
        }
    }
}
