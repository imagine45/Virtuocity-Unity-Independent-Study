using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chargeMeter : MonoBehaviour
{
    public GameObject player;
    public GameObject meter;
    private float charge;

    private void Start()
    {
    }
    private void Update()
    {
        charge = player.GetComponent<PlayerController>().getCharge();
        meter.GetComponent<SpriteRenderer>().color = new Color((255f - 20 * charge)/255, (55f - 20 * charge) / 255, (40f + 10 * charge) / 255);
        this.transform.localScale = new Vector3(charge / 10, 1, 1);
    }
}
