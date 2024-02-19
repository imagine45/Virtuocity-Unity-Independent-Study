using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class batteryBehavior : MonoBehaviour
{
    /*public GameObject player;
    public GameObject battery;

    private enum states { RESPAWNS = 0, DOES_NOT_RESPAWN = 1 }
    [SerializeField] states state;
    private float respawnTimer = 0f;
    private float respawnRate = 5f;

    private bool collected = false;

    private void Start()
    {
        battery.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag.Equals("Player"))
        {
            player.GetComponent<PlayerController>().batteryCollect();
            battery.SetActive(false);
            collected = true;
        }
    }

    private void FixedUpdate()
    {
        if (state == 0)
        {
            if (collected)
            {
                respawnTimer += Time.deltaTime;
            }
            if (respawnTimer >= respawnRate)
            {
                collected = false;
                battery.SetActive(true);
            }
            print(respawnTimer);
        }
    }*/
}
