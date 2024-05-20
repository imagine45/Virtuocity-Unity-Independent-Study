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
        time += Time.deltaTime; 

        if(time >= 0.1)
        {
            player.GetComponent<PlayerController>().kill();
        }
        Debug.Log(time);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        time = 0;
    }
}
