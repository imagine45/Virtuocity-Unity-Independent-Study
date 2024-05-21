using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class batteryBehavior : MonoBehaviour
{
    [SerializeField] Animator animator;
    private bool near = false;
    private float time = 0;
    private float yPos;

    private void Start()
    {
        yPos = this.transform.position.y;
    }

    private void FixedUpdate()
    {
        isNear();
        idleBob();
    }

    public void isCollected()
    {
        animator.SetTrigger("Collected");
    }

    public void respawned()
    {
        animator.SetTrigger("Respawned");
    }

    private void isNear()
    {
        GameObject player = GameObject.Find("Player");
        if (Vector3.Distance(player.transform.position, transform.position) < 3 && !near)
        {
            near = true;
            animator.SetTrigger("isNear");
            StartCoroutine(lightning(3));
        }
    }

    IEnumerator lightning(int time)
    {
        yield return new WaitForSeconds(time);

        near = false;
    }

    private void idleBob()
    {
        if (time >= Mathf.PI)
        {
            time = 0;
        } else
        {
            time += Time.deltaTime;
        }

        this.transform.position = new Vector3(transform.position.x, yPos + Mathf.Sin(time) / 4, 0);

    }
}
