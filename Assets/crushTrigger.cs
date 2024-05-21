using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crushTrigger : MonoBehaviour
{

    private GameObject player;
    private float time;

    void Start()
    {
        player = GameObject.Find("Player");
        time = 0;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (!collision.CompareTag("Battery"))
        {
            time += Time.deltaTime;
        }

        if (time >= 0.1 && !player.GetComponent<PlayerController>().isDead)
        {
            player.GetComponent<PlayerController>().kill();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        time = 0;
    }
}
