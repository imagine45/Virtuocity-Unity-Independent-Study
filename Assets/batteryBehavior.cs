using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class batteryBehavior : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] float boostAmount = 2.5f;
    private GameObject player; 
    private bool near = false;
    private float time = 0;
    private float yPos;

    private void Start()
    {
        yPos = this.transform.position.y;
        player = GameObject.Find("Player");
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

    private void isNear()
    {
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

    public float BoostAmount()
    {
        return boostAmount;
    }
}
