using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boomBox : MonoBehaviour
{
    [SerializeField] Animator animator;
    private GameObject timer;
    private GameObject player;
    private double time;
    private bool inDistance;

    private void Start()
    {
        timer = GameObject.Find("FMODEvents");
        player = GameObject.Find("Player");
        Timer.beatUpdated += explode;
    }

    private void OnDestroy()
    {
        Timer.beatUpdated -= explode;
    }

    private void explode()
    {
        if (timer.GetComponent<Timer>().currentBeat == 1)
        {
            animator.SetTrigger("explode");
            //Debug.Log("boom");

            startTime(Vector2.Distance(player.transform.position, this.transform.position));
            
        }
    }

    private void startTime(double t)
    {

        time = t;
        
    }

    private void FixedUpdate()
    {
        if (time > 0)
        {
            time -= 20f * Time.deltaTime;
        }
        if (time < 0)
        {
            if (inDistance)
            {
                player.GetComponent<PlayerController>().kill();
                //Debug.Log("Kill");
            }
            time = 0;
        }

        inDistance = (Vector2.Distance(player.transform.position, this.transform.position) <= 4.3f);

        //Debug.Log(inDistance);
    }
}
