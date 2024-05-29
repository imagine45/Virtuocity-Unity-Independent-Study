using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hologramBehavior : MonoBehaviour
{

    private GameObject player;
    public Animator animator;
    private float radius;
    private bool inRadius = false;

    void Start()
    {
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);

        player = GameObject.Find("Player");
        radius = 15f;
    }

    void Update()
    {
        InRadius();
    }

    private void InRadius ()
    {
        if (Vector3.Distance(player.transform.position, this.transform.position) <= radius && inRadius == false)
        {
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            inRadius = true;
            animator.SetTrigger("start");
            Object.Destroy(this);
        }
    }
}
