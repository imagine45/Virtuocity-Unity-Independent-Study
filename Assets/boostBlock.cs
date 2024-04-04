using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boostBlock : MonoBehaviour
{
    public GameObject timer;
    public GameObject player;
    private bool playerOn = false;

    private enum directions { RIGHT, LEFT = 1 }
    [SerializeField] directions direction;

    private void Awake()
    {
        Timer.beatUpdated += boost;
        print((int)direction);
    }

    private void OnDestroy()
    {
        Timer.beatUpdated -= boost;
    }

    private void boost()
    {
        if (timer.GetComponent<Timer>().currentBeat == 3)
        {
            print("Boost w/ player");
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
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOn = false;
        }
    }
}
