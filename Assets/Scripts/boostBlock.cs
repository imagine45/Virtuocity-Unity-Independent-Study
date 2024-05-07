using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boostBlock : MonoBehaviour
{
    private GameObject timer;
    private GameObject player;
    private bool playerOn = false;

    private enum directions { RIGHT, LEFT = 1 }
    [SerializeField] directions direction;

    private void Awake()
    {
        timer = GameObject.Find("FMODEvents");
        player = GameObject.Find("Player");
        Timer.beatUpdated += boost;
    }

    private void OnDestroy()
    {
        Timer.beatUpdated -= boost;
    }

    private void boost()
    {
        if (timer.GetComponent<Timer>().currentBeat == 1)
        {
            this.GetComponent<Animator>().SetTrigger("Boost");

            if (playerOn)
            {
                player.GetComponent<PlayerController>().speedBlockBoost((int)direction);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOn = true;
            player.GetComponent<PlayerController>().speedBlockIdle((int)direction, true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOn = false;
            player.GetComponent<PlayerController>().speedBlockIdle((int)direction, false);
        }
    }
}
