using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class DeathTransition : MonoBehaviour
{
    
    private GameObject player;
    public Animator animator;

    private void Start()
    {
        player = GameObject.Find("Player");
        transform.localScale = new Vector3(50, transform.localScale.y * Screen.height/1080, 10);
    }

    void Update()
    {
        animator.SetBool("isDead", player.GetComponent<PlayerController>().isDead);
    }
    
}
